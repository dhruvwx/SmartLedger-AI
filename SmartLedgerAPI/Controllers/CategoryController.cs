using APILibrary.Services.DTOs.Category;
using APILibrary.Services.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SmartLedgerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly ICategoryRepository categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository , IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categoriesList = await categoryRepository.GetAllCategoriesAsync();
            var categories = mapper.Map<List<CategoryResponseDTO>>(categoriesList);
            return Ok(categories);

        }

    }
}
