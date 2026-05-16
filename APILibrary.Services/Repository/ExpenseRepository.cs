using APILibrary.Data;
using APILibrary.Data.Models;
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

        public async Task<List<Expense>> GetAllExpensesAsync(int userId)
        {
            var userExpenses = await db.Expenses.Where(e => e.UserId == userId).Include(e => e.Category).ToListAsync();
            return userExpenses;

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

            db.SaveChanges();
            return expenseOfUser;
        }
    }
}
