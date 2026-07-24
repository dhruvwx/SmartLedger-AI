using APILibrary.Data;
using APILibrary.Data.Models;
using APILibrary.Services.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedgerAI.Tests.Repository
{
    public class ExpenseRepositoryTests
    {
        private SmartLedgerDbContext GetInMemoryDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<SmartLedgerDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new SmartLedgerDbContext(optionsBuilder);
        }



        [Fact]
        public async Task CreateExpenseAsync_ValidExpense_SavesAndReturnsIdCreatedAsPrimaryKeyByEfCore()
        {
            //assert 
            using var context = GetInMemoryDbContext();

            //using disposes after test , here create=ing real repo on fake database
            var repo = new ExpenseRepository(context);


            var expense = new Expense
            {
                //ID = database will create , thats what we testing
                Description = "Test Expense",
                Amount = 100,
                IsGstApplicable = false,
                UserId = 1,
                CategoryId = 1
            };


            //act
            var result = await repo.CreateExpenseAsync(expense);

            //assert
            Assert.True(result.Id > 0);

            var checkSavedExpense = await context.Expenses.FirstOrDefaultAsync(e => e.Id ==  result.  Id);

            Assert.NotNull(checkSavedExpense);
            Assert.Equal(100, result.Amount);
        }





        [Fact]
        public async Task GetAmountSpentBySingleBudgetOfCategory_ReturnsCorrectSum()
        {
            using var context = GetInMemoryDbContext();
            var repo = new ExpenseRepository(context);

            var expense1 = new Expense
            {
                Amount = 100,
                UserId = 1,
                CategoryId = 1,
                Date = new DateTime(2026, 6, 22),
                Description = "Test"
            };
            var expense2 = new Expense
            {
                Amount = 50,
                UserId = 1,
                CategoryId = 1,
                Date = new DateTime(2026, 6, 21),
                Description = "Test"
            };
            var expense3 = new Expense
            {
                Amount = 177,
                UserId = 1,
                CategoryId = 2,
                Date = new DateTime(2026, 6, 23),
                Description = "Test"
            };
            var expense4 = new Expense
            {
                Amount = 100,
                UserId = 2,
                CategoryId = 3,
                Date = new DateTime(2026, 6, 22),
                Description = "Test"
            };
            var expense5 = new Expense
            {
                Amount = 100,
                UserId = 2,
                CategoryId = 1,
                Date = new DateTime(2026, 6, 22),
                Description = "Test"
            };
            
            context.Expenses.AddRange(expense1, expense2, expense3, expense4, expense5);
            await context.SaveChangesAsync();

            //act
            var result1 = await repo.GetAmountSpentBySingleBudgetOfCategory(2, 1, 6, 2026);
            var result2 = await repo.GetAmountSpentBySingleBudgetOfCategory(2, 3, 6, 2026);
            var result3 = await repo.GetAmountSpentBySingleBudgetOfCategory(1, 1, 6, 2026);
            var result4 = await repo.GetAmountSpentBySingleBudgetOfCategory(1, 2, 6, 2026);
            var result5 = await repo.GetAmountSpentBySingleBudgetOfCategory(1, 3, 6, 2026);
            var result6 = await repo.GetAmountSpentBySingleBudgetOfCategory(2, 2, 6, 2026);


            //assert
            Assert.Equal(100, result1);
            Assert.Equal(100, result2);
            Assert.Equal(150, result3);
            Assert.Equal(177, result4);
            Assert.Equal(0, result5);
            Assert.Equal(0, result6);
        }


    }
} 
