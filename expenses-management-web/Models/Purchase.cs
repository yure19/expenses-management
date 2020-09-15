using ExpensesMgmtWeb.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpensesMgmtWeb.Models
{
    public class Purchase
    {
        private ILazyLoader LazyLoader { get; set; }
        private ICollection<PurchasedProduct> _PurchasedProducts;
        private Store _Store;

        public int Id { get; set; }

        [Required(ErrorMessage = "Must enter the name.")]
        [RegularExpression("^[a-zA-Z0-9].*$", ErrorMessage = "The {0} is invalid.")]
        public string Name { get; set; }

        [DataType(DataType.DateTime)]
        [Required(ErrorMessage = "Must enter the date.")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy HH:mm:ss}")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Must enter the tax.")]
        public decimal Tax { get; set; }

        [Display(Name = "Store")]
        [Remote(action: "VerifyPurchase", controller: "Purchases", AdditionalFields = (nameof(Id) + "," + nameof(Name) + "," + nameof(Date)))]
        public int StoreId { get; set; }

        public bool Confirmed { get; set; }

        [HiddenInput]
        public string UserId { get; set; }

        public ICollection<PurchasedProduct> PurchasedProducts {
            get => LazyLoader.Load(this, ref _PurchasedProducts);
            set => _PurchasedProducts = value;
        }

        public Store Store {
            get => LazyLoader.Load(this, ref _Store);
            set => _Store = value;
        }

        public ExpensesMgmtWebUser User { get; set; }
    }
}
