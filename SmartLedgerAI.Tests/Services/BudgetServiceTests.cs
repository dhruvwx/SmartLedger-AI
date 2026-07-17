using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Budget;
using APILibrary.Services.Repository;
using APILibrary.Services.Services;
using AutoMapper;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartLedgerAI.Tests.Services
{
    public class BudgetServiceTests
    {
        [Fact]
        public async Task GetBudgetsAsync_When80PercentSpent_ShowsWarning()
        {
            //arrange
            int userId = 1; 

            var budget = new Budget
            {
                Id = 1,
                MonthMaxAmountLimit = 100,
                Month = 6,
                Year = 2026, 
                UserId = userId,
                CategoryId = 1,
                Category = new Category { Id = 1, CategoryName = "Food" }
            };


            var mockBudgetRepo = new Mock<IBudgetRepository>();
            mockBudgetRepo.Setup(b => b.GetBudgetsAsync(userId))
                          .ReturnsAsync(new List<Budget> { budget });

            var mockExpenseRepo = new Mock<IExpenseRepository>();
            mockExpenseRepo.Setup(e => e.GetAmountSpentBySingleBudgetOfCategory(userId, budget.CategoryId, budget.Month, budget.Year)).ReturnsAsync(80);

            var mockMapper = new Mock<IMapper>();

            var budgetService = new BudgetService
                (
                mockBudgetRepo.Object,
                mockMapper.Object,
                mockExpenseRepo.Object
                );


            //act
            var result = await budgetService.GetBudgetsAsync(userId);

            //assert
            Assert.Equal("80% Of Budget Limit Crossed" , result[0].WarningMessage );

            Assert.False(result[0].IsExceeded);
        }




        [Theory]
        [InlineData(50, false, "")]
        [InlineData(80, false, "80% Of Budget Limit Crossed")]
        [InlineData(150, true, "Budget Exceeded")]
        public async Task GetBudgetAsync_WithDifferentPercentage_ShowsCorrectWarnings(decimal percentageSpent, bool IsExceeded, string warning)
        {
            int userId = 1;
            var budget = new Budget
            {
                Id = 1,
                MonthMaxAmountLimit = 1000,
                Month = 12,
                Year = 2026,
                UserId = userId,
                CategoryId = 1,
                Category = new Category { Id = 1, CategoryName = "Travel" }
            };


            var amountSpent = (budget.MonthMaxAmountLimit * percentageSpent) / 100;

            var mockBudgetRepo = new Mock<IBudgetRepository>();
            mockBudgetRepo.Setup(b => b.GetBudgetsAsync(userId))
                          .ReturnsAsync(new List<Budget> { budget });

            var mockExpenseRepo = new Mock<IExpenseRepository>();
            mockExpenseRepo.Setup(e => e.GetAmountSpentBySingleBudgetOfCategory(userId, budget.CategoryId, budget.Month, budget.Year)).ReturnsAsync(amountSpent);

            var mockMapper = new Mock<IMapper>();

            var budgetService = new BudgetService
                (mockBudgetRepo.Object,
                 mockMapper.Object,
                 mockExpenseRepo.Object
                );



            //ACT 
            var result = await budgetService.GetBudgetsAsync(userId);


            //Assert
            Assert.Equal(warning, result[0].WarningMessage);
            Assert.Equal(IsExceeded, result[0].IsExceeded);
         

        }
    }
}
