using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.DTOs.Expense
{
    public class ExpenseRequestDTO
    {
        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0.01,9999999, ErrorMessage = "Amout should be greater than 0")]
        public decimal Amount { get; set; }


        //when was expense done
        public DateTime Date { get; set; } = DateTime.UtcNow;




        ///REMOVED COZ NOW AI PUTS THE CATEGORY ID
        //// Foreign Key: which category?
        //[Required]
        //public int CategoryId { get; set; }

    }
}
