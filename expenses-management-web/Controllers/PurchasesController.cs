using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpensesMgmtWeb.Data;
using ExpensesMgmtWeb.Models;
using System;
using Microsoft.AspNetCore.Authorization;
using ExpensesMgmtWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace ExpensesMgmtWeb.Controllers
{
    [Authorize]
    public class PurchasesController : Controller
    {
        private readonly ExpensesMgmtContext _context;
        private readonly UserManager<ExpensesMgmtWebUser> _userManager;

        public PurchasesController(ExpensesMgmtContext context,
            UserManager<ExpensesMgmtWebUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Purchases
        public IActionResult Index()
        {
            IQueryable<dynamic> purchasesWithTotal = _context.Purchases
                                              .Include(p => p.Store)
                                              .Where(p => _userManager.GetUserId(User).Equals(p.UserId))
                                              .Select(p => new
                                              {
                                                  Purchase = p,
                                                  Total = p.PurchasedProducts.Sum(pp => pp.Price * pp.Quantity) + p.Tax
                                              });
            return View(purchasesWithTotal);
        }

        // GET: Purchases/Details/5
        public async Task<IActionResult> Details(int id)
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

        // GET: Purchases/Create
        public IActionResult Create()
        {
            LoadAllStores();

            return View();
        }

        // POST: Purchases/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            Purchase purchase = new Purchase();

            if (!await TryUpdateModelAsync(purchase, "",
                p => p.Name, p => p.Date, p => p.Tax, p => p.StoreId))
            {
                LoadAllStores();
                return View(purchase);
            }

            if (PurchaseExists(purchase.StoreId, purchase.Id, purchase.Name, purchase.Date))
            {
                ModelState.AddModelError(nameof(purchase.StoreId), string.Format("The purchase '{0}' already exists.", purchase.Name));

                LoadAllStores();
                return View(purchase);
            }

            purchase.User = await _userManager.GetUserAsync(User);

            _context.Add(purchase);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), nameof(PurchasedProducts), new { purchase.Id });  
        }

        // GET: Purchases/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null || !_userManager.GetUserId(User).Equals(purchase.UserId))
            {
                return NotFound();
            }

            LoadAllStores();
            return View(purchase);
        }

        // POST: Purchases/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null || !_userManager.GetUserId(User).Equals(purchase.UserId))
            {
                return NotFound();
            }

            if (! await TryUpdateModelAsync(purchase, "", 
                p => p.Name, p => p.Tax, p => p.Date, p => p.StoreId))
            {
                LoadAllStores();
                return View(purchase);
            }

            if (PurchaseExists(purchase.StoreId, purchase.Id, purchase.Name, purchase.Date))
            {
                ModelState.AddModelError(nameof(purchase.StoreId), string.Format("The purchase '{0}' already exists.", purchase.Name));

                LoadAllStores();
                return View(purchase);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), nameof(PurchasedProducts), new { purchase.Id });

        }

        // GET: Purchases/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var purchase = await _context.Purchases.Include(p => p.Store).FirstOrDefaultAsync(m => m.Id == id);

            if (purchase == null || !_userManager.GetUserId(User).Equals(purchase.UserId))
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

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null || !_userManager.GetUserId(User).Equals(purchase.UserId))
            {
                return NotFound();
            }

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Purchases/Confirmation/5
        public async Task<IActionResult> Confirmation(int id)
        {
            var purchase = await _context.Purchases.FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null || !_userManager.GetUserId(User).Equals(purchase.UserId))
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

        // POST: Purchases/Confirmation/5
        [HttpPost, ActionName("Confirmation")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmationConfirmed(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);

            if (purchase == null || !_userManager.GetUserId(User).Equals(purchase.UserId))
            {
                return NotFound();
            }

            if (!purchase.Confirmed)
            {
                purchase.Confirmed = true;
                _context.Update(purchase);
            }

            foreach (var prod in purchase.PurchasedProducts)
            {
                if (!prod.Confirmed) {
                    prod.Confirmed = true;
                    _context.Update(prod);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        //called from Purchase.StoreId method
        [HttpPost, HttpGet]
        public IActionResult VerifyPurchase(int storeId, int id, string name, DateTime date)
        {
            if (!PurchaseExists(storeId, id, name, date)) {
                return Json(true);
            }
            return Json(string.Format("The purchase '{0}' already exists.", name));
        }

        private bool PurchaseExists(int storeId, int Id, string name, DateTime date)
        {
            return _context.Purchases.Any(p => p.Id != Id && p.Name == name && p.Date == date && p.StoreId == storeId);
        }

        private void LoadAllStores() 
        {
            ViewData["StoreId"] = new SelectList((from s in _context.Stores
                                                  select new
                                                  {
                                                      IdDep = s.Id,
                                                      FullName = s.Id + " - " + s.Name
                                                  }),
                                                    "IdDep",
                                                    "FullName");
        }
    }
}
