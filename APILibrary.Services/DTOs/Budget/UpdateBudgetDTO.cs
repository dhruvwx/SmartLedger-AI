using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.DTOs.Budget
{
    public class UpdateBudgetDTO
    {
        [Required]
        [Range(1, 9999999)]
        public decimal MonthMaxAmountLimit { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
