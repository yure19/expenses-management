using ExpensesMgmtWeb.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ExpensesMgmtWeb.Areas.Identity.Data
{
    public class ExpensesMgmtWebUser : IdentityUser
    {
        [PersonalData]
        public string FirstName { get; set; }

        [PersonalData]
        public string LastName { get; set; }

        public ICollection<Purchase> Purchases { get; set; }
    }
}
