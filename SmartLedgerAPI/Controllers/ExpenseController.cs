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
        private readonly IMapper mapper;

        public ExpenseController(IExpenseRepository expenseRepository , IMapper mapper)
        {
            this.expenseRepository = expenseRepository;
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

            expenseModel = await expenseRepository.CreateExpenseAsync(expenseModel);

            var output = mapper.Map<ExpenseResponseDTO>(expenseModel);
            return Ok(output);
        }


        [HttpGet]
        public async Task<IActionResult> GetAllExpenses()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var userExpense = await expenseRepository.GetAllExpensesAsync(userId);

            var userExpensesDTO = mapper.Map<List<ExpenseResponseDTO>>(userExpense);

            return Ok(userExpensesDTO);
        }


        [HttpPut]
        [Route("id")]
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
    }
}
