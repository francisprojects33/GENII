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
        public async Task<List<StockProductItems>> GetNumberOfItemsSoldPerProduct()
        {

            //List<string> singleProductDetail = new List<string>();
            //var getProductsId = _context.Products.Select(i => i.ProductId);

            //foreach (var item in getProductsId)
            //{
            //    var getproductName = _context.Products.Where(i => i.ProductId == item).Select(n => n.ItemName).FirstOrDefault();

            //    var getProductsItemInStock = _context.Products.Where(x => x.ProductId == item).Select(x => x.ProductsItemInStock).FirstOrDefault();
            //    var getCountproductIdInInvoice = _context.ProductInvoices.Where(x => x.ProductId == item).Count();
            //    var numberOfItemsSoldPerProduct = getProductsItemInStock - getCountproductIdInInvoice;

            //    var productDetail = getproductName + " : " + numberOfItemsSoldPerProduct.ToString();

            //    singleProductDetail.Add(productDetail);
            //}

            //foreach (var item in singleProductDetail)
            //{
            //    Console.WriteLine(item);
            //}


            // ***********************************************

            // data context 1 (Id and quantitySold)
            var numberOfItemsSoldPerProduct = await _context.ProductInvoices.Where(x => x.ProductId != 0).GroupBy(x => x.ProductId).Select(grp => new StockProductItems { ProductItemId = grp.Key, ProductItemSoldQuantity = grp.Sum(item => item.QuantitySold) }).ToListAsync();

            //Console.WriteLine();

            //var productIds = numberOfItemsSoldPerProduct.Select(x => new StockProductItems { ProductItemId = x.ProductItemId }).ToList();

            // data context 2 (Id and Name)
            var numberOfItemsSoldPerProductNames = _context.Products.Select(x => new StockProductItems { ProductItemId = x.ProductId, ProductItemName = x.ItemName } ).ToList();

            // Merging both list together
            var numberOfItemsSoldPerProductAll = numberOfItemsSoldPerProduct.Union(numberOfItemsSoldPerProductNames);


            //.Join .Select(x => new StockProductItems { ProductItemId = x.ProductItemId, ProductItemName = x.ProductItemName })


            // ***********************************************
            // ***********************************************

            // 1. productName is not present
            //
            //IQueryable<StockProductItems> query =
            //    from c in _context.ProductInvoices
            //    join p in _context.Products
            //        on c.ProductId equals p.ProductId
            //    group c by c.ProductId into p
            //    select new StockProductItems { ProductItemId = p.Key, ProductItemSoldQuantity = p.Sum(x => x.QuantitySold) };


            // -----------

            // 2. working fine, pull all the 120 data
            //
            //IQueryable<string> query =
            //   from c in _context.ProductInvoices
            //   join p in _context.Products
            //       on c.ProductId equals p.ProductId
            //   select c.ProductId + " " + p.ItemName + " " + c.QuantitySold;


            // -----------

            // 2.1. working fine, pull all the 120 data. I just need to group by product id and sum
            //
            //IQueryable<string> query =
            //   from c in _context.ProductInvoices
            //   join p in _context.Products
            //       on c.ProductId equals p.ProductId
            //   group c by c.ProductId into p
            //   select c.ProductId + " " + p.ItemName + " " + c.QuantitySold;


            // -----------

            //IQueryable<string> query =
            //    from c in _context.ProductInvoices
            //    join p in _context.Products
            //        on c.ProductId equals p.ProductId
            //    group c by c.ProductId into p
            //    select c.ProductId + " " + p.ItemName + " " + c.QuantitySold;


            // ------------------

            var soldQtyForEachItem = await _context.ProductInvoices
                .Where(x => x.ProductId != 0)
                .GroupBy(x => new { x.ProductId, x.Product.ItemName })
                .Select(grp => new StockProductItems
                {
                    ProductItemId = grp.Key.ProductId,
                    ProductItemName = grp.Key.ItemName,
                    ProductItemSoldQuantity = grp.Sum(item => item.QuantitySold)
                })
                .ToListAsync();


            Console.WriteLine();


            // ***********************************************
            // ***********************************************

            //var numberOfItemsSoldPerProduct = await _context.ProductInvoices.Where(x => x.ProductId != 0).GroupBy(x => x.ProductId).Select(grp => new StockProductItems { ProductItemId = grp.Key, ProductItemSoldQuantity = grp.Sum(item => item.QuantitySold) }).ToListAsync();

            Console.WriteLine();

            return numberOfItemsSoldPerProduct;
        }


        // 3. The number of products sold
        public int GetNumberOfProductsSold()
        {
            // *** Requirement 3 - Start (The number of products sold. ) ***

            //var getProductsItemInStock = _context.Products.Where(x => x.ProductId == id).Select(x => x.ProductsItemInStock).FirstOrDefault();
            //var getCountproductIdInInvoice = _context.ProductInvoices.Where(x => x.ProductId == id).Count();
            //_productReport.NumberOfItemsSoldPerProduct = getProductsItemInStock - getCountproductIdInInvoice;

            // Use the below code !!!
            var numberOfProductsSold = _context.ProductInvoices.Select(x => x.ProductId).Distinct().Count();

            // *** Requirement 3 - End ***

            return numberOfProductsSold;
        }

        public int GetnumberOfProductsInStock()
        {
            // *** Requirement 4 - Start (The number of products in stock) ***

            var numberOfProductsInStock = _context.Products.Select(p => p.ProductId).Count();

            // *** Requirement 4 - End ***

            return numberOfProductsInStock;
        }

        public int GetnumberOfProductsInStockVsSold()
        {
            // *** Requirement 5 - Start (The number of products in stock vs sold) ***

            var numberOfProductsInStock = _context.Products.Select(p => p.ProductId).Count();
            var numberOfProductsSold = _context.ProductInvoices.Select(x => x.ProductId).Distinct().Count();

            var numberOfProductsInStockVsSold = numberOfProductsInStock - numberOfProductsSold;

            // *** Requirement 5 - End ***
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
