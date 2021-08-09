using GeniiApp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeniiApp.Models;
using GeniiApp.StocksRepo;
using Microsoft.EntityFrameworkCore;

namespace GeniiApp.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? id)
        {
            return View();
        }


        // 1. The number of Items sold per product
        public async Task<List<string>> GetNumberOfItemsSoldPerProduct()
        {
            List<string> itemsSoldPerProduct = new List<string>();

            var numberOfItemsSoldPerProduct = await _context.ProductInvoices
                .Where(x => x.ProductId != 0)
                .GroupBy(x => new { x.ProductId, x.Product.ItemName })
                .Select(grp => new StockProductItems
                {
                    ProductItemId = grp.Key.ProductId,
                    ProductItemName = grp.Key.ItemName,
                    ProductItemSoldQuantity = grp.Sum(item => item.QuantitySold)
                })
                .ToListAsync();

            foreach (var item in numberOfItemsSoldPerProduct)
            {
                var productDetail = item.ProductItemName + " : " + item.ProductItemSoldQuantity;
                itemsSoldPerProduct.Add(productDetail);
            }

            return itemsSoldPerProduct;
        }

        // 2. The number of products
        public int GetNumberOfProducts()
        {
            var numberOfProducts = _context.Products.Count();

            return numberOfProducts;
        }

        // 3. The number of products sold
        public int GetNumberOfProductsSold()
        {
            var numberOfProductsSold = _context.ProductInvoices.Select(x => x.ProductId).Distinct().Count();

            return numberOfProductsSold;
        }

        //4. The number of products in stock
        public int GetnumberOfProductsInStock()
        {
            var numberOfProductsInStock = _context.Products.Select(p => p.ProductId).Count();

            return numberOfProductsInStock;
        }

        //5. The number of products in stock vs sold
        public string GetnumberOfProductsInStockVsSold()
        {
            var numberOfProductsInStock = _context.Products.Select(p => p.ProductId).Count();
            var numberOfProductsSold = _context.ProductInvoices.Select(x => x.ProductId).Distinct().Count();
            //var numberOfProductsInStockVsSold = numberOfProductsInStock - numberOfProductsSold;

            string numberOfProductsInStockVsSold = numberOfProductsInStock + " / " + numberOfProductsSold;

            return numberOfProductsInStockVsSold;
        }

    }

    public class ProductReport
    {
        public int NumberOfItemsSoldPerProduct { get; set; }
        public int NumberOfProducts { get; set; }
        public int NumberOfProductsSold { get; set; }
        public int NumberOfProductsInStock { get; set; }
        public int NumberOfProductsInStockVsSold { get; set; }
    }
}
