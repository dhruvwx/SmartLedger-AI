using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Expense;
using APILibrary.Services.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SmartLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseRepository expenseRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IMapper mapper;

        public ExpenseController(IExpenseRepository expenseRepository , IMapper mapper , ICategoryRepository categoryRepository)
        {
            this.expenseRepository = expenseRepository;
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }


        [HttpPost]
        public async Task<IActionResult> CreateExpense(ExpenseRequestDTO dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }
           
            var expenseModel = mapper.Map<Expense>(dto);

            expenseModel.UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var category = await categoryRepository.GetCategoryById(expenseModel.CategoryId);

            Console.WriteLine(category.CategoryName);
            if (category.CategoryName.Trim() == "Business")
            {
                expenseModel.IsGstApplicable = true;
                Console.WriteLine(expenseModel.IsGstApplicable);
            }
            else
            {
                expenseModel.IsGstApplicable = false;
            }

            expenseModel = await expenseRepository.CreateExpenseAsync(expenseModel);


            var output = mapper.Map<ExpenseResponseDTO>(expenseModel);
            return Ok(output);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllExpenses(int? categoryId, int? month, decimal? minAmount, decimal? maxAmount)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var userExpense = await expenseRepository.GetAllExpensesAsync(userId, categoryId, month, minAmount, maxAmount);

            var userExpensesDTO = mapper.Map<List<ExpenseResponseDTO>>(userExpense);

            return Ok(userExpensesDTO);
        }


        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateExpense(int id , UpdateExpenseDTO dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!); 

            var expenseModel = mapper.Map<Expense>(dto);
           
            var updatedValues = await expenseRepository.UpdateExpensesAsync(userId, id, expenseModel);

            if(updatedValues == null)
            {
                return BadRequest("USER OR EXPENSE DONT EXIST");
            }

            return Ok(mapper.Map<ExpenseResponseDTO>(updatedValues));


        }


        [HttpDelete]
        [Route("{expenseId:int}")]
        public async Task<IActionResult> DeleteExpense(int expenseId)
        {
           var userId =  int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var deletedExpense = await expenseRepository.DeleteExpenseAsync(userId , expenseId);
            if (deletedExpense == null)
            {
                return BadRequest("expense does no exist");
            }
            var responseDto = mapper.Map<ExpenseResponseDTO>(deletedExpense);
            return Ok(responseDto);
        }
    }
}
