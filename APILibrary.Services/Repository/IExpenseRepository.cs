using APILibrary.Data.Models;
using APILibrary.Services.DTOs.DashBoard;
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

        // optional category -- filters int? categoryId , int? month , decimal? minAmount , decimal maxAmount
        Task<List<Expense>> GetAllExpensesAsync(int userId , int? categoryId , int? month , decimal? minAmount , decimal? maxAmount);
        Task<Expense?> UpdateExpensesAsync(int userId , int expenseId , Expense updatedExpense);

        Task<Expense?> DeleteExpenseAsync(int userId, int expenseId);

        Task<DashboardResponseDTO> GetDashboardAsync(int userId);


    }
}
