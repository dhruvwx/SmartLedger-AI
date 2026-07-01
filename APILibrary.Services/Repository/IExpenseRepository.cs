using APILibrary.Data.Models;
using APILibrary.Services.DTOs.DashBoard;
using APILibrary.Services.DTOs.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Repository
{
    public interface IExpenseRepository
    {
        Task<Expense> CreateExpenseAsync(Expense expense);


        // optional category -- filters int? categoryId , int? month , decimal? minAmount , decimal maxAmount || page not nullable coz we'll put default values
        Task<List<Expense>> GetAllExpensesAsync(int userId, int? categoryId, int? month, decimal? minAmount, decimal? maxAmount, int pageNo, int pageSize, string? sortBy, string? sortOrder);


        Task<Expense?> UpdateExpensesAsync(int userId, int expenseId, Expense updatedExpense);

        Task<Expense?> DeleteExpenseAsync(int userId, int expenseId);




        //used in service
        //Task<List<ExpenseSummaryDTO>> GetExpenseSummaryAsync(int userId);


        Task<decimal> GetAmountSpentBySingleBudgetOfCategory (int userId , int categoryId , int month , int year);

        Task<List<Expense>> GetExpensesForDashboardAndSummaryAsync(int userId);



    }
}

