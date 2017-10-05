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
        private static string VisionApiRequestUrlAnalyze = ConfigurationManager.AppSettings["VisionApiRequestUrlAnalyze"];
        private static string EmotionApiRequestUrlRecognize = ConfigurationManager.AppSettings["EmotionApiRequestUrlRecognize"];
        private static string ImageUrlBase = ConfigurationManager.AppSettings["ImageUrlBase"];

        [FunctionName("GenerateKeywords")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "GenerateKeywords/{photoId}")]HttpRequestMessage req, string photoId, TraceWriter log)
        {
            log.Info($"GenerateKeywords HTTP trigger processing a request. Photo ID: {photoId}");

            HttpStatusCode returnStatusCode = HttpStatusCode.BadRequest;

            try
            {
                var documentClient = new DocumentClient(new Uri(DocumentEndpointUrl), DocumentAuthorizationKey);

                //var visionServiceClient = new VisionServiceClient(VisionApiSubscriptionKey); // Had issues when using Functions so calling rest api direct
                //var emotionServiceClient = new VisionServiceClient(EmotionApiSubscriptionKey); // Had issues when using Functions so calling rest api direct

                var visionHttpClient = new HttpClient();
                visionHttpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", VisionApiSubscriptionKey);

                var emotionHttpClient = new HttpClient();
                emotionHttpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", EmotionApiSubscriptionKey);

                Dictionary<string, object> document =
                    documentClient.CreateDocumentQuery<Dictionary<string, object>>(DocumentCollectionUri,
                        $"SELECT * FROM root r WHERE r.DocumentType = '{DocumentTypeIdentifier}' AND r.DocumentVersion = '{DocumentVersion}' AND r.id = '{photoId}'")
                    .AsEnumerable().FirstOrDefault();

                if (document == null)
                    return req.CreateResponse(HttpStatusCode.NotFound);

                if (document.ContainsKey("StandardUrl") && !string.IsNullOrWhiteSpace((string)document["StandardUrl"]))
                {
                    var keywords = document.ContainsKey("Keywords") ? JsonConvert.DeserializeObject<List<string>>((string)document["Keywords"]) : new List<string>();
                    string caption = null;

                    // Let's use Cog Services to get some keywords
                    var imageUrl = $"{ImageUrlBase}{(string)document["StandardUrl"]}";

                    var visionResponse = visionHttpClient.PostAsync(VisionApiRequestUrlAnalyze, new StringContent($"{{\"url\":\"{imageUrl}\"}}", Encoding.UTF8, "application/json")).Result;
                    var visionJson = visionResponse.Content.ReadAsStringAsync().Result;
                    var visionObject = JsonConvert.DeserializeObject<VisionObject>(visionJson);

                    if (visionObject != null)
                    {
                        bool isPerson = false;
                        if (visionObject?.Tags != null && visionObject?.Tags.Count > 0)
                        {
                            foreach (var tag in visionObject.Tags)
                            {
                                if (!keywords.Any(t => t == tag.Name) && tag.Confidence > 0.3)
                                {
                                    keywords.Add(tag.Name);
                                    if (tag.Name == "person")
                                        isPerson = true;
                                }
                            }
                        }
                        // If category is people related and there is a tag of person
                        if (isPerson && visionObject.Categories.Any(c => c.Name.StartsWith("people") && c.Score > 0.3))
                        {
                            var emotionResponse = emotionHttpClient.PostAsync(EmotionApiRequestUrlRecognize, new StringContent($"{{\"url\":\"{imageUrl}\"}}", Encoding.UTF8, "application/json")).Result;
                            var emotionJson = emotionResponse.Content.ReadAsStringAsync().Result;
                            var emotionObjectList = JsonConvert.DeserializeObject<List<EmotionObject>>(emotionJson);

                            // Will only add emotion keyword if person/people are happy or surprised
                            if (emotionObjectList != null && emotionObjectList.Count > 0)
                            {
                                foreach (var emotionObject in emotionObjectList)
                                {
                                    var scores = emotionObject.Scores;
                                    if (scores.Happiness > 0.8 && scores.Happiness <= 1.0 && !keywords.Any(t => t == "happy"))
                                        keywords.Add("happy");
                                    if (scores.Surprise > 0.8 && scores.Surprise <= 1.0 && !keywords.Any(t => t == "surprised"))
                                        keywords.Add("surprised");
                                }
                            }
                        }
                    }

                    if (visionObject != null && (!document.ContainsKey("Description") || string.IsNullOrEmpty((string)document["Description"])))
                    {
                        if (visionObject.Description?.Captions != null && visionObject?.Description?.Captions.Count > 0)
                        {
                            var topCaption = visionObject.Description.Captions.OrderByDescending(c => c.Confidence).FirstOrDefault();
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

    public class VisionObject
    {
        [JsonProperty("categories")]
        public List<Category> Categories { get; set; }
        [JsonProperty("tags")]
        public List<Tag> Tags { get; set; }
        [JsonProperty("description")]
        public Description Description { get; set; }
        [JsonProperty("requestId")]
        public string RequestId { get; set; }
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }
    }

    public class Category
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("score")]
        public double Score { get; set; }
    }

    public class Tag
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }

    public class Caption
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("confidence")]
        public double Confidence { get; set; }
    }

    public class Description
    {
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
        [JsonProperty("captions")]
        public List<Caption> Captions { get; set; }
    }

    public class Metadata
    {
        [JsonProperty("width")]
        public int Width { get; set; }
        [JsonProperty("height")]
        public int Height { get; set; }
        [JsonProperty("format")]
        public string Format { get; set; }
    }

    public class EmotionObject
    {
        [JsonProperty("faceRectangle")]
        public FaceRectangle FaceRectangle { get; set; }
        [JsonProperty("scores")]
        public Scores Scores { get; set; }
    }

    public class FaceRectangle
    {
        [JsonProperty("left")]
        public long Left { get; set; }
        [JsonProperty("height")]
        public long Height { get; set; }
        [JsonProperty("top")]
        public long Top { get; set; }
        [JsonProperty("width")]
        public long Width { get; set; }
    }

    public class Scores
    {
        [JsonProperty("fear")]
        public double Fear { get; set; }
        [JsonProperty("contempt")]
        public double Contempt { get; set; }
        [JsonProperty("anger")]
        public double Anger { get; set; }
        [JsonProperty("disgust")]
        public double Disgust { get; set; }
        [JsonProperty("neutral")]
        public double Neutral { get; set; }
        [JsonProperty("happiness")]
        public double Happiness { get; set; }
        [JsonProperty("sadness")]
        public double Sadness { get; set; }
        [JsonProperty("surprise")]
        public double Surprise { get; set; }
    }
}
