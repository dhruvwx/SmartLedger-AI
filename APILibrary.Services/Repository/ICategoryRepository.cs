using APILibrary.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Repository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();


        //this is needed beacuse we want to know what is the name of category , if business we make GST applicable -- expense controller
        Task<Category> GetCategoryByIdAsync(int categoryId);

        Task<Category> GetCategoryByNameAsync(string categoryName);
    }
}
