using APILibrary.Data;
using APILibrary.Data.Models;
using APILibrary.Services.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedgerAI.Tests.Repository
{
    public class BudgetRepositoryTests
    {
        private SmartLedgerDbContext GetInMemoryContext()
        {
            var contextBuilder = new DbContextOptionsBuilder<SmartLedgerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SmartLedgerDbContext(contextBuilder);
        }

        [Fact]
        public async Task GetBudgetsAsync_TwoUsers_ReturnsOnlyRequestedUserData()
        {
            using var context = GetInMemoryContext();
            var repo = new BudgetRepository(context);

            var Budget1 = new Budget
            {
                MonthMaxAmountLimit = 2000,
                UserId = 1,
                Year = 2026,
                CategoryId = 1,
                Category = new Category { CategoryName = "Food" }

            };
            var Budget2 = new Budget
            {
                MonthMaxAmountLimit = 3000,
                UserId = 2,
                Year = 2026,
                CategoryId = 1,
                Category = new Category { CategoryName = "Food"}
            };

            context.Budgets.AddRange(Budget1 ,  Budget2);
            await context.SaveChangesAsync();



            //act
            var result1 = await repo.GetBudgetsAsync(1);
            var result2 = await repo.GetBudgetsAsync(2);


            //assert
            Assert.Equal(2000, result1[0].MonthMaxAmountLimit);
            Assert.Equal(3000, result2[0].MonthMaxAmountLimit);

            Assert.Single(result1);
            Assert.Single(result2);
            
        }
    }
}
