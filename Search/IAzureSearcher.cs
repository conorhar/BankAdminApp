using System;
using Azure;
using Azure.Search.Documents;
using SharedThings.SearchModels;

namespace Search
{
    public interface IAzureSearcher
    {
        void Run();
    }

    class AzureSearcher : IAzureSearcher
    {
        private string indexName = "customers";
        private string searchUrl = "https://mvc2banksearch.search.windows.net";
        private string key = "7944A86AC5708C189525F978CDC0D7BA";

        public void Run()
        {
            var searchClient = new SearchClient(new Uri(searchUrl),
                indexName, new AzureKeyCredential(key));

            while (true)
            {
                Console.Write("Ange sökord: ");
                string search = Console.ReadLine();

                //var searchOptions = new SearchOptions
                //{
                //    OrderBy = { },
                //    Skip = 10,
                //    Size = 20
                //};

                var searchResult = searchClient.Search<CustomerInAzure>(search);

                foreach (var result in searchResult.Value.GetResults())
                {
                    Console.WriteLine(result.Document.Id);
                }
            }
        }
    }
}