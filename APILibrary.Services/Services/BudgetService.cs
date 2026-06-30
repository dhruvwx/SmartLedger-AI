using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Budget;
using APILibrary.Services.Repository;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Services
{
    public class BudgetService : IBudgetService
    {



        private readonly IBudgetRepository budgetRepo;
        private readonly IMapper mapper;
        public BudgetService(IBudgetRepository budgetRepo , IMapper mapper)
        {
            this.budgetRepo = budgetRepo ;
            this.mapper = mapper ;
        }




        public async Task<BudgetResponseDTO> CreateBudgetAsync(CreateBudgetDTO dto, int userId)
        {
            var budget = mapper.Map<Budget>(dto);
            budget.Id = userId ;

            var savedBudget = await budgetRepo.CreateBudgetAsync(budget);
            return mapper.Map<BudgetResponseDTO>(savedBudget);
        }




        public Task<BudgetResponseDTO> DeleteBudgetAsync(int userId, int budgetId)
        {
            throw new NotImplementedException();
        }

        public Task<List<BudgetResponseDTO>> GetBudgetsAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<BudgetResponseDTO> UpdateResponse(int userId, int budgetId, UpdateBudgetDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}
