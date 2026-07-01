using APILibrary.Data;
using APILibrary.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly SmartLedgerDbContext db;
        public CategoryRepository(SmartLedgerDbContext db)
        {
            this.db = db;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            return await db.Categories.ToListAsync();
        }



        /*================================================================================================
        |                                                                                                 |
        |  ///=== THE FOLLOWING CODE ARE USED INTERNALLY ONLY SO NO NEED TO PUT IN SERVICE LAYER===///    |
        |                                                                                                 |
        |================================================================================================*/ 

        //this is needed beacuse we want to know what is the name of category , if business we make GST applicable -- use in expense controller
        public async Task<Category> GetCategoryByIdAsync(int categoryId)
        {
            return await db.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

        }

        //used in expense controller
        public async Task<Category> GetCategoryByNameAsync(string categoryName)
        {
            return await db.Categories.FirstOrDefaultAsync(c => c.CategoryName == categoryName);
        }
    }
}
