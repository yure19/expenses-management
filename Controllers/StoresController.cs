using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ExpensesMgmtWeb.Data;
using ExpensesMgmtWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ExpensesMgmtWeb.Controllers
{
    [Authorize]
    public class StoresController : Controller
    {
        private readonly ExpensesMgmtContext _context;

        public StoresController(ExpensesMgmtContext context)
        {
            _context = context;
        }

        // GET: Stores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Stores.Include(p => p.Purchases).ToListAsync());
        }

        // GET: Stores/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var store = await _context.Stores.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // GET: Stores/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stores/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            Store store = new Store();

            if (! await TryUpdateModelAsync(store, "", s => s.Id, s => s.Name, s => s.Address)) {
                return View(store);
            }

            if (StoreExists(store.Address, store.Name, store.Id))
            {
                ModelState.AddModelError(nameof(store.Address), string.Format("The store '{0}' already exists.", store.Name));

                return View(store);
            }

            _context.Add(store);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Stores/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var store = await _context.Stores.FindAsync(id);

            if (store == null)
            {
                return NotFound();
            }

            return View(store);
        }

        // POST: Stores/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Store store = await _context.Stores.FindAsync(id);
            if (store == null)
            {
                return NotFound();
            }

            if (! await TryUpdateModelAsync(store, "", s => s.Id, s => s.Name, s => s.Address)) {
                return View(store);
            }

            if (StoreExists(store.Address, store.Name, store.Id))
            {
                ModelState.AddModelError("Address", string.Format("The store '{0}' already exists.", store.Name));

                return View(store);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));            
        }

        // POST: Stores/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var store = await _context.Stores.FindAsync(id);

            if (store == null) {
                return NotFound();
            }

            if (store.Purchases.Count > 0) {
                return StatusCode(StatusCodes.Status403Forbidden,
                    "The store has associated purchases, so you can't delete it.");
            }

            _context.Stores.Remove(store);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, HttpGet]
        public IActionResult VerifyStore(string address, string name, int id)
        {
            if (StoreExists(address, name, id))
            {
                return Json(string.Format("The store '{0}' already exists.", name));
            }
            return Json(true);
        }

        private bool StoreExists(string address, string name, int id)
        {
            return _context.Stores.Any(s => s.Id != id && s.Name == name && s.Address == address);
        }
    }
}
