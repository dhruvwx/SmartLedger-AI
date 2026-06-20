using APILibrary.Data;
using APILibrary.Data.Models;
using APILibrary.Services.DTOs.DashBoard;
using APILibrary.Services.DTOs.Expense;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Repository
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly SmartLedgerDbContext db;
        public ExpenseRepository(SmartLedgerDbContext db)
        {
            this.db = db;
        }
        public async Task<Expense> CreateExpenseAsync(Expense expense)
        {
            await db.Expenses.AddAsync(expense);
            await db.SaveChangesAsync();
            return expense;
        }

        public async Task<Expense> DeleteExpenseAsync(int userId, int expenseId)
        {
            var selectedExpense = await db.Expenses.Include(e => e.Category).FirstOrDefaultAsync(e => e.UserId == userId && e.Id == expenseId);
            if (selectedExpense == null)
            {
                return null;
            }
            else
            {
                db.Expenses.Remove(selectedExpense);
                await db.SaveChangesAsync();
                return selectedExpense;
            }

        }

        // optional category -- filters int? categoryId , int? month , decimal? minAmount , decimal maxAmount

        public async Task<List<Expense>> GetAllExpensesAsync(int userId, int? categoryId, int? month, decimal? minAmount, decimal? maxAmount, int pageNo, int pageSize, string? sortBy, string? sortOrder) 
        {
            var queryable = db.Expenses.Where(e => e.UserId == userId).Include(e => e.Category).AsQueryable();
            //now filter every nullable optional parameter

            if(categoryId != null)
            {
                queryable = queryable.Where(q => q.CategoryId == categoryId);
            }
            if(month != null)
            {
                queryable = queryable.Where(q => q.Date.Month == month);
            }
            if(minAmount != null)
            {
                queryable = queryable.Where(q => q.Amount >= minAmount);
            }
            if(maxAmount != null)
            {
                queryable = queryable.Where(q => q.Amount <= maxAmount);
            }
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if(sortBy.ToLower() == "amount")
                {
                    if (sortOrder?.ToLower() == "desc" || sortOrder?.ToLower() == "descending")
                    {
                          queryable = queryable.OrderByDescending(q => q.Amount);
                    }
                    else
                    {
                        queryable = queryable.OrderBy(q => q.Amount);
                    }
                }
                else if(sortBy.ToLower() == "date")
                {
                    if(sortOrder?.ToLower() == "desc" || sortOrder?.ToLower() == "descending")
                    {
                        queryable = queryable.OrderByDescending(q => q.Date);
                    }
                    else
                    {
                        queryable = queryable.OrderBy(q => q.Date);
                    }
                }
            }

            //Pagination
            var pageResult = (pageNo - 1) * pageSize;

            return await queryable.Skip(pageResult).Take(pageSize).ToListAsync(); // this is when db is hit

        }

        public async Task<DashboardResponseDTO> GetDashboardAsync(int userId)
        {
            var userExpenses = await db.Expenses.Where(e => e.UserId == userId).Include(e => e.Category).ToListAsync();

            var dashboard = new DashboardResponseDTO();

            dashboard.TotalSpent = userExpenses.Sum(e => e.Amount);
            dashboard.TotalExpenses = userExpenses.Count();
            dashboard.MonthlySpent = userExpenses
                .Where(e => e.Date.Month == DateTime.UtcNow.Month)
                .Sum(e => e.Amount);
            dashboard.TopCategory = userExpenses
                .GroupBy(e => e.Category.CategoryName)
                .OrderByDescending(g => g.Sum(e => e.Amount))
                .Select(g => g.Key).FirstOrDefault()?? "NO EXPENSES";

           return dashboard;
        }

        public async Task<Expense?> UpdateExpensesAsync(int userId, int expenseId, Expense updatedExpense)
        {
            var expenseOfUser = await db.Expenses.Include(e => e.Category).FirstOrDefaultAsync(e => e.UserId == userId && e.Id == expenseId);
            if (expenseOfUser == null)
            {
                return null;
            }
            expenseOfUser.CategoryId = updatedExpense.CategoryId;
            expenseOfUser.Date = updatedExpense.Date;
            expenseOfUser.Description = updatedExpense.Description; ;
            expenseOfUser.Amount = updatedExpense.Amount;
            expenseOfUser.IsGstApplicable = updatedExpense.IsGstApplicable;


            var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == updatedExpense.CategoryId);
            if (category.CategoryName == "Business")
            {
                expenseOfUser.IsGstApplicable = true;
            }
            else
            {
                expenseOfUser.IsGstApplicable = false;
            }

            db.SaveChanges();
            return expenseOfUser;
        }

        async Task<List<ExpenseSummaryDTO>> IExpenseRepository.GetExpenseSummaryAsync(int userId)
        {
          var categoryWiseExpenseList = await db.Expenses
                .Where(e => e.UserId == userId)
                .Include(e => e.Category)
                .GroupBy(e => e.Category.CategoryName)
                .Select(g => new ExpenseSummaryDTO
                {
                    CategoryName = g.Key,
                    TotalSpent = g.Sum(e => e.Amount)
                })
                .ToListAsync();

            return categoryWiseExpenseList;
        }
    }
}
