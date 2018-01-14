using Contoso.Events.Data;
using Contoso.Events.Models;
using Contoso.Events.ViewModels.Dynamic;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Contoso.Events.ViewModels
{
    public class RegisterViewModel
    {
        public RegisterViewModel()
        { }

        
        public RegisterViewModel(string eventKey)
        {
            using (EventsContext context = new EventsContext())
            {
                this.Event = context.Events.SingleOrDefault(e => e.EventKey == eventKey);
            }

            string connectionstring = ConfigurationManager.AppSettings.Get("Microsoft.WindowsAzure.Storage.ConnectionString");
            CloudStorageAccount account = Microsoft.WindowsAzure.Storage.CloudStorageAccount.Parse(connectionstring);
            var client = account.CreateCloudTableClient();
            var table = client.GetTableReference("EventRegistrations");
            table.CreateIfNotExists();


            string partitionKey = $"Stub_{eventKey}";
            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);

            var query = new TableQuery().Where(filter);
            IEnumerable<DynamicTableEntity> tableEntities = table.ExecuteQuery(query);
            DynamicTableEntity tableEntity = tableEntities.SingleOrDefault();
            this.RegistrationStub = DynamicEntity.GenerateDynamicItem(tableEntity);
        }

        public Event Event { get; set; }

        public Registration RegistrationStub { get; set; }
    }
}