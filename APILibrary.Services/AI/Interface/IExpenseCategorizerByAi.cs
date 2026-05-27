using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.AI.Interface
{
    public interface IExpenseCategorizerByAi
    {
        // ai returns that we can map to our categories , takes in description of the expense
        Task<string> CategorizeExpenseAsync(string description);
    }
}
