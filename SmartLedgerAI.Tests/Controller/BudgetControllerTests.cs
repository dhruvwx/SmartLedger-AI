using Microsoft.AspNetCore.Mvc;
using System.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using APILibrary.Services.DTOs.Budget;
using Moq;
using APILibrary.Services.Services;
using Serilog.Core;
using SmartLedgerAPI.Controllers;
using Microsoft.Extensions.Logging;

namespace SmartLedgerAI.Tests.Controller
{
    public class BudgetControllerTests
    {
        //crate user coz controller expects NameIdentifier for verification , we dont hit JWT 
                //Controller Context is that gives access to the controller

        protected ControllerContext FakeUserContext(int userId)
        {
            var claim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());

            List<Claim> claims = new List<Claim>();
            claims.Add(claim);

            //identity - person who own the claims
                  //new ClaimsIdentity(claims, authenticationType);
            var Identity = new ClaimsIdentity(claims ,  "TestAuth");


            //ClaimsPrincipal - LoggedIn User
            var user = new ClaimsPrincipal(Identity);

            //HttpContext  - container that holds current HTTP REQUEST =  header , cookies , User -- it is the enviornment 
            var httpContext = new DefaultHttpContext { User = user };

            return new ControllerContext { HttpContext = httpContext };
        }



        [Fact]
        public async Task CreateBudget_ValidInput_ReturnsOk()
        {
            var createBudgetDto = new CreateBudgetDTO
            {
                MonthMaxAmountLimit = 1000,
                Month = 6,
                Year = 2026,
                CategoryId = 1
            };

            var responseDto = new BudgetResponseDTO
            {
                MonthMaxAmountLimit = createBudgetDto.MonthMaxAmountLimit,
                Month = createBudgetDto.Month,
                Year = createBudgetDto.Year,
                CategoryId = createBudgetDto.CategoryId,
                SpentAmount = 700,
                RemainingAmount = 300,
                IsExceeded = false,
                CategoryName = "Travel",
                WarningMessage = "",
                BudgetId = 1
            };

            var mockBudgetService = new Mock<IBudgetService>();
            mockBudgetService.Setup(b => b.CreateBudgetAsync(createBudgetDto, 1))
                             .ReturnsAsync(responseDto);

            var mockLogger = new Mock<ILogger<BudgetController>>();

            var budgetController = new BudgetController
                (
                    mockLogger.Object,
                    mockBudgetService.Object
                );

            //TO PRETEND USER WITH ID PASSED IN -CreateBudgetAsync(createBudgetDto, 1) i.e. 1 IS LOGGED IN 
            budgetController.ControllerContext = FakeUserContext(1);

            //act
            var result = await budgetController.CreateBudget(createBudgetDto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<BudgetResponseDTO>(okResult.Value);

            Assert.Equal(responseDto.BudgetId, response.BudgetId);

            mockBudgetService.Verify(s => s.CreateBudgetAsync(createBudgetDto, 1), Times.Once);
        }

    }
}
