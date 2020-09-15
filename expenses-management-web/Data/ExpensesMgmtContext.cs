using ExpensesMgmtWeb.Areas.Identity.Data;
using ExpensesMgmtWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ExpensesMgmtWeb.Data
{
    public class ExpensesMgmtContext : IdentityDbContext<ExpensesMgmtWebUser>
    {
        public ExpensesMgmtContext(DbContextOptions<ExpensesMgmtContext> options)
            : base(options)
        {
            
        }

        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<PurchasedProduct> PurchasedProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Purchase>().
            HasOne(s => s.Store).
            WithMany(p => p.Purchases).
            OnDelete(DeleteBehavior.Cascade);
        }
    }
}
