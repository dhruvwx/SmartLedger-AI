using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.DTOs.Expense
{
    public class ExpenseSummaryDTO
    { 
        //category analysis
        public string CategoryName { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
