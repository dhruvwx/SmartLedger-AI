using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Data.Models
{
    public class Expense
    {
        public int Id { get; set; } //primary key

        // describe the type of expense (the category)
        public string Description { get; set; }

        public decimal Amount { get; set; }

        //when was expense done
        public DateTime Date { get; set; } = DateTime.UtcNow;

        //true if a business expense else false by default for all the expense
        public bool IsGstApplicable { get; set; } = false;


        //AI fill the category of the expense //is nullable
        public string? AiCategoryExpense { get; set; }
         

        // Foreign Key which user created this expense?
        public int UserId { get; set; }

        // Foreign Key: which category?
        public int CategoryId { get; set; }


        // ============Navigation========== the actual User object AND Category object
        public Category Category { get; set; } = null!;
        public User User { get; set; } = null!;

    }
}
