using APILibrary.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Data
{
    public class SmartLedgerDbContext : DbContext
    {
        public SmartLedgerDbContext(DbContextOptions<SmartLedgerDbContext> options) : base(options)
        {

        }
     

        //DbSet are the tables
        public DbSet<User> Users { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<Invoice> Invoices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData
                (
                    new Category { Id = 1, CategoryName = "Food/Dining", IsDefaultCategory = true },
                    new Category { Id = 2, CategoryName = "Travel", IsDefaultCategory = true },
                    new Category { Id = 3, CategoryName = "Utilities", IsDefaultCategory = true },
                    new Category { Id = 4, CategoryName = "Entertainment", IsDefaultCategory = true },
                    new Category { Id = 5, CategoryName = "Business", IsDefaultCategory = true },
                    new Category { Id = 6, CategoryName = "Shopping", IsDefaultCategory = true },
                    new Category { Id = 7, CategoryName = "HealthCare", IsDefaultCategory = true }
                );

            modelBuilder.Entity<Expense>().Property(e => e.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<Budget>().Property(b => b.MonthMaxAmountLimit).HasPrecision(18, 2);

            modelBuilder.Entity<Invoice>().Property(i => i.AmountBeforeTax).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.GSTAmount).HasPrecision(18, 2);
            modelBuilder.Entity<Invoice>().Property(i => i.TotalAmount).HasPrecision(18, 2);
        }
    }
}
 