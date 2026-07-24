using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Budget;
using APILibrary.Services.Repository;
using APILibrary.Services.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SmartLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController : ControllerBase
    {
        //private readonly IBudgetRepository budgetRepository;
        //private readonly IMapper mapper;

        private readonly IBudgetService budgetService;
        private readonly ILogger<BudgetController> logger;

        public BudgetController(/*IBudgetRepository budgetRepository,
         IMapper mapper , */ ILogger<BudgetController> logger , IBudgetService budgetService)
        {
            //this.budgetRepository = budgetRepository;
            //this.mapper = mapper;
            this.logger = logger;
            this.budgetService = budgetService;
        }















        [HttpPost]
        public async Task<IActionResult> CreateBudget(CreateBudgetDTO dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);



            //var budgetModel = mapper.Map<Budget>(dto);

            //budgetModel.UserId = userId;

            //var createBudget = await budgetRepository.CreateBudgetAsync(budgetModel);

            ////var output = mapper.Map<BudgetResponseDTO>(createBudget);
            //logger.LogInformation($"{userId} created budget");

            //var resonseBudgetDto = mapper.Map<BudgetResponseDTO>(createBudget);

            //return Ok(resonseBudgetDto);



            var response = await budgetService.CreateBudgetAsync(dto , userId);

            logger.LogInformation($" user {userId} created budget - {response.BudgetId}");

            return Ok(response);
        }














        [HttpGet]
        public async Task<IActionResult> GetBudgets()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);



            //var budgets = await budgetRepository.GetBudgetsAsync(userId);

            //return Ok(budgets);


            var budgets = await budgetService.GetBudgetsAsync(userId);
            return Ok(budgets);
        }








        [HttpPut]
        [Route("{budgetId}")]
        public async Task<IActionResult> UpdateBudget([FromRoute] int budgetId, UpdateBudgetDTO dto)
        {
            if(ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);



            //var budgetModel = mapper.Map<Budget>(dto);

            //var updatedBudget = await budgetRepository.UpdateBudgetAsync(userId, budgetId, budgetModel);

            //if(updatedBudget == null)
            //{
            //    return BadRequest("Budget does not exist");
            //}

            //var responseDto = mapper.Map<BudgetResponseDTO>(updatedBudget);

            //logger.LogInformation($"{userId} updated budget id {budgetId}");

            //return Ok(responseDto);



            var response = await budgetService.UpdateBudgetAsync(userId, budgetId, dto);
            
            if(response == null) { return BadRequest("Budget or User dont exist");  }

            logger.LogInformation($"User {userId} updated budget id - {budgetId}");

            return Ok(response);
        }












        [HttpDelete]
        [Route("{budgetId}")]
        public async Task<IActionResult> DeleteBudget([FromRoute]int budgetId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            



            //var deletedBudget = await budgetRepository.DeleteBudgetAsync(userId, budgetId);

            //if(deletedBudget == null)
            //{
            //    return BadRequest("Budget does not exist");
            //}

            //var responseDto = mapper.Map<BudgetResponseDTO>(deletedBudget);

            //logger.LogInformation($"{userId} deleted budget id {budgetId}");



            var responseDto = await budgetService.DeleteBudgetAsync(userId, budgetId);

            if(responseDto == null) { return BadRequest("Budget or User dont Exist"); }

            logger.LogInformation($"User {userId} deleted budget with id - {budgetId}");

            return Ok(responseDto);
        }
    }
}

