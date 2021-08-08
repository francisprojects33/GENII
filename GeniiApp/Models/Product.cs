using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GeniiApp.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [Display(Name = "Number of product items in stock")]
        public int ProductsItemInStock { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        [Display(Name = "Cost per item")]
        public decimal CostPerItem { get; set; }

        [Required]
        [Display(Name = "Name of the item")]
        public string ItemName { get; set; }

        public IList<ProductInvoice> ProductInvoices { get; set; }
    }
}
