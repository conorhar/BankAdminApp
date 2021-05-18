using System;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using SharedThings.Data;
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
            
            //var pageableResult = searchResult.Value.GetResults();
            //var customerSearchModel = new CustomerSearchModel{ TotalCount = Convert.ToInt32(searchResult.Value.TotalCount) };
            
            //foreach (var result in pageableResult)
            //{
            //    customerSearchModel.Customers.Add(_dbContext.Customers.First(r => r.CustomerId == Convert.ToInt32(result.Document.Id)));   
            //}

            //return customerSearchModel;
            
            return searchResult;
        }
    }
}