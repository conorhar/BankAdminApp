using System;
using Azure.Search.Documents.Indexes;

namespace SharedThings.SearchModels
{
    public class CustomerInAzure
    {
        [SimpleField(IsKey = true, IsFilterable = true, IsSortable = true)]
        public string Id { get; set; }

        [SearchableField(IsSortable = true)]
        public string FirstName { get; set; }

        [SearchableField(IsSortable = true)]
        public string Surname { get; set; }

        [SearchableField(IsSortable = true)]
        public string City { get; set; }

        [SimpleField(IsSortable = true)]
        public string Address { get; set; }

        [SimpleField(IsSortable = true)]
        public DateTime? Birthday { get; set; }
    }
}