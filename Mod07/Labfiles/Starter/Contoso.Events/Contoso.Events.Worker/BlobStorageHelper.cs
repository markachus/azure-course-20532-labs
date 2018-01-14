using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;

namespace Contoso.Events.Worker
{
    public sealed class BlobStorageHelper : StorageHelper
    {
        private readonly CloudBlobClient _blobClient;

        public BlobStorageHelper()
            : base()
        {
            _blobClient = base.StorageAccount.CreateCloudBlobClient();
        }

        
        public Uri CreateBlob(MemoryStream stream, string eventKey)
        {
            var container =_blobClient.GetContainerReference("signin");
            container.CreateIfNotExists();

            String blobName = $"{eventKey}_SignIn_{DateTime.UtcNow.ToString("yyyyMMdd")}";
            var blob = container.GetBlockBlobReference(blobName);

            stream.Seek(0, SeekOrigin.Begin);
            blob.UploadFromStream(stream);
            return blob.Uri;
        }
    }
}