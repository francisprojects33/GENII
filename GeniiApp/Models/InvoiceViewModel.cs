using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeniiApp.Models
{
    public class InvoiceViewModel
    {
        public int InvoiceId { get; set; }

        [Display(Name = "User created the invoice")]
        public string CreateByUser { get; set; }

        [Display(Name = "Created date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Total of the invoice")]
        public int TotalInvoice { get; set; }

        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Display(Name = "Surname")]
        public string SurName { get; set; }



        //public IList<ProductInvoice> ProductInvoices { get; set; }
    }

}
