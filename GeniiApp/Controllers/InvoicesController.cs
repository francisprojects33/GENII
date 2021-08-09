using GeniiApp.Areas.Identity.Data;
using GeniiApp.Data;
using GeniiApp.Models;
using GeniiApp.StocksRepo;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace GeniiApp.Controllers
{
    public class InvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private readonly IStocks _stocks;
        private readonly AuthDbContext _authDbContext;

        public InvoicesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, IStocks stocks, AuthDbContext authDbContext)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _userManager = userManager;
            _stocks = stocks;
            _authDbContext = authDbContext;
        }

        //[Authorize(Roles = "Manager")]
        public async Task<IActionResult> Index()
        {
            var invoiceCont = await  _context.Invoices.ToListAsync();
            var usersDetail = await _userManager.Users.Select(x => new ApplicationUser { Id = x.Id, FirstName = x.FirstName, SurName = x.SurName }).ToListAsync();

            var invoiceData = from invoice in invoiceCont
                               join
                               usrDtl in usersDetail
                               on invoice.CreateByUser.ToString() equals usrDtl.Id into tempstorage
                               from dx in tempstorage.DefaultIfEmpty()
                               select new InvoiceViewModel
                               {
                                   InvoiceId = invoice.InvoiceId,
                                   CreateByUser = invoice.CreateByUser.ToString(),
                                   CreatedDate = invoice.CreatedDate,
                                   TotalInvoice = invoice.TotalInvoice,
                                   FirstName = (dx != null) ? dx.FirstName : "NULL",
                                   SurName = (dx != null) ? dx.SurName : "NULL"
                               };

            return View(invoiceData);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(m => m.InvoiceId == id);

            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        public IActionResult Create()
        {
            PopulateProductsDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InvoiceId,CreatedDate,CreateByUser,TotalInvoice,ProductInvoices")] Invoice invoice)
        {
            var user = await GetCurrentUserAsync();
            var userId = user?.Id;
            Guid userIdGuid = Guid.Parse(userId);
            string email = user?.Email;

            var invoiceData = new Invoice
            {
                CreatedDate = DateTime.Now,
                CreateByUser = userIdGuid,
                ProductInvoices = invoice.ProductInvoices,
                TotalInvoice = invoice.TotalInvoice
            };

            if (ModelState.IsValid)
            {
                _context.Add(invoiceData);
                await _context.SaveChangesAsync();

                var stockLevelPerItem = await StockLevelForEachItem();
                await _stocks.IsStocksLevelLowAsync(stockLevelPerItem);

                return RedirectToAction(nameof(Index));
            }
            return View(invoiceData);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices.Include("ProductInvoices").Where(i => i.InvoiceId == id).FirstOrDefaultAsync();

            if (invoice == null)
            {
                return NotFound();
            }

            PopulateProductInvoiceItemName(id);
            PopulateProductsDropDownList();
            return View(invoice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InvoiceId,CreatedDate,CreateByUser,TotalInvoice,ProductInvoices")] Invoice invoice)
        {
            if (id != invoice.InvoiceId)
            {
                return NotFound();
            }

            var invoiceStoredToDelete = await _context.ProductInvoices.Where(i => i.InvoiceId == id).Select(x => x.ProductId).ToListAsync();

            foreach (var item in invoiceStoredToDelete)
            {
                var invoiceToDelete = await _context.ProductInvoices.FindAsync(item, id);
                _context.ProductInvoices.Remove(invoiceToDelete);
                await _context.SaveChangesAsync();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvoiceExists(invoice.InvoiceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(invoice);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(m => m.InvoiceId == id);
            if (invoice == null)
            {
                return NotFound();
            }

            return View(invoice);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceId == id);
        }

        private void PopulateProductsDropDownList()
        {
            IEnumerable<SelectListItem> items = _context.Products.Select(c => new SelectListItem
            {
                Value = c.ProductId.ToString() + "-" + c.CostPerItem,
                Text = c.ItemName
            });

            ViewData["Products"] = items;
        }

        public List<string> PopulateProductInvoiceItemName(int? id)
        {
            var invoice = _context.Invoices.Include("ProductInvoices").Where(i => i.InvoiceId == id).FirstOrDefault();
            var ProductInvoiceItemName = invoice.ProductInvoices.Select(p => new { p.ProductId, p.QuantitySold }).ToList();
            var productIds = ProductInvoiceItemName.SelectMany(i => Enumerable.Repeat(i.ProductId, i.QuantitySold)).ToList();
            var ItemFullDetail = productIds
                            .Join(_context.Products,
                            left => left,
                            right => right.ProductId,
                            (left, right) => new { items = right });

            IEnumerable<SelectListItem> items = ItemFullDetail.Select(c => new SelectListItem
            {
                Value = c.items.ProductId.ToString() + "-" + c.items.CostPerItem,
                Text = c.items.ItemName
            });

            ViewData["ProductsPerInvoice"] = items;


            // View Index
            var itemNames = ItemFullDetail.Select(i => i.items.ItemName).ToList();

            return itemNames;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        private async Task<List<StockProductItems>> StockLevelForEachItem()
        {
            var stockLevelForEachItem = await _context.ProductInvoices
                            .Where(x => x.ProductId != 0)
                            .GroupBy(x => new { x.ProductId, x.Product.ItemName, x.Product.ProductsItemInStock })
                            .Select(grp => new StockProductItems
                            {
                                ProductItemId = grp.Key.ProductId,
                                ProductItemName = grp.Key.ItemName,
                                ProductItemInitialQuantity = grp.Key.ProductsItemInStock,
                                ProductItemSoldQuantity = grp.Sum(item => item.QuantitySold)
                            })
                            .ToListAsync();

            return stockLevelForEachItem;
        }

    }

}
