using AutoMapper;
using Entity.DTO_s;
using Entity.Models;

namespace Services.Map_Classes
{
    public class MapperClass : Profile
    {
        public MapperClass()
        {
            // Product to ProductDto (for GET operations)
            CreateMap<Product, ProductDto>().ReverseMap();

            // CreateProductDto to Product (for POST operations)
            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(_ => true));

            // UpdateProductDto to Product (for PUT operations)
            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}