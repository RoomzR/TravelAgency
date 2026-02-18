using AutoMapper;
using TravelAgency.BLL.DTOs;
using TravelAgency.DAL.Entities;

namespace TravelAgency.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Tour, TourDTO>()
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
                .ForMember(dest => dest.HotelCategoryName, opt => opt.MapFrom(src => src.HotelCategory.Name))
                .ForMember(dest => dest.TourTypeName, opt => opt.MapFrom(src => src.TourType.Name))
                .ForMember(dest => dest.HotelName, opt => opt.MapFrom(src => src.Hotel.Name))
                .ReverseMap();

            CreateMap<TourCreateDTO, Tour>();
            CreateMap<TourUpdateDTO, Tour>();

            CreateMap<TourType, TourTypeDTO>().ReverseMap();

            CreateMap<Country, CountryDTO>().ReverseMap();
            CreateMap<CountryCreateDTO, Country>();
            CreateMap<CountryUpdateDTO, Country>();

            CreateMap<City, CityDTO>()
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name))
                .ReverseMap();

            CreateMap<Hotel, HotelDTO>()
                .ForMember(dest => dest.HotelCategoryName, opt => opt.MapFrom(src => src.HotelCategory.Name))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
                .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.City.Country.Name))
                .ReverseMap();
            
            CreateMap<Booking, BookingDTO>()
                          .ForMember(dest => dest.TourTitle, opt => opt.MapFrom(src => src.Tour.Title))
                          .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.UserName))
                          .ForMember(dest => dest.FinalPrice, opt => opt.MapFrom(src => src.FinalPrice))
                          .ReverseMap();

            CreateMap<BookingCreateDTO, Booking>();
            CreateMap<BookingUpdateDTO, Booking>();

            CreateMap<Payment, PaymentDTO>().ReverseMap();

            CreateMap<NewsArticle, NewsDTO>()
                 .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName))
                 .ReverseMap();

            CreateMap<NewsCreateDTO, NewsArticle>();
            CreateMap<NewsUpdateDTO, NewsArticle>();

            CreateMap<Review, ReviewDTO>()
            .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.UserName)) 
            .ForMember(dest => dest.TourTitle, opt => opt.MapFrom(src => src.Tour.Title));

            CreateMap<ReviewCreateDTO, Review>();

            CreateMap<Promocode, PromoCodeDTO>().ReverseMap();
            CreateMap<PromoCodeCreateDTO, Promocode>();
            CreateMap<PromoCodeUpdateDTO, Promocode>();

            CreateMap<FAQ, FAQDTO>().ReverseMap();
            CreateMap<FAQ, FAQCreateDTO>().ReverseMap();
            CreateMap<FAQ, FAQUpdateDTO>().ReverseMap();

            CreateMap<ContactRequest, ContactRequestDTO>().ReverseMap();
            CreateMap<ContactRequest, ContactRequestCreateDTO>().ReverseMap();
            CreateMap<ContactRequest, ContactRequestUpdateDTO>().ReverseMap();

            CreateMap<TourImage, TourImageDTO>().ReverseMap();
        }
    }
}