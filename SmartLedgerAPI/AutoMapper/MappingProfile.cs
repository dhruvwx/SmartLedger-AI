using APILibrary.Data.Models;
using APILibrary.Services.DTOs.Auth;
using APILibrary.Services.DTOs.Budget;
using APILibrary.Services.DTOs.Category;
using APILibrary.Services.DTOs.Expense;
using AutoMapper;

namespace SmartLedgerAPI.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, LoginRegisterResponseDTO>();

            CreateMap<Expense, ExpenseResponseDTO>()
                            .ForMember(e => e.CategoryName,
                                        opt => opt.MapFrom(e => e.Category.CategoryName));
            CreateMap<ExpenseResponseDTO, Expense>();
            CreateMap<ExpenseRequestDTO , Expense>().ReverseMap();
            CreateMap<UpdateExpenseDTO , Expense>().ReverseMap();

            CreateMap<Category , CategoryResponseDTO>().ReverseMap();

            CreateMap<Budget, CreateBudgetDTO>().ReverseMap();
            CreateMap<BudgetResponseDTO, Budget>().ReverseMap();
            CreateMap<UpdateBudgetDTO, Budget>().ReverseMap();  
        }
    }
}
