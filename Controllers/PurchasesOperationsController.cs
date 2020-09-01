using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpensesMgmtWeb.Areas.Identity.Data;
using ExpensesMgmtWeb.Data;
using ExpensesMgmtWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExpensesMgmtWeb.Controllers
{
    [Authorize]
    public class PurchasesOperationsController : Controller
    {
        private readonly ExpensesMgmtContext _context;
        private readonly UserManager<ExpensesMgmtWebUser> _userManager;

        public PurchasesOperationsController(ExpensesMgmtContext context,
            UserManager<ExpensesMgmtWebUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: PurchasesOperations/ExpensesAndPurchasesByDate
        public IActionResult ExpensesAndPurchasesByDate()
        {
            return View();
        }

        // POST: PurchasesOperations/ExpensesAndPurchasesByDate
        [HttpPost, ActionName("ExpensesAndPurchasesByDate")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<ChartData>> ExpensesAndPurchasesByDatePost()
        {
            ChartData chartData = new ChartData
            {
                InputDataErrors = new Dictionary<string, List<string>>()
            };

            if (!await TryUpdateModelAsync(chartData, "",
                chart => chart.DateFrom, chart => chart.DateTo))
            {
                foreach (var fieldName in ModelState.Keys)
                {
                    var value = ModelState[fieldName];
                    var errors = value.Errors;
                    if (errors.Count > 0)
                    {
                        List<string> errorMessages = new List<string>();
                        foreach (var error in errors)
                        {
                            errorMessages.Add(error.ErrorMessage);
                        }

                        chartData.InputDataErrors.Add(fieldName, errorMessages);
                    } 
                }
                return chartData;
            }

            var purchasesWithTotal = from purchase in _context.Purchases
                                     where purchase.Date.Date >= chartData.DateFrom &&
                                           purchase.Date.Date <= chartData.DateTo &&
                                           purchase.PurchasedProducts.Count > 0 &&
                                           purchase.UserId.Equals(_userManager.GetUserId(User))
                                     select new
                                     {
                                         purchase.Date.Date,
                                         Total = purchase.PurchasedProducts.Sum(pp => pp.Price * pp.Quantity) + purchase.Tax
                                     };

            chartData.Data = purchasesWithTotal.AsEnumerable().
                GroupBy(a => a.Date).
                Select(g => new
                {
                    Date = g.Key.ToString("MM/dd/yyyy"),
                    Expenses = g.Sum(a => a.Total),
                    Purchases = g.Count()
                }).
                OrderBy(a => a.Date).AsQueryable();

            return chartData;
        }

        //GET: PurchasesOperations/CurrentYearExpensesByMonth
        public IActionResult CurrentYearExpensesByMonth()
        {
            ChartData chartData = new ChartData();

            var purchasesWithTotal = from purchase in _context.Purchases
                                     where purchase.Date.Year == DateTime.Today.Year &&
                                           purchase.PurchasedProducts.Count > 0 &&
                                           purchase.UserId.Equals(_userManager.GetUserId(User))
                                     select new
                                     {
                                         purchase.Date.Month,
                                         Total = purchase.PurchasedProducts.Sum(pp => pp.Price * pp.Quantity) + purchase.Tax
                                     };

            chartData.Data = purchasesWithTotal.AsEnumerable().
                GroupBy(a => a.Month).
                Select(g => new
                {
                    Month = g.Key,
                    Expenses = g.Sum(a => a.Total),
                }).
                OrderBy(a => a.Month).AsQueryable();

            return View(chartData);
        }
    }
}