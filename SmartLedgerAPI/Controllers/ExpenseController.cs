using APILibrary.Data.Models;
using APILibrary.Services.AI.Repository;
using APILibrary.Services.DTOs.Expense;
using APILibrary.Services.Repository;
using APILibrary.Services.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Claims;

namespace SmartLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExpenseController : ControllerBase
    {
        //private readonly IExpenseRepository expenseRepository;
        //private readonly ICategoryRepository categoryRepository;
        //private readonly IExpenseCategorizerByAi expenseCategorizerByAi;
        //private readonly IMapper mapper;


        private readonly ILogger<ExpenseController> logger;
        private readonly IExpenseService expenseService;

        public ExpenseController(/*IExpenseRepository expenseRepository , IMapper mapper , ICategoryRepository categoryRepository , IExpenseCategorizerByAi expenseCategorizerByAi*/ ILogger<ExpenseController> logger , IExpenseService expenseService)
        {
            //this.expenseRepository = expenseRepository;
            //this.categoryRepository = categoryRepository;
            //this.expenseCategorizerByAi = expenseCategorizerByAi;
            //this.mapper = mapper;


            this.expenseService = expenseService;
            this.logger = logger;

        }










        [HttpPost]
        public async Task<IActionResult> CreateExpense(ExpenseRequestDTO dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            //string categoryNameReturnedByAi = await expenseCategorizerByAi.CategorizeExpenseAsync(dto.Description);

            //var categoryByName = await categoryRepository.GetCategoryByNameAsync(categoryNameReturnedByAi);

            //var expenseModel = mapper.Map<Expense>(dto);

            //expenseModel.CategoryId = categoryByName.Id;
            //expenseModel.Date = dto.Date ?? DateTime.UtcNow;
            //expenseModel.UserId = userId;

            //var category = await categoryRepository.GetCategoryByIdAsync(expenseModel.CategoryId);

            //if (category.CategoryName.Trim() == "Business")
            //{
            //    expenseModel.IsGstApplicable = true;
            //}
            //else
            //{
            //    expenseModel.IsGstApplicable = false;
            //}

            //expenseModel = await expenseRepository.CreateExpenseAsync(expenseModel);

            //var output = mapper.Map<ExpenseResponseDTO>(expenseModel);
            //logger.LogInformation($"{expenseModel.UserId} created expense {expenseModel.Amount}");
            //return Ok(output);



            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);


            var response = await expenseService.CreateExpenseAsync(dto, userId);


            logger.LogInformation($"user {userId} created expense: {response.CategoryName} for {response.Description} - {response.Amount}");

            return Ok(response);
        }












        [HttpGet]
        public async Task<IActionResult> GetAllExpenses(int? categoryId, int? month, decimal? minAmount, decimal? maxAmount,[FromQuery] int pageNo = 1,[FromQuery] int pageSize = 5, [FromQuery] string? sortBy = null, string? sortOrder = null) 
            //[FromQuery] is default
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);




            //var userExpense = await expenseRepository.GetAllExpensesAsync(userId, categoryId, month, minAmount, maxAmount, pageNo, pageSize, sortBy, sortOrder);

            //var userExpensesDTO = mapper.Map<List<ExpenseResponseDTO>>(userExpense);




            var expensesList = await expenseService.GetAllExpensesAsync(userId , categoryId, month, minAmount, maxAmount, pageNo, pageSize, sortBy, sortOrder);

            return Ok(expensesList);
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





            //var expenseModel = mapper.Map<Expense>(dto);

            //string categoryNameReturnedByAi = await expenseCategorizerByAi.CategorizeExpenseAsync(dto.Description);

            //var categoryByName = await categoryRepository.GetCategoryByNameAsync(categoryNameReturnedByAi);

            //expenseModel.CategoryId = categoryByName.Id;

            //var updatedValues = await expenseRepository.UpdateExpensesAsync(userId, id, expenseModel);

            //if(updatedValues == null)
            //{
            //    return BadRequest("USER OR EXPENSE DONT EXIST");
            //}

            //logger.LogInformation($"user {userId} UPDATED EXPENSE id {id}");
            //return Ok(mapper.Map<ExpenseResponseDTO>(updatedValues));






            var response = await expenseService.UpdateExpenseAsync(userId, id, dto);

            if (response == null)
            {
                return BadRequest("User or Expense dont Exist");
            }

            logger.LogInformation($"{userId} updated expense with id - {id}");

            return Ok(response);
        }







        [HttpDelete]
        [Route("{expenseId:int}")]
        public async Task<IActionResult> DeleteExpense(int expenseId)
        {
           var userId =  int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);



            //var deletedExpense = await expenseRepository.DeleteExpenseAsync(userId , expenseId);
            //if (deletedExpense == null)
            //{
            //    return BadRequest("expense does no exist");
            //}
            //var responseDto = mapper.Map<ExpenseResponseDTO>(deletedExpense);

            //logger.LogInformation($"user {userId} deleted id {expenseId}");



            var deletedExpense = await expenseService.DeleteExpenseAsync(userId, expenseId);
            if (deletedExpense == null)
            {
                return BadRequest($"Expense with {expenseId} does not exist");
            }

            logger.LogInformation($"{userId} deleted expense with id - {expenseId}");

            return Ok(deletedExpense);
        }







        [HttpGet("dashboard")]  
        //api/[controller]/dashboard -- same as [Route("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);



            //var dashboard = await expenseRepository.GetDashboardAsync(userId);


            var dashboard = await expenseService.GetDashboardAsync(userId);

            return Ok(dashboard);
        }







        [HttpGet("summary")]
        public async Task<IActionResult> GetExpenseSumarry()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);


             
            //var summary = await expenseRepository.GetExpenseSummaryAsync(userId);


            var summary = await expenseService.GetExpenseSummaryAsync(userId);

            return Ok(summary);
        }
    }
}
