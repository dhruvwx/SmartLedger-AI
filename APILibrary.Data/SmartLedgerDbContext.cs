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


        //Table needs few Default values == it is added by OnCreatingModel(ModelBuilder)

                        //.HasData() == seeds the data as rows in blank columns created by EF CORE
                                    //Id hardcoded coz IDENTITY(auto increment) for seed data = EF tracks rows it seeded so it can update them in future migrations if you change the seed data.

                        //.HasPrecison() == DECIMAL(18,2) --SqlServer 18digits, 2digits after decimal, {it is same as [Column(TypeName = "decimal(18,2)")]} it is also default but explicit is better  ----- Do this for all money columns

                        //.Property() -- used to configure exact column in the table

                        //.Entity<> == configures table

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
 