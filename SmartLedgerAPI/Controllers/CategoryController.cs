using APILibrary.Services.DTOs.Category;
using APILibrary.Services.Repository;
using APILibrary.Services.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        //private readonly ICategoryRepository categoryRepository;
        //private readonly IMapper mapper;
        private readonly ICategoryService categoryService;
        public CategoryController(/*ICategoryRepository categoryRepository , IMapper mapper , */ICategoryService categoryService)
        {
            //this.categoryRepository = categoryRepository;
            //this.mapper = mapper;


            this.categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            //var categoriesList = await categoryRepository.GetAllCategoriesAsync();
            //var categories = mapper.Map<List<CategoryResponseDTO>>(categoriesList);

            var categories = await categoryService.GetAllCategoriesAsync();
            return Ok(categories);

        }

    }
}
