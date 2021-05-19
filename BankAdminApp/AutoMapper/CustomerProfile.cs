using AutoMapper;
using SharedThings.Models;
using SharedThings.ViewModels;

namespace BankAdminApp.AutoMapper
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerNewViewModel>();
        }
    }
}