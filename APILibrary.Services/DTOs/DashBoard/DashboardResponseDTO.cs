using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.DTOs.DashBoard
{
    public class DashboardResponseDTO
    {

        public decimal TotalSpent{ get; set; }

        // current month spending
        public decimal MonthlySpent{ get; set; }

        // total expense count
        public int TotalExpenses{ get; set; }

        // highest spending category
        public string TopCategory{ get; set; }
    }
}
