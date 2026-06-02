using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.DTOs.Budget
{
    public class BudgetResponseDTO
    {
        public decimal MonthMaxAmountLimit { get; set; }

        public decimal SpentAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        public bool IsExceeded { get; set; }

        public string CategoryName { get; set; }

        public string WarningMessage { get; set; }

        public int BudgetId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int CategoryId { get; set; }

    }
};