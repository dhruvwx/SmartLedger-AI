using APILibrary.Services.DTOs.Category;
using APILibrary.Services.Repository;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository categoryRepo;
        private readonly IMapper mapper;
        public CategoryService(ICategoryRepository categoryRepo, IMapper mapper)
        {
            this.mapper = mapper;
            this.categoryRepo = categoryRepo;
        }



        public async Task<List<CategoryResponseDTO>> GetAllCategoriesAsync()
        {
            var categories = await categoryRepo.GetAllCategoriesAsync();

            return mapper.Map<List<CategoryResponseDTO>>(categories);
        }
    }
}
