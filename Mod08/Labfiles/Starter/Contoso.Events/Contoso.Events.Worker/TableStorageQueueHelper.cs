using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Configuration;

namespace Contoso.Events.Worker
{
    
    public class TableStorageQueueHelper : StorageHelper, IQueueHelper<CloudQueueMessage>
    {
        private readonly CloudQueueClient _queueClient;
        private readonly string _signInQueueName;

        public TableStorageQueueHelper()
            : base()
        {
            _queueClient = base.StorageAccount.CreateCloudQueueClient();
            _signInQueueName = ConfigurationManager.AppSettings.Get("SignInQueueName");
        }

        public IQueueMessage<CloudQueueMessage> Receive()
        {

            CloudQueue queue = _queueClient.GetQueueReference(_signInQueueName);
            queue.CreateIfNotExists();

            CloudQueueMessage msg = queue.GetMessage();
            return new TableStorageQueueMessage(msg);
        }

        public void CompleteMessage(CloudQueueMessage message)
        {
            CloudQueue queue = _queueClient.GetQueueReference(_signInQueueName);
            queue.CreateIfNotExists();
            queue.DeleteMessage(message);
        }

        public void AbandonMessage(CloudQueueMessage message)
        {

        }
    }
}