using System;
using System.ComponentModel.DataAnnotations;

namespace SharedThings.Models
{
    public class DailyReport
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        
        [MaxLength(50)]
        public string Country { get; set; }

        [MaxLength(100)]
        public string EmailHeader { get; set; }
        
        [MaxLength(1000)]
        public string EmailBody { get; set; }
    }
}