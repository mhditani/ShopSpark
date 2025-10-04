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







            // Order Mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    src.Customer != null ? $"{src.Customer.FirstName} {src.Customer.LastName}" : "Unknown"));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<CreateOrderDto, Order>()
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => "Pending"));

            CreateMap<CreateOrderItemDto, OrderItem>();

            CreateMap<UpdateOrderStatusDto, Order>().ReverseMap();



            // Customer Mappings
            CreateMap<ApplicationUser, CustomerDto>().ReverseMap();
            CreateMap<UpdateCustomerDto, ApplicationUser>().ReverseMap();

        }
    }
}