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

        private readonly IExpenseRepository expenseRepo;

        public BudgetService(IBudgetRepository budgetRepo , IMapper mapper , IExpenseRepository expenseRepo)
        {
            this.budgetRepo = budgetRepo ;
            this.mapper = mapper ;

            this.expenseRepo = expenseRepo;
        }




        public async Task<BudgetResponseDTO> CreateBudgetAsync(CreateBudgetDTO dto, int userId)
        {
            var budget = mapper.Map<Budget>(dto);
            budget.UserId = userId ;

            var savedBudget = await budgetRepo.CreateBudgetAsync(budget);
            return mapper.Map<BudgetResponseDTO>(savedBudget);
        }






        public async Task<List<BudgetResponseDTO>> GetBudgetsAsync(int userId)
        {
            var budgets = await budgetRepo.GetBudgetsAsync(userId);

            var response = new List<BudgetResponseDTO>();
            
            foreach(var budget in budgets)
            {
                var spentAmount = await expenseRepo.GetAmountSpentBySingleBudgetOfCategory(userId, budget.CategoryId, budget.Month, budget.Year);

                decimal percentageUsed = (spentAmount / budget.MonthMaxAmountLimit) * 100;

                string warning = "";
                if(spentAmount > budget.MonthMaxAmountLimit)
                {
                    warning = "Budget Exceeded";
                }
                else if(percentageUsed >= 80)
                {
                    warning = "80% Of Budget Limit Crossed";
                }

                response.Add(new BudgetResponseDTO
                {
                    MonthMaxAmountLimit = budget.MonthMaxAmountLimit,
                    SpentAmount = spentAmount,
                    RemainingAmount = budget.MonthMaxAmountLimit - spentAmount,
                    IsExceeded = spentAmount > budget.MonthMaxAmountLimit,
                    CategoryName = budget.Category.CategoryName,
                    WarningMessage = warning,
                    BudgetId = budget.Id,
                    Month = budget.Month,
                    Year = budget.Year,
                    CategoryId = budget.CategoryId
                });
            }

            return response;
        }





        public async Task<BudgetResponseDTO?> UpdateBudgetAsync(int userId, int budgetId, UpdateBudgetDTO dto)
        {
            var budget = mapper.Map<Budget>(dto);

            var updatedBudget = await budgetRepo.UpdateBudgetAsync(userId, budgetId, budget);

            if(updatedBudget == null) { return null; }

            return mapper.Map<BudgetResponseDTO>(updatedBudget);    
        }


        public async Task<BudgetResponseDTO?> DeleteBudgetAsync(int userId, int budgetId)
        {
           var deletedBudget = await budgetRepo.DeleteBudgetAsync(userId, budgetId);

            if (deletedBudget == null) { return null; }

            return mapper.Map<BudgetResponseDTO>(deletedBudget);
        }

        

        
    }
}




