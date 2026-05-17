using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.DTOs.Category
{
    public class CategoryResponseDTO
    {
        //used for category selection
        public int Id { get; set; }

        public string CategoryName { get; set; }
    }
}
