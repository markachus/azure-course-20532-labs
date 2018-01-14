using Contoso.Events.Data;
using Contoso.Events.Models;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;

namespace Contoso.Events.ViewModels
{
    public class SignInSheetViewModel
    {
        private const string PROCESSING_URI = "uri://processing";

        public SignInSheetViewModel()
        { }

        public SignInSheetViewModel(string eventKey)
        {
            this.SignInSheetState = default(SignInSheetState);

            using (EventsContext context = new EventsContext())
            {
                var eventItem = context.Events.SingleOrDefault(e => e.EventKey == eventKey);

                this.Event = eventItem;

                if (this.Event.SignInDocumentUrl == PROCESSING_URI)
                {
                    this.SignInSheetState = SignInSheetState.SignInDocumentProcessing;
                }
                else if (!String.IsNullOrEmpty(this.Event.SignInDocumentUrl))
                {
                    this.SignInSheetState = SignInSheetState.SignInDocumentAlreadyExists;
                }
                else
                {
                    QueueMessage message = new QueueMessage
                    {
                        EventId = eventItem.Id,
                        MessageType = QueueMessageType.SignIn
                    };
                    string messageString = JsonConvert.SerializeObject(message);

                    GenerateSignInSheetServiceBus(context, eventItem, message);
                    //GenerateSignInSheetTableStorage(context, eventItem, messageString);
                }
            }
        }

        private string tableStorageConnectionString = WebConfigurationManager.AppSettings["Microsoft.WindowsAzure.Storage.ConnectionString"];
        private string serviceBusConnectionString = WebConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"];
        private string signInQueueName = WebConfigurationManager.AppSettings["SignInQueueName"];

        private void GenerateSignInSheetServiceBus(EventsContext context, Event eventItem, QueueMessage message)
        {
            eventItem.SignInDocumentUrl = PROCESSING_URI;

            context.SaveChanges();

            this.Event = eventItem;

            this.SignInSheetState = SignInSheetState.SignInDocumentProcessing;

            QueueClient queueClient = QueueClient.CreateFromConnectionString(serviceBusConnectionString, signInQueueName);
            queueClient.Send(new BrokeredMessage(message));
        }

        private void GenerateSignInSheetTableStorage(EventsContext context, Event eventItem, string message)
        {
            eventItem.SignInDocumentUrl = PROCESSING_URI;

            context.SaveChanges();

            this.Event = eventItem;

            this.SignInSheetState = SignInSheetState.SignInDocumentProcessing;

            CloudStorageAccount account = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["Microsoft.WindowsAzure.Storage.ConnectionString"]);
            CloudQueueClient client = account.CreateCloudQueueClient();
            CloudQueue queue = client.GetQueueReference(signInQueueName);
            queue.AddMessage(new CloudQueueMessage(message));

        }

        public void GenerateSignInSheet(int eventId)
        {
            using (EventsContext context = new EventsContext())
            {
                var eventItem = context.Events.SingleOrDefault(e => e.Id == eventId);

                eventItem.SignInDocumentUrl = PROCESSING_URI;

                context.SaveChanges();

                this.Event = eventItem;
            }

            this.SignInSheetState = SignInSheetState.SignInDocumentProcessing;
        }

        public Event Event { get; set; }

        public SignInSheetState SignInSheetState { get; set; }
    }

    public enum SignInSheetState
    {
        Unknown = 0,
        SignInDocumentProcessing,
        SignInDocumentAlreadyExists
    }
}