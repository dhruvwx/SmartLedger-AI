using APILibrary.Services.DTOs.Budget;
using APILibrary.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Services
{
    public interface IBudgetService
    {
        Task<BudgetResponseDTO> CreateBudgetAsync(CreateBudgetDTO dto, int userId);
        Task<List<BudgetResponseDTO>> GetBudgetsAsync(int userId);
        Task<BudgetResponseDTO> UpdateResponse(int userId , int  budgetId , UpdateBudgetDTO dto);
        Task<BudgetResponseDTO> DeleteBudgetAsync(int userId , int budgetId);
    }
}
