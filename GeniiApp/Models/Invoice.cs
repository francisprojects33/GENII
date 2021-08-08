using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GeniiApp.Models
{
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Display(Name = "Created date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "User created the invoice")]
        public Guid CreateByUser { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Total of the invoice")]
        public int TotalInvoice { get; set; }

        public IList<ProductInvoice> ProductInvoices { get; set; }
    }
}
