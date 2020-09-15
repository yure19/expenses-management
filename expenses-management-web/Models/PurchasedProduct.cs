using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace ExpensesMgmtWeb.Models
{
    public class PurchasedProduct
    {
        private ILazyLoader LazyLoader { get; set; }
        private Purchase _purchase;

        public int Id { get; set; }

        [Required(ErrorMessage = "Must enter the {0}.")]
        [RegularExpression("^[a-zA-Z0-9].*$", ErrorMessage = "The {0} is invalid.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Must enter the {0}.")]
        public decimal Price { get; set; }

        [Display(Name = "Measure Unit")]
        [Required(ErrorMessage = "Must enter the {0}.")]
        public string MeasureUnit { get; set; }

        [Required(ErrorMessage = "Must enter the {0}.")]
        [Remote(action: "VerifyPurchasedProduct", controller: "PurchasedProducts", AdditionalFields = (nameof(Id) + "," + nameof(Name) + "," + nameof(Price) + "," + nameof(MeasureUnit) + "," + nameof(PurchaseId)))]
        public decimal Quantity { get; set; }

        [HiddenInput]
        [Display(Name = "Purchase Id")]
        public int PurchaseId { get; set; }

        [HiddenInput]
        public int ProductId { get; set; }

        public bool Confirmed { get; set; }

        public Purchase Purchase {
            get => LazyLoader.Load(this, ref _purchase);
            set => _purchase = value;
        }

        public virtual Product Product { get; set; }
    }
}
