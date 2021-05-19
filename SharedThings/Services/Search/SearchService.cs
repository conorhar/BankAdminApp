using System;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using SharedThings.Data;
using SharedThings.Models;
using SharedThings.SearchModels;

namespace SharedThings.Services.Search
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _dbContext;
        private string indexName = "customers";
        private string searchUrl = "https://mvc2banksearch.search.windows.net";
        private string key = "7944A86AC5708C189525F978CDC0D7BA";

        public SearchService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Response<SearchResults<CustomerInAzure>> GetResults(string sortField, string sortOrder, string q, int page, int pageSize)
        {
            var searchClient = new SearchClient(new Uri(searchUrl),
                indexName, new AzureKeyCredential(key));

            int howManyRecordsToSkip = (page - 1) * pageSize;

            var searchOptions = new SearchOptions
            {
                OrderBy = { sortField + " " + sortOrder },
                Skip = howManyRecordsToSkip,
                Size = pageSize,
                IncludeTotalCount = true
            };

            var searchResult = searchClient.Search<CustomerInAzure>(q, searchOptions);
            
            return searchResult;
        }

        public void AddOrUpdateSearchIndex(Customer customer)
        {
            var searchClient = new SearchClient(new Uri(searchUrl), indexName, new AzureKeyCredential(key));
            var batch = new IndexDocumentsBatch<CustomerInAzure>();

            var customerInAzure = new CustomerInAzure
            {
                Id = customer.CustomerId.ToString(),
                SortableId = customer.CustomerId,
                FirstName = customer.Givenname,
                Surname = customer.Surname,
                City = customer.City,
                Address = customer.Streetaddress,
                Birthday = customer.Birthday
            };

            batch.Actions.Add(new IndexDocumentsAction<CustomerInAzure>(IndexActionType.MergeOrUpload, customerInAzure));
            IndexDocumentsResult result = searchClient.IndexDocuments(batch);
        }
    }
}