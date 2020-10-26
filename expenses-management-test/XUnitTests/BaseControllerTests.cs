using ExpensesMgmtWeb.Data;
using ExpensesMgmtWeb.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Abstractions;

namespace expenses_management_test.UnitTests
{
    public class BaseControllerTests
    {
        protected DbContextOptions<ExpensesMgmtContext> ContextOptions { get; }

        protected BaseControllerTests(DbContextOptions<ExpensesMgmtContext> contextOptions) 
        {
            ContextOptions = contextOptions;
            Seed();
        }

        private void Seed() 
        {
            using (var context = new ExpensesMgmtContext(ContextOptions)) {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var store_1 = new Store
                {
                    Id = 1,
                    Name = "Walmart",
                    Address = "8651 NW 13th Terrace, Doral"
                };

                var product_1 = new Product
                {
                    Id = 1,
                    Name = "rice"
                };

                context.AddRange(store_1, product_1);
                context.SaveChanges();

                var purchasedProducts = new List<PurchasedProduct>();
                var purchasedProduct = new PurchasedProduct
                {
                    Id = 1,
                    Name = "rice",
                    Price = 20,
                    Quantity = 2,
                    MeasureUnit = "lb",
                    PurchaseId = 1,
                    ProductId = 1,
                    Confirmed = true
                };
                purchasedProducts.Add(purchasedProduct);

                var purchase_1 = new Purchase
                {
                    Id = 1,
                    Name = "Purch_Oct_1",
                    Date = new DateTime(2020, 10, 26),
                    Tax = 3,
                    Confirmed = true,
                    StoreId = 1,
                    UserId = null,
                    PurchasedProducts = purchasedProducts
                };
                
                context.AddRange(purchase_1);
                context.SaveChanges();
            }
        }
    }
}
