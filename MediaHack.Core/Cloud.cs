using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaHack.Core
{
    public class Cloud
    {
        private CloudConfig _config;

        private const String ContainerName = "videos";    
        private const String NameField = "Name";

        public Cloud(string configurationPath)
        {
            var json = File.ReadAllText(configurationPath);
            _config = JsonConvert.DeserializeObject<CloudConfig>(json);
        }

        public Video GetVideos(string name)
        {
            var container = GetBlobContainer();
            var blob = container.GetBlobReference(name);
            return CreateVideo(blob);
        }

        /// <summary>
        /// Retrieve a reference to a container
        /// </summary>
        /// <returns></returns>
        private CloudBlobContainer GetBlobContainer()
        {
            var storageAccount = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={_config.StorageServiceAccountName};AccountKey={_config.StorageServiceAccessKey};");

            var blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            var container = blobClient.GetContainerReference(ContainerName);

            // Create the container if it doesn't already exist.
            if (container.CreateIfNotExists())
            {
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }

            return container;
        }

        public IEnumerable<Video> GetVideos()
        {
            var container = GetBlobContainer();

            var result = (from o in container.ListBlobs()
                          where o is CloudBlob
                          let b = o as CloudBlob
                          select CreateVideo(b)).ToList();

            return result;
        }
            
        public Video Upload(string path)
        {
            var container = GetBlobContainer();

            // Retrieve reference to a blob named "myblob".

            var blob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
            blob.Properties.ContentType = "video/mp4";
            blob.Metadata[NameField] = Path.GetFileNameWithoutExtension(path);
                              
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(path))
            {
                blob.UploadFromStream(fileStream);
            }

            return CreateVideo(blob);
        }

        private Video CreateVideo(CloudBlob blob)
        {
            if (blob == null)
            {
                return null;
            }

            blob.FetchAttributes();

            return new Video
            {
                Title = blob.Metadata[NameField],
                Name = blob.Name,
                StorageUrl = blob.Uri.ToString(),
                ContentDeliveryNetworkUrl = $"https://{_config.CdnEndpointName}.vo.mscend.net/{ContainerName}/{blob.Name}"
            };
        }
    }
}
