using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using System.Configuration;

namespace Contoso.Events.Worker
{
    
    public sealed class ServiceBusQueueHelper : IQueueHelper<BrokeredMessage>
    {
        private readonly QueueClient _client;

        public ServiceBusQueueHelper()
        {
            string sbusConnection = ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
            string queueName = ConfigurationManager.AppSettings["SignInQueueName"];
            _client = QueueClient.CreateFromConnectionString(sbusConnection, queueName);
        }

        public IQueueMessage<BrokeredMessage> Receive()
        {
           BrokeredMessage message = _client.Receive();
            return new ServiceBusQueueMessage(message);
        }

        public void CompleteMessage(BrokeredMessage message)
        {
            message.Complete();
        }

        public void AbandonMessage(BrokeredMessage message)
        {
            message.Abandon();
        }
    }
}