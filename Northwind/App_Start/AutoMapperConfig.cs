using Northwind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Northwind
{
    public class AutoMapperConfig
    {
        // Using AutoMapper: Creating Mappings
        // http://cpratt.co/using-automapper-creating-mappings/
        public static void RegisterMappings()
        {
            //AutoMapper.Mapper.CreateMap<Book, BookViewModel>()
            //    .ForMember(dest => dest.BootTitle, opts => opts.MapFrom(src => src.Title));
            AutoMapper.Mapper.Initialize(cfg => {
                cfg.CreateMap<Employee, EmployeeVM>();
                cfg.CreateMap<Employee, EmployeeVM>().ReverseMap();
                cfg.CreateMap<Customer, CustomerVM>();
                cfg.CreateMap<Customer, CustomerVM>().ReverseMap();
            });
        }
    }
}