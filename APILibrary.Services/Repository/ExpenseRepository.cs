using APILibrary.Data;
using APILibrary.Data.Models;
using APILibrary.Services.DTOs.DashBoard;
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

        public async Task<List<Expense>> GetAllExpensesAsync(int userId, int? categoryId, int? month, decimal? minAmount, decimal? maxAmount)
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

            return await queryable.ToListAsync(); // this is when db is hit

        }

        public async Task<DashboardResponseDTO> GetDashboardAsync(int userId)
        {
            var userExpenses = await db.Expenses.Where(e => e.UserId == userId).Include(e => e.Category).ToListAsync();

            var dashboard = new DashboardResponseDTO();

            dashboard.TotalSpent = userExpenses.Sum(e => e.Amount);
            dashboard.MonthlySpent = userExpenses.Where(e => e.Date.Month == DateTime.UtcNow.Month).Sum(e => e.Amount);
            dashboard.TotalExpenses = userExpenses.Count();
            dashboard.TopCategory = userExpenses.GroupBy(e => e.Category.CategoryName).OrderByDescending(g => g.Sum(e => e.Amount)
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


    }
}
