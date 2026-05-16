using APILibrary.Data.Models;
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
        Task<List<Expense>> GetAllExpensesAsync(int userId);
        Task<Expense?> UpdateExpensesAsync(int userId , int expenseId , Expense updatedExpense);


    }
}
