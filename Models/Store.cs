using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpensesMgmtWeb.Models
{
    public class Store
    {
        private ICollection<Purchase> _Purchases;
        private ILazyLoader LazyLoader { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Must enter the {0}.")]
        [RegularExpression("^[a-zA-Z0-9].*$", ErrorMessage = "The {0} is invalid.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Must enter the {0}.")]
        [RegularExpression("^[a-zA-Z0-9].*$", ErrorMessage = "The {0} is invalid.")]
        [Remote(action: "VerifyStore", controller: "Stores", AdditionalFields = nameof(Name) + "," + nameof(Id))]
        public string Address { get; set; }

        public ICollection<Purchase> Purchases {
            get => LazyLoader.Load(this, ref _Purchases);
            set => _Purchases = value;
        }
    }
}
