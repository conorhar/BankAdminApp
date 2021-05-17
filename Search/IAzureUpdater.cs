using System;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using SharedThings;
using SharedThings.Data;
using SharedThings.SearchModels;

namespace Search
{
    public interface IAzureUpdater
    {
        void Run();
    }

    class AzureUpdater : IAzureUpdater
    {
        private readonly ApplicationDbContext _dbContext;
        private string indexName = "customers";
        private string searchUrl = "https://mvc2banksearch.search.windows.net";
        private string key = "7944A86AC5708C189525F978CDC0D7BA";

        public AzureUpdater(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Run()
        {
            CreateIndexIfNotExists();

            var searchClient = new SearchClient(new Uri(searchUrl),
                indexName, new AzureKeyCredential(key));

            var batch = new IndexDocumentsBatch<CustomerInAzure>();

            foreach (var customer in _dbContext.Customers)
            {
                var customerInAzure = new CustomerInAzure()
                {
                    Id = customer.CustomerId.ToString(),
                    SortableId = customer.CustomerId,
                    FirstName = customer.Givenname,
                    Surname = customer.Surname,
                    City = customer.City,
                    Address = customer.Streetaddress,
                    Birthday = customer.Birthday
                };

                batch.Actions.Add(new IndexDocumentsAction<CustomerInAzure>(IndexActionType.MergeOrUpload,
                    customerInAzure));
            }

            IndexDocumentsResult result = searchClient.IndexDocuments(batch);
        }

        private void CreateIndexIfNotExists()
        {
            var serviceEndpoint = new Uri(searchUrl);
            var credential = new AzureKeyCredential(key);
            var adminClient = new SearchIndexClient(serviceEndpoint, credential);

            var fieldBuilder = new FieldBuilder();
            var searchFields = fieldBuilder.Build(typeof(CustomerInAzure));

            var definition = new SearchIndex(indexName, searchFields);

            adminClient.CreateOrUpdateIndex(definition);
        }
    }
}