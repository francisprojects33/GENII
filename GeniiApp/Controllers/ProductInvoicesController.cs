using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GeniiApp.Data;
using GeniiApp.Models;

namespace GeniiApp.Controllers
{
    public class ProductInvoicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductInvoicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ProductInvoices
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ProductInvoices.Include(p => p.Invoice).Include(p => p.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ProductInvoices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productInvoice = await _context.ProductInvoices
                .Include(p => p.Invoice)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (productInvoice == null)
            {
                return NotFound();
            }

            return View(productInvoice);
        }

        // GET: ProductInvoices/Create
        public IActionResult Create()
        {
            ViewData["InvoiceId"] = new SelectList(_context.Invoices, "InvoiceId", "InvoiceId");
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ItemName");
            return View();
        }

        // POST: ProductInvoices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,InvoiceId")] ProductInvoice productInvoice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productInvoice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InvoiceId"] = new SelectList(_context.Invoices, "InvoiceId", "InvoiceId", productInvoice.InvoiceId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ItemName", productInvoice.ProductId);
            return View(productInvoice);
        }

        // GET: ProductInvoices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productInvoice = await _context.ProductInvoices.FindAsync(id);
            if (productInvoice == null)
            {
                return NotFound();
            }
            ViewData["InvoiceId"] = new SelectList(_context.Invoices, "InvoiceId", "InvoiceId", productInvoice.InvoiceId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ItemName", productInvoice.ProductId);
            return View(productInvoice);
        }

        // POST: ProductInvoices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,InvoiceId")] ProductInvoice productInvoice)
        {
            if (id != productInvoice.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productInvoice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductInvoiceExists(productInvoice.ProductId))
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
            ViewData["InvoiceId"] = new SelectList(_context.Invoices, "InvoiceId", "InvoiceId", productInvoice.InvoiceId);
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ItemName", productInvoice.ProductId);
            return View(productInvoice);
        }

        // GET: ProductInvoices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productInvoice = await _context.ProductInvoices
                .Include(p => p.Invoice)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (productInvoice == null)
            {
                return NotFound();
            }

            return View(productInvoice);
        }

        // POST: ProductInvoices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productInvoice = await _context.ProductInvoices.FindAsync(id);
            _context.ProductInvoices.Remove(productInvoice);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductInvoiceExists(int id)
        {
            return _context.ProductInvoices.Any(e => e.ProductId == id);
        }
    }
}
