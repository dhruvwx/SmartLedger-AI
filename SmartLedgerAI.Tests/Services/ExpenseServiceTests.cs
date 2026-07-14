using APILibrary.Data.Models;
using APILibrary.Services.AI.Repository;
using APILibrary.Services.AI.Services;
using APILibrary.Services.DTOs.Expense;
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
    public class ExpenseServiceTests
    {

        [Fact]
        public async Task CreateExpenseAsync_WithValidInput_ReturnsExpenseWithCategory()
        {
            //arrange
            int userId = 1;
            var expenseRequestDto = new ExpenseRequestDTO
            {
                Description = "Swiggy delivery",
                Amount = 500,
                Date = DateTime.UtcNow
            };

            var mockAiRepo = new Mock<IExpenseCategorizerByAi>();
            mockAiRepo.Setup(ai => ai.CategorizeExpenseAsync(expenseRequestDto.Description)).ReturnsAsync("Food");

            var mockCategoryRepo = new Mock<ICategoryRepository>();

            var foodCategory = new Category { Id = 1, CategoryName = "Food" };

            mockCategoryRepo.Setup(c => c.GetCategoryByNameAsync("Food")).ReturnsAsync(foodCategory);


            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<Expense>(expenseRequestDto))
                      .Returns(new Expense 
                      { 
                          Description = expenseRequestDto.Description,
                          Amount = expenseRequestDto.Amount
                      });


            var mockExpenseRepo = new Mock<IExpenseRepository>();

            var savedExpense = new Expense
            {
                Id = 1,
                Description = expenseRequestDto.Description,
                Amount = expenseRequestDto.Amount,
                IsGstApplicable = false,
                UserId = userId,
                CategoryId = foodCategory.Id
            };
            mockExpenseRepo.Setup(e => e.CreateExpenseAsync(It.IsAny<Expense>()))
                           .ReturnsAsync(savedExpense);

            mockMapper.Setup(m => m.Map<ExpenseResponseDTO>(savedExpense))
                      .Returns(new ExpenseResponseDTO
                      {
                          Id = 1,
                          Description = expenseRequestDto.Description,
                          Amount= expenseRequestDto.Amount,
                          IsGstApplicable = false,
                          CategoryName = foodCategory.CategoryName
                      });



            var expenseService = new ExpenseService
                (mockExpenseRepo.Object,
                mockCategoryRepo.Object,
                mockAiRepo.Object,
                mockMapper.Object
                );


            //act
            var result = await expenseService.CreateExpenseAsync(expenseRequestDto, userId);

            //assert
            Assert.NotNull(result);

            Assert.Equal("Food", result.CategoryName);

            Assert.False(result.IsGstApplicable);

            Assert.Equal(expenseRequestDto.Amount, result.Amount);

            mockAiRepo.Verify(ai => ai.CategorizeExpenseAsync(expenseRequestDto.Description), Times.Once);

            mockExpenseRepo.Verify(e => e.CreateExpenseAsync(It.IsAny<Expense>()), Times.Once);
        }






        [Fact]
        public async Task CreateExpenseAsync_WithBusinessCategory_SetGstTrue()
        {
            int userId = 1;
            var expenseRequestDto = new ExpenseRequestDTO
            {
                Description = "Office Supplies",
                Amount = 12000
            };



            var mockAiRepo = new Mock<IExpenseCategorizerByAi>();
            mockAiRepo.Setup(ai => ai.CategorizeExpenseAsync(expenseRequestDto.Description))
                      .ReturnsAsync("Business");



            var mockCategoryRepo = new Mock<ICategoryRepository>();
            var businessCategory = new Category { Id = 2, CategoryName = "Business" };
            mockCategoryRepo.Setup(c => c.GetCategoryByNameAsync("Business"))
                            .ReturnsAsync(businessCategory);


            var mockExpenseRepo = new Mock<IExpenseRepository>();

            Expense capturedExpense = null;
            mockExpenseRepo.Setup(e => e.CreateExpenseAsync(It.IsAny<Expense>()))
                           .Callback<Expense>(e => capturedExpense = e)
                           .ReturnsAsync(new Expense { Id = 1 , IsGstApplicable = true });


            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<Expense>(expenseRequestDto))
                      .Returns(new Expense());
            mockMapper.Setup(m => m.Map<ExpenseResponseDTO>(It.IsAny<Expense>()))
                      .Returns(new ExpenseResponseDTO { IsGstApplicable = true });


            var expenseService = new ExpenseService
                (mockExpenseRepo.Object,
                mockCategoryRepo.Object,
                mockAiRepo.Object,
                mockMapper.Object
                );

            //act 
            var result = await expenseService.CreateExpenseAsync(expenseRequestDto, userId);

            //assert
            Assert.NotNull(capturedExpense);

            Assert.True(capturedExpense.IsGstApplicable);
        }






        [Fact]
        public async Task CreateExpenseAsync_WhenRepositoryThrows_PropagatesException()
        {
            //arrange
            int userId = 1;
            var expenseRequestDto = new ExpenseRequestDTO
            {
                Description = "Test",
                Amount = 100
            };

        }

    }
}
