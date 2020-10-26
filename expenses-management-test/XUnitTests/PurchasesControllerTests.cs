using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using ExpensesMgmtWeb.Data;
using ExpensesMgmtWeb.Controllers;
using Microsoft.AspNetCore.Identity;
using Moq;
using ExpensesMgmtWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Xunit.Abstractions;
using System.Linq;
using System.Collections.Generic;
using ExpensesMgmtWeb.Models;

namespace expenses_management_test.UnitTests
{
    public class PurchasesControllerTests : BaseControllerTests
    {
        private readonly ITestOutputHelper output;

        public PurchasesControllerTests(ITestOutputHelper output)
            : base(
                new DbContextOptionsBuilder<ExpensesMgmtContext>()
                  .UseSqlite("Filename=Test.db")
                  .Options)
        {
            this.output = output;
        }

        [Fact]
        public void PurchaseDetailsTest()
        {
            var store = new Mock<IUserStore<ExpensesMgmtWebUser>>();

            var mockUser = new Mock<UserManager<ExpensesMgmtWebUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            using (var context = new ExpensesMgmtContext(ContextOptions))
            {
                var controller = new PurchasesController(context, mockUser.Object);

                //verifications
                var viewResult = Assert.IsType<ViewResult>((ViewResult) controller.Details(1).Result);
                var purchaseWithProduct = Assert.IsType<PurchaseWithProducts>(viewResult.Model);
                Assert.Equal(1, purchaseWithProduct.Purchase.Id);

                output.WriteLine("Verified: \n\treturnedType = ViewResult, \n\tmodelType = PurchaseWithProducts, \n\tPurchase.Id = 1)");
            }
        }
    }
}
