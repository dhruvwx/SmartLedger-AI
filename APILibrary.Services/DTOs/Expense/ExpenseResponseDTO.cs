using APILibrary.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.DTOs.Expense
{
    public class ExpenseResponseDTO
    {
        public int Id { get; set; } //primary key

        public string Description { get; set; }

        public decimal Amount { get; set; }

        //when was expense done
        public DateTime Date { get; set; } = DateTime.UtcNow;


        // Foreign Key: which category? 
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        //true if a business expense else false by default for all the expense
        public bool IsGstApplicable { get; set; } = false;

        //AI fill the category of the expense //is nullable
        public string? AiCategoryExpense { get; set; }

    }
}
