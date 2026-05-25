using APILibrary.Data;
using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Budget;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Repository
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly SmartLedgerDbContext db;

        public BudgetRepository(SmartLedgerDbContext db)
        {
            this.db = db;
        }

        public async Task<Budget> CreateBudgetAsync(Budget budget)
        {
            await db.Budgets.AddAsync(budget);
            await db.SaveChangesAsync();
            return budget;
        }

        public async Task<Budget?> DeleteBudgetAsync(int userId, int budgetId)
        {
           var budgetToDelete = await db.Budgets.FirstOrDefaultAsync(b => b.UserId == userId && b.Id == budgetId);
            if (budgetToDelete == null)
            {
                return null;
            }
            db.Budgets.Remove(budgetToDelete);
            await db.SaveChangesAsync();
            return budgetToDelete;
        }

        public async Task<List<BudgetResponseDTO>> GetBudgetsAsync(int userId)
        {
            var budgets = await db.Budgets
                 .Where(b => b.UserId == userId)
                 .Include(b => b.Category)
                 .ToListAsync();

            var response = new List<BudgetResponseDTO>();

            foreach (var b in budgets)
            {
                var spentAmountForSingleBudget = await db.Expenses
                                                    .Where(e => e.UserId == userId &&
                                                    e.CategoryId == b.CategoryId &&
                                                    e.Date.Month == b.Month &&
                                                    e.Date.Year == b.Year).SumAsync(e => e.Amount);

                string warning = "";
                decimal percentageUsed = (spentAmountForSingleBudget / b.MonthMaxAmountLimit) * 100;
                if(spentAmountForSingleBudget > b.MonthMaxAmountLimit)
                {
                    warning = "Budget Exceeded";
                }else if(percentageUsed >= 80)
                {
                    warning = "80% of Budget Used";
                }

                    response.Add(new BudgetResponseDTO
                    {
                        MonthMaxAmountLimit = b.MonthMaxAmountLimit,
                        SpentAmount = spentAmountForSingleBudget,
                        RemainingAmount = b.MonthMaxAmountLimit - spentAmountForSingleBudget,
                        IsExceeded = spentAmountForSingleBudget > b.MonthMaxAmountLimit,
                        CategoryName = b.Category.CategoryName,
                        WarningMessage = warning

                    });
            }
            return response;
        }

        public async Task<Budget?> UpdateBudgetAsync(int userId, int budgetId, Budget budget)
        {
            var budgetToUpdate = await db.Budgets.FirstOrDefaultAsync(b => b.UserId == userId && b.Id == budgetId);
            if (budgetToUpdate == null)
            {
                return null;
            }
            budgetToUpdate.Year = budget.Year;
            budgetToUpdate.Month = budget.Month;
            budgetToUpdate.MonthMaxAmountLimit = budget.MonthMaxAmountLimit;
            budgetToUpdate.CategoryId = budget.CategoryId;

            await db.SaveChangesAsync();
            return budgetToUpdate;
        }
    }
}
