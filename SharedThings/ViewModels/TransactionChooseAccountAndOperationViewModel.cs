using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SharedThings.ViewModels
{
    public class TransactionChooseAccountAndOperationViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName  { get; set; }
        
        [Range(1, 1000000, ErrorMessage = "Please choose an operation")]
        public int SelectedOperationId { get; set; }
        public List<SelectListItem> AllOperations { get; set; }

        [Range(1, 1000000, ErrorMessage = "Please choose an account")]
        public int SelectedAccountId { get; set; }
        public List<SelectListItem> AllAccounts { get; set; }
    }
}