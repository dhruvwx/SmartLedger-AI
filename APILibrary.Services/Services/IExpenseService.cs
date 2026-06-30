using APILibrary.Services.DTOs.DashBoard;
using APILibrary.Services.DTOs.Expense;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Services
{
    public interface IExpenseService
    {
        Task<ExpenseResponseDTO> CreateExpenseAsync(ExpenseRequestDTO dto, int userId);
        Task<List<ExpenseResponseDTO>> GetAllExpensesAsync(int userId, int? categoryId, int? month, decimal? minAmount, decimal? maxAmount, int pageNo, int pageSize, string? sortBy, string? orderBy);
        Task<ExpenseResponseDTO?> UpdateExpenseAsync(int userId, int expenseId, UpdateExpenseDTO dto);
        Task<ExpenseResponseDTO?> DeleteExpenseAsync(int userId, int expenseId);
        Task<DashboardResponseDTO> GetDashboardAsync(int userId);
        Task<List<ExpenseSummaryDTO>> GetExpenseSummaryAsync(int userId);
    }
}
