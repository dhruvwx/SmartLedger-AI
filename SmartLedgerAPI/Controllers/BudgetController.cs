using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Budget;
using APILibrary.Services.Repository;
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
        private readonly IBudgetRepository budgetRepository;
        private readonly IMapper mapper;

        public BudgetController(IBudgetRepository budgetRepository,
         IMapper mapper)
        {
            this.budgetRepository = budgetRepository;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBudget(CreateBudgetDTO dto)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var budgetModel = mapper.Map<Budget>(dto);

            budgetModel.UserId = userId;

            var createBudget = await budgetRepository.CreateBudgetAsync(budgetModel);

            //var output = mapper.Map<BudgetResponseDTO>(createBudget);

            return Ok(createBudget);
        }

        [HttpGet]
        public async Task<IActionResult> GetBudgets()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var budgets = await budgetRepository.GetBudgetsAsync(userId);

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

            var budgetModel = mapper.Map<Budget>(dto);

            var updatedBudget = await budgetRepository.UpdateBudgetAsync(userId, budgetId, budgetModel);

            if(updatedBudget == null)
            {
                return BadRequest("Budget does not exist");
            }

            var responseDto = mapper.Map<BudgetResponseDTO>(updatedBudget);

            return Ok(responseDto);
        }

        [HttpDelete]
        [Route("{budgetId}")]
        public async Task<IActionResult> DeleteBudget([FromRoute]int budgetId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var deletedBudget = await budgetRepository.DeleteBudgetAsync(userId, budgetId);

            if(deletedBudget == null)
            {
                return BadRequest("Budget does not exist");
            }

            var responseDto = mapper.Map<BudgetResponseDTO>(deletedBudget);

            return Ok(responseDto);
        }
    }
}

