using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Data.Models
{
    public class Category
    {
        public int Id { get; set; }

        //food , dining cab etc
        public string CategoryName { get; set; }

        //is the category created by user(false) or is automatically seeded(true)
        public bool IsDefaultCategory { get; set; } = false;





        //if IsDefaultCategory is true therefore  automatic seeded hence null
        //if created by used his id shows.
        public int? CreatedByUserId { get; set; }



        // ===========Navigation============ who created this category?(system categories have no creator)
        public User? CreatedByUser { get; set; }



        //================= Navigation===========one Category has MANY Expenses
        // List<Expense> = the actual list of expenses in this category    like in food == pizza burger etc

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();



        // Collection Navigation: one Category has MANY Budgets
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }



}
