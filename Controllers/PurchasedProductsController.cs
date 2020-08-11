using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpensesMgmtWeb.Data;
using ExpensesMgmtWeb.Models;
using Microsoft.AspNetCore.Authorization;
using ExpensesMgmtWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;

namespace ExpensesMgmtWeb.Controllers
{
    [Authorize]
    public class PurchasedProducts : Controller
    {
        private readonly ExpensesMgmtContext _context;
        private readonly UserManager<ExpensesMgmtWebUser> _userManager;

        public PurchasedProducts(ExpensesMgmtContext context,
            UserManager<ExpensesMgmtWebUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: PurchasedProducts
        public async Task<IActionResult> Index(int id)
        {
            var purchase = await _context.Purchases
                                         .FirstOrDefaultAsync(p => p.Id == id &&
                                         p.UserId.Equals(_userManager.GetUserId(User)));

            if (purchase == null)
            {
                return NotFound();
            }

            var PurchaseWithProducts = new PurchaseWithProducts
            {
                Purchase = purchase,
                PurchasedProducts = purchase.PurchasedProducts
            };

            return View(PurchaseWithProducts);
        }

        // GET: PurchasedProducts/Create
        public async Task<IActionResult> Create(int id)
        {
            Purchase purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null || !_userManager.GetUserId(User).Equals(purchase.UserId))
            {
                return NotFound();
            }

            ViewData["PurchaseId"] = id;

            return View();
        }

        // POST: PurchasedProducts/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(int id)
        {
            var purchase = await _context.Purchases
                                            .FirstOrDefaultAsync(p => p.Id == id &&
                                            p.UserId.Equals(_userManager.GetUserId(User)));

            if (purchase == null)
            {
                return NotFound();
            }

            PurchasedProduct purchProduct = new PurchasedProduct();

            if (!await TryUpdateModelAsync(purchProduct, "",
                pp => pp.Name, pp => pp.Price, pp => pp.MeasureUnit, pp => pp.Quantity, pp => pp.PurchaseId))
            {
                ViewData["PurchaseId"] = id;

                return View(purchProduct);
            }

            if (PurchasedProductExists(purchProduct.Quantity, purchProduct.Id,
                    purchProduct.Name, purchProduct.Price, purchProduct.MeasureUnit, purchProduct.PurchaseId))
            {
                ModelState.AddModelError(nameof(purchProduct.Quantity), string.Format("The product '{0}' already exists.", purchProduct.Name));

                ViewData["PurchaseId"] = purchProduct.PurchaseId;

                return View(purchProduct);
            }

            purchProduct.ProductId = GetProduct(purchProduct.Name).Id;

            _context.Add(purchProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), nameof(PurchasedProducts), new { Id = purchProduct.PurchaseId });
        }

        private Product GetProduct(string name) {
            Product product = _context.Products
                    .FirstOrDefault(m => m.Name == name);

            if (product == null) {
                _context.Add(new Product { Name = name });
                _context.SaveChanges();
            }

            return _context.Products.FirstOrDefault(m => m.Name == name);
        }

        // GET: PurchasedProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchasedProduct = await _context.PurchasedProducts.FindAsync(id);
            if (purchasedProduct == null || !_userManager.GetUserId(User).Equals(purchasedProduct.Purchase.UserId))
            {
                return NotFound();
            }

            ViewData["PurchaseId"] = new SelectList((from p in _context.Purchases
                                                     select new
                                                     {
                                                         IdP = p.Id,
                                                         FullName = p.Id + " - " + p.Name
                                                     }),
                                                  "IdP",
                                                  "FullName");
            ViewData["ProductId"] = purchasedProduct.ProductId;

            return View(purchasedProduct);
        }

        // POST: PurchasedProducts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            PurchasedProduct purchProduct = await _context.PurchasedProducts.FindAsync(id);
            int oldPurchaseId = purchProduct.PurchaseId;

            if (purchProduct == null || !_userManager.GetUserId(User).Equals(purchProduct.Purchase.UserId)) {
                return NotFound();
            }

            if (!await TryUpdateModelAsync(purchProduct, "",
                p => p.Name, p => p.Price, p => p.MeasureUnit, p => p.Quantity,
                p => p.PurchaseId, p => p.ProductId)) {

                ViewData["PurchaseId"] = new SelectList((from p in _context.Purchases
                                                         select new
                                                         {
                                                             IdP = p.Id,
                                                             FullName = p.Id + " - " + p.Name
                                                         }),
                                                  "IdP",
                                                  "FullName");

                ViewData["ProductId"] = purchProduct.ProductId;

                return View(purchProduct);

            }

            if (PurchasedProductExists(purchProduct.Quantity, purchProduct.Id,
                    purchProduct.Name, purchProduct.Price, purchProduct.MeasureUnit, purchProduct.PurchaseId))
            {
                ModelState.AddModelError(nameof(purchProduct.Quantity), string.Format("The product '{0}' already exists.", purchProduct.Name));

                ViewData["PurchaseId"] = new SelectList((from p in _context.Purchases
                                                         select new
                                                         {
                                                             IdP = p.Id,
                                                             FullName = p.Id + " - " + p.Name
                                                         }),
                                              "IdP",
                                              "FullName");
                ViewData["ProductId"] = purchProduct.ProductId;

                return View(purchProduct);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { id = oldPurchaseId });            
        }

        //POST: PurchasedProducts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var purchProduct = await _context.PurchasedProducts.FindAsync(id);

            if (purchProduct == null || !_userManager.GetUserId(User).Equals(purchProduct.Purchase.UserId))
            {
                return NotFound();
            }

            _context.PurchasedProducts.Remove(purchProduct);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new {id = purchProduct.PurchaseId });
        }

        [HttpPost, HttpGet]
        public IActionResult VerifyPurchasedProduct(decimal quantity, int id, string name, decimal price, string measureUnit, int purchaseId) 
        {
            if (PurchasedProductExists(quantity, id, name, price, measureUnit, purchaseId))
            {
                return Json(string.Format("The product '{0}' already exists.", name));
            }
            return Json(true);
        }

        private bool PurchasedProductExists(decimal quantity, int id, string name, decimal price, string measureUnit, int purchaseId) 
        {
            return _context.PurchasedProducts.Any(p => p.Id != id && p.Quantity == quantity && p.Name == name && p.Price == price && p.MeasureUnit == measureUnit && p.PurchaseId == purchaseId);
        }
    }
}
