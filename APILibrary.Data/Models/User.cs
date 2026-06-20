using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Data.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Email { get; set; }

        //will use BCrypt that stores hashed pw not orignal.
        public string PasswordHash { get; set; } 

        public string FirstName { get; set; }

        public string LastName { get; set; }

        //new user is user by default
        public string Role { get; set; } = "User";

        //Standard time every where so can change to show local on the front end
        public DateTime UserCreatedAt { get; set; } = DateTime.UtcNow;


        //========================================================================================//
        //  NAVIGATION    //ICollection used instead of list , will have list

        
        //user can have multiple expenses
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();

        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();


    }
}
