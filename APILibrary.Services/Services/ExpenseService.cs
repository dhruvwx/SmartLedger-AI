using APILibrary.Data.Models;
using APILibrary.Services.AI.Repository;
using APILibrary.Services.DTOs.DashBoard;
using APILibrary.Services.DTOs.Expense;
using APILibrary.Services.Repository;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Services
{
    public class ExpenseService : IExpenseService
    {

        private readonly IExpenseRepository expenseRepo;
        private readonly ICategoryRepository categoryRepo;
        private readonly IExpenseCategorizerByAi ai;
        private readonly IMapper mapper;

        public ExpenseService(IExpenseRepository expenseRepo, ICategoryRepository categoryRepo, IExpenseCategorizerByAi ai, IMapper mapper)
        {
            this.expenseRepo = expenseRepo;
            this.categoryRepo = categoryRepo;
            this.ai = ai;
            this.mapper = mapper;
        }





        public async Task<ExpenseResponseDTO> CreateExpenseAsync(ExpenseRequestDTO dto, int userId)
        {
            var categoryNameByAi = await ai.CategorizeExpenseAsync(dto.Description);

            var category = await categoryRepo.GetCategoryByNameAsync(categoryNameByAi);

            var expense = mapper.Map<Expense>(dto);
            expense.CategoryId = category.Id;
            expense.UserId = userId;
            if(category.CategoryName.Trim() == "Business")
            {
                expense.IsGstApplicable = true;
            }
            else { expense.IsGstApplicable = false; }
            var savedExpense = await expenseRepo.CreateExpenseAsync(expense);

            return mapper.Map<ExpenseResponseDTO>(savedExpense);
        }



        


        public async Task<List<ExpenseResponseDTO>> GetAllExpensesAsync(int userId, int? categoryId, int? month, decimal? minAmount, decimal? maxAmount, int pageNo, int pageSize, string? sortBy, string? orderBy)
        {
            var expenses = await expenseRepo.GetAllExpensesAsync(userId, categoryId, month, minAmount, maxAmount, pageNo, pageSize, sortBy, orderBy);
            return mapper.Map<List<ExpenseResponseDTO>>(expenses);
        }






        public async Task<ExpenseResponseDTO?> UpdateExpenseAsync(int userId, int expenseId, UpdateExpenseDTO dto)
        {
            var categoryName = await ai.CategorizeExpenseAsync(dto.Description);
            var category = await categoryRepo.GetCategoryByNameAsync(categoryName);

            var expense = mapper.Map<Expense>(dto);
            expense.CategoryId = category.Id;

            if(category.CategoryName.Trim() == "Business")
            {
                expense.IsGstApplicable = true;
            }
            else { expense.IsGstApplicable = false; }

            var updatedExpense = await expenseRepo.UpdateExpensesAsync(userId, expenseId, expense);

            if(updatedExpense == null) { return null; }

            return mapper.Map<ExpenseResponseDTO>(updatedExpense);

        }





        public async Task<ExpenseResponseDTO?> DeleteExpenseAsync(int userId, int expenseId)
        {
            var deletedExpense = await expenseRepo.DeleteExpenseAsync(userId, expenseId);
            if (deletedExpense == null) { return null; }

            return mapper.Map<ExpenseResponseDTO>(deletedExpense);
        }





        public async Task<DashboardResponseDTO> GetDashboardAsync(int userId)
        {
            var expenses = await expenseRepo.GetExpensesForDashboardAndSummaryAsync(userId);

            var dashboard = new DashboardResponseDTO();

            dashboard.TotalSpent = expenses.Sum(e => e.Amount);

            dashboard.MonthlySpent = expenses.Where(e => e.Date.Month == DateTime.UtcNow.Month).Sum(e => e.Amount);

            dashboard.TotalExpenses = expenses.Count();

            dashboard.TopCategory = expenses.GroupBy(e => e.Category.CategoryName)
                                            .OrderByDescending(e => e.Sum(e => e.Amount))
                                            .Select(g => g.Key).FirstOrDefault() ?? "No Expenses";
            return dashboard;
        }





        public async Task<List<ExpenseSummaryDTO>> GetExpenseSummaryAsync(int userId)
        {
            var expenses = await expenseRepo.GetExpensesForDashboardAndSummaryAsync(userId);

            var summary = expenses.GroupBy(e => e.Category.CategoryName)
                                  .Select(g => new ExpenseSummaryDTO
                                  {
                                      CategoryName = g.Key,
                                      TotalSpent = g.Sum(e => e.Amount)
                                  }).ToList();

            return summary;
        }

        
    }
}
