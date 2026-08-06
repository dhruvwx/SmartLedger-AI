using APILibrary.Services.DTOs.Category;
using APILibrary.Services.Repository;
using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
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

        //add IMemoryCache and logger
        private readonly ILogger<CategoryService> logger;
        private readonly IMemoryCache cache;
        public CategoryService(ICategoryRepository categoryRepo, IMapper mapper, ILogger<CategoryService> logger, IMemoryCache cache)
        {
            this.mapper = mapper;
            this.categoryRepo = categoryRepo;

            this.logger = logger;
            this.cache = cache;
        }

        private const string cacheKey = "AllCategories"; //declare once for all methods



        public async Task<List<CategoryResponseDTO>> GetAllCategoriesAsync()
        {

            if(cache.TryGetValue(cacheKey , out List<CategoryResponseDTO> resultCashedData))
            {
                logger.LogInformation("Cache HIT on key {CacheKey}", cacheKey);
                return resultCashedData;
            }

            //if TryGetValue returns false - no key or null/expired value : FOLLOWING CODE RUNS i.e - fetches from Db and storing as cache in memory

            logger.LogInformation("Cache MISS for key {CacheKey} - fetch from db", cacheKey);

            //fetch db
            var categories = await categoryRepo.GetAllCategoriesAsync();
            var responseDTOs = mapper.Map<List<CategoryResponseDTO>>(categories);

            //invalidations - with rules of memory entry options
            var memoryCacheOptions = new MemoryCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromMinutes(2)) // renews with every read
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)); //fixed , delete after 10 minutes

            //set value to cache
            cache.Set(cacheKey, responseDTOs, memoryCacheOptions);

            return responseDTOs;
        }
        //If write methods are there Create ,Update ,Delete then use- for all after database code before return. - db.Update , db.Delete , db.Add
                //cache.Remove(cacheKey);
    }
}
