//-----------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System.Web.Configuration;

namespace PhotoSharingApp.AppService.Shared.Context
{
    /// <summary>
    /// The service environment definition.
    /// </summary>
    public class EnvironmentDefinition : EnvironmentDefinitionBase
    {
        private DocumentDbStorage _documentDbStorage;

        /// <summary>
        /// The DocumentDB database.
        /// </summary>
        public override DocumentDbStorage DocumentDbStorage
        {
            get
            {
                if (_documentDbStorage == null)
                {
                    _documentDbStorage = new DocumentDbStorage
                    {
                        // We have supplied a default DatabaseId and CollectionId here, feel free to configure your own.
                        // On first time startup, the service will create a DocumentDB database and collection for you
                        // if none exist with these names.
                        DataBaseId = "photosharing-database",
                        CollectionId = "photosharing-document-data",
                        EndpointUrl = "https://snapgolddemodb.documents.azure.com:443/",
                        AuthorizationKey = "0e5TPr752unFx3Xk58ZC0AeHunPB9LKpugdUjTm0OCM1PFx9eASetyPDMbtnE03P0wh4rt4Fptbt5IKY3YW1Yw=="
                    };
                }

                return _documentDbStorage;
            }
        }

        /// <summary>
        /// The Notification Hub's default full shared access signature.
        /// </summary>
        public override string HubFullSharedAccessSignature
        {
            get
            {
                return "[Your Notification Hub's Full Shared Access Signature]";
            }
        }

        /// <summary>
        /// The Notification Hub's name.
        /// </summary>
        public override string HubName
        {
            get { return "[Your Notification Hub Name]"; }
        }

        /// <summary>
        /// The Application Insights instrumentation key. This value is read from the Web.config file.
        /// </summary>
        public override string InstrumentationKey
        {
            get { return "[You App Insights Instrumentation Key]"; }
        }

        /// <summary>
        /// The Azure Storage access key that is used for storing
        /// uploaded photos.
        /// </summary>
        public override string StorageAccessKey
        {
            get { return "UXWKEc4JJD/nFtoJ+31nLuMABErqdwbeTChkOijEOXdH4rm42vPicsf0wwLNXVLvqmrnDtzKB5nX+VMSU7/j/Q=="; }
        }

        /// <summary>
        /// The Azure Storage account name that is used for storing
        /// uploaded photos.
        /// </summary>
        public override string StorageAccountName
        {
            get { return "snapgolddemostorage"; }
        }
    }
}