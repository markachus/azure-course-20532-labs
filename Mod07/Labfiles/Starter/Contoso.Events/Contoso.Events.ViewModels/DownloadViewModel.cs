using Contoso.Events.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace Contoso.Events.ViewModels
{
    public class DownloadViewModel
    {
        private readonly CloudStorageAccount _storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["Microsoft.WindowsAzure.Storage.ConnectionString"]);
        private readonly string _blobId;

        public DownloadViewModel(string blobId)
        {
            _blobId = blobId;
        }

        
        public async Task<DownloadPayload> GetStream()
        {
            var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
            var container = cloudBlobClient.GetContainerReference("signin");
            container.CreateIfNotExists();

            var realBlobId = _blobId.Substring(0, _blobId.IndexOf("."));
            var ret = new DownloadPayload();
            ret.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            ret.Stream = await container.GetBlockBlobReference(realBlobId).OpenReadAsync();

            return await Task.FromResult<DownloadPayload>(ret);
        }

        
        public async Task<string> GetSecureUrl()
        {
            SharedAccessBlobPolicy blobPolicy = new SharedAccessBlobPolicy();
            blobPolicy.SharedAccessExpiryTime = DateTime.Now.AddMinutes(15);
            blobPolicy.Permissions = SharedAccessBlobPermissions.Read;

            BlobContainerPermissions blobPermissions = new BlobContainerPermissions();
            blobPermissions.PublicAccess = BlobContainerPublicAccessType.Off;
            blobPermissions.SharedAccessPolicies.Add("ReadBlobPolicy", blobPolicy);

            var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
            var container = cloudBlobClient.GetContainerReference("signin");
            container.CreateIfNotExists();

            await container.SetPermissionsAsync(blobPermissions);

            var sas = container.GetSharedAccessSignature(new SharedAccessBlobPolicy(), "ReadBlobPolicy");
            var blob = container.GetBlobReference(Path.GetFileNameWithoutExtension(_blobId));
            return await Task.FromResult<string>($"{blob.Uri}{sas}");
        }
    }
}