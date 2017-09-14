using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Emotion;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace PhotoSharingApp.Functions
{
    public static class GenerateKeywords
    {
        private static string DocumentDatabaseId = ConfigurationManager.AppSettings["DocumentDatabaseId"];
        private static string DocumentCollectionId = ConfigurationManager.AppSettings["DocumentCollectionId"];
        private static string DocumentEndpointUrl = ConfigurationManager.AppSettings["DocumentEndpointUrl"];
        private static string DocumentAuthorizationKey = ConfigurationManager.AppSettings["DocumentAuthorizationKey"];
        private static string DocumentCollectionUri = $"dbs/{DocumentDatabaseId}/colls/{DocumentCollectionId}";
        private static string DocumentTypeIdentifier = "PHOTO";
        private static string DocumentVersion = "1.0";
        private static string VisionApiSubscriptionKey = ConfigurationManager.AppSettings["VisionApiSubscriptionKey"];
        private static string EmotionApiSubscriptionKey = ConfigurationManager.AppSettings["EmotionApiSubscriptionKey"];
        private static string VisionApiRequestUrlTag = ConfigurationManager.AppSettings["VisionApiRequestUrlTag"];
        private static string VisionApiRequestUrlDescribe = ConfigurationManager.AppSettings["VisionApiRequestUrlDescribe"];


        [FunctionName("GenerateKeywords")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "GenerateKeywords/{photoId}")]HttpRequestMessage req, string photoId, TraceWriter log)
        {
            log.Info($"GenerateKeywords HTTP trigger processing a request. Photo ID: {photoId}");

            HttpStatusCode returnStatusCode = HttpStatusCode.BadRequest;

            try
            {
                var documentClient = new DocumentClient(new Uri(DocumentEndpointUrl), DocumentAuthorizationKey);

                //var visionServiceClient = new VisionServiceClient(VisionApiSubscriptionKey); // Issues so calling rest api direct
                //var emotionServiceClient = new VisionServiceClient(EmotionApiSubscriptionKey); // Issues so calling rest api direct
                var visionHttpClient = new HttpClient();
                visionHttpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", VisionApiSubscriptionKey);

                Dictionary<string, object> document =
                    documentClient.CreateDocumentQuery<Dictionary<string, object>>(DocumentCollectionUri,
                        $"SELECT * FROM root r WHERE r.DocumentType = '{DocumentTypeIdentifier}' AND r.DocumentVersion = '{DocumentVersion}' AND r.id = '{photoId}'")
                    .AsEnumerable().FirstOrDefault();

                if (document == null)
                    return req.CreateResponse(HttpStatusCode.NotFound);

                if (!(document.ContainsKey("Keywords") && document.ContainsKey("Description")) && document.ContainsKey("StandardUrl") && !string.IsNullOrWhiteSpace((string)document["StandardUrl"]))
                {
                    var keywords = new List<string>();
                    string caption = null;

                    // Let's use Cog Services to get some keywords
                    var imageUrl = (string)document["StandardUrl"];

                    if (!document.ContainsKey("Keywords"))
                    {
                        //var analysisResult = await visionServiceClient.GetTagsAsync(imageUrl); // Bombs out so calling rest api direct

                        var response = visionHttpClient.PostAsync(VisionApiRequestUrlTag, new StringContent($"{{\"url\":\"{imageUrl}\"}}", Encoding.UTF8, "application/json")).Result;
                        var json = response.Content.ReadAsStringAsync().Result;
                        var tagObject = JsonConvert.DeserializeObject<TagObject>(json);

                        if (tagObject?.Tags != null && tagObject?.Tags.Count > 0)
                        {
                            foreach (var tag in tagObject.Tags)
                            {
                                if (tag.Confidence > 0.3)
                                    keywords.Add(tag.Name);
                            }
                        }
                    }

                    if (!document.ContainsKey("Description"))
                    {
                        var response = visionHttpClient.PostAsync(VisionApiRequestUrlDescribe, new StringContent($"{{\"url\":\"{imageUrl}\"}}", Encoding.UTF8, "application/json")).Result;
                        var json = response.Content.ReadAsStringAsync().Result;
                        var descibeObject = JsonConvert.DeserializeObject<DescribeObject>(json);

                        if (descibeObject?.description?.Captions != null && descibeObject?.description?.Captions.Count > 0)
                        {
                            var topCaption = descibeObject.description.Captions.OrderByDescending(c => c.Confidence).FirstOrDefault();
                            if (topCaption?.Confidence > 0.75)
                                caption = topCaption.Text;
                        }
                    }

                    var hasChanged = false;

                    if (keywords.Count > 0)
                    {
                        document.Add("Keywords", keywords);
                        hasChanged = true;
                    }

                    if (!string.IsNullOrWhiteSpace(caption))
                    {
                        document.Add("Description", caption);
                        hasChanged = true;
                    }

                    returnStatusCode = HttpStatusCode.NoContent; // We want to assume a successful outcome at this point.

                    if (hasChanged)
                    {
                        // Save document back to store
                        var response = documentClient.UpsertDocumentAsync(DocumentCollectionUri, document).Result;
                        if ((int)response.StatusCode > 299)
                            returnStatusCode = HttpStatusCode.InternalServerError; // Assume error writing back to doc store
                    }
                }
                else
                {
                    returnStatusCode = HttpStatusCode.NotModified;
                }

            }
            catch (Exception ex)
            {
                req.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
                throw;
            }

            return req.CreateResponse(returnStatusCode);
        }
    }

    public class TagObject
    {
        [JsonProperty(PropertyName = "tags")]
        public List<Tag> Tags { get; set; }
        [JsonProperty(PropertyName = "requestId")]
        public string RequestId { get; set; }
        [JsonProperty(PropertyName = "metadata")]
        public Metadata Metadata { get; set; }
    }

    public class DescribeObject
    {
        public Description description { get; set; }
        public string requestId { get; set; }
        public Metadata metadata { get; set; }
    }

    public class Tag
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "confidence")]
        public double Confidence { get; set; }
        [JsonProperty(PropertyName = "hint")]
        public string Hint { get; set; }
    }

    public class Description
    {
        [JsonProperty(PropertyName = "tags")]
        public List<string> Tags { get; set; }
        [JsonProperty(PropertyName = "captions")]
        public List<Caption> Captions { get; set; }
    }
    public class Caption
    {
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }
        [JsonProperty(PropertyName = "confidence")]
        public double Confidence { get; set; }
    }

    public class Metadata
    {
        [JsonProperty(PropertyName = "width")]
        public int Width { get; set; }
        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }
        [JsonProperty(PropertyName = "format")]
        public string Format { get; set; }
    }
}
