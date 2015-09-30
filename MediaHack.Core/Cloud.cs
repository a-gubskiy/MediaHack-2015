using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MediaHack.Core
{
    public class Cloud
    {
        public string StorageConnectionString { get; private set; }
        public string CdnIdentifier { get; private set; }

        private const String ContainerName = "videos";

        public Cloud()
        {

        }

        /// <summary>
        /// Retrieve a reference to a container
        /// </summary>
        /// <returns></returns>
        private CloudBlobContainer GetBlobContainer()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting(StorageConnectionString));
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
            var blockBlob = container.GetBlockBlobReference(Path.GetFileNameWithoutExtension(path));

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(path))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            return CreateVideo(blockBlob);
        }

        private Video CreateVideo(CloudBlob blob)
        {
            return new Video
            {
                Name = blob.Name,
                StorageUrl = blob.Uri.ToString(),
                ContentDeliveryNetworkUrl = $"https://{CdnIdentifier}.vo.mscend.net/{ContainerName}/{blob.Name}"
            };
        }
    }
}
