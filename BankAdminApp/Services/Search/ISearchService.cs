using System.Collections.Generic;
using Azure;
using Azure.Search.Documents.Models;
using BankAdminApp.Models;
using SharedThings.Models;
using SharedThings.SearchModels;

namespace BankAdminApp.Services.Search
{
    public interface ISearchService
    {
        Response<SearchResults<CustomerInAzure>> GetResults(string sortField, string sortOrder, string q, int page, int pageSize);
    }
}