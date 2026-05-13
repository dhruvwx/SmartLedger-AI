using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Data.Models
{
    public class Budget
    {  

        // bugget for different categories food budget , travl budget etc


        public int Id { get; set; }



        //max budget ALLOWED
        public decimal MonthMaxAmountLimit { get; set; }


        
        //1 to 12 (Jan to Dec)
        public int Month {  get; set; }



        //2026 2027
        public int Year { get; set; }




        // =======Foreign Key======= which user owns this budget?
        public int UserId { get; set; }
        //the user object 
        public User User { get; set; } = null!;


        // =========Foreign Key======which category is this budget for?     CategoryId = 1 means "Food budget"
        public int CategoryId { get; set; }
        // Navigation the actual Category object
        public Category Category { get; set; } = null!;
    }


}
