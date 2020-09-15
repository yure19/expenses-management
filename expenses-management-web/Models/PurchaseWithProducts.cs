using System.Collections.Generic;

namespace ExpensesMgmtWeb.Models
{
    public class PurchaseWithProducts
    {
        public Purchase Purchase { get; set; }
        public ICollection<PurchasedProduct> PurchasedProducts {get; set;}
    }
}
