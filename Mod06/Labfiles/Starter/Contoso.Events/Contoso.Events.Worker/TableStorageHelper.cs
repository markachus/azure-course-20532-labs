using Contoso.Events.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contoso.Events.Worker
{
    public sealed class TableStorageHelper : StorageHelper
    {
        private readonly CloudTableClient _tableClient;

        public TableStorageHelper()
            : base()
        {
            _tableClient = base.StorageAccount.CreateCloudTableClient();
        }

        
        public IEnumerable<string> GetRegistrantNames(string eventKey)
        {
            CloudTable table = _tableClient.GetTableReference("EventRegistrations");

            string partitionKey = eventKey;
            string filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);

            var query = new TableQuery<Registration>().Where(filter);
            IEnumerable<Registration> registration = table.ExecuteQuery<Registration>(query);

            IEnumerable<string> names = registration
                                            .OrderBy(r => r.LastName)
                                            .ThenBy(r => r.FirstName)
                                            .Select(r => $"{r.FirstName}, {r.LastName}");

            return names.ToList();
        }
    }
}