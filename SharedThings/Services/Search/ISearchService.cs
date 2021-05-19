using Azure;
using Azure.Search.Documents.Models;
using SharedThings.Models;
using SharedThings.SearchModels;

namespace SharedThings.Services.Search
{
    public interface ISearchService
    {
        Response<SearchResults<CustomerInAzure>> GetResults(string sortField, string sortOrder, string q, int page, int pageSize);
        void AddOrUpdateSearchIndex(Customer customer);
    }
}