using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Budget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Repository
{
    public interface IBudgetRepository
    {
        Task<Budget> CreateBudgetAsync(Budget budget);
        Task<List<Budget>> GetBudgetsAsync(int userId);

        Task<Budget?> UpdateBudgetAsync(int userId, int budgetId, Budget budget);
        Task<Budget?> DeleteBudgetAsync(int userId, int budgetId);
    }
}
