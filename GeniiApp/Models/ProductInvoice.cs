using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GeniiApp.Models
{
    public class ProductInvoice
    {
        [Key]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        [Key]
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public int QuantitySold { get; set; }
    }
}
