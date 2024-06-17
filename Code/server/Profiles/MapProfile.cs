using AutoMapper;
using RecommendationEngineServer.Models.DTOs;
using RecommendationEngineServer.Models.Entities;

namespace RecommendationEngineServer.Profiles
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<FoodItem, FoodItemDTO>();
            CreateMap<FoodItemDTO, FoodItem>();

            CreateMap<FeedbackDTO, Feedback>();
            CreateMap<Feedback, FeedbackDTO>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.FoodItem.ItemName));
            CreateMap<Feedback, DisplayFeedbackDTO>()
               .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating.ToString()))
               .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment))
               .ForMember(dest => dest.FeedbackDate, opt => opt.MapFrom(src => src.FeedbackDate));

            CreateMap<RecommendedMenuDTO, RecommendedMenu>()
           .ForMember(dest => dest.MenuId, opt => opt.Ignore())
           .ForMember(dest => dest.User, opt => opt.Ignore())
           .ForMember(dest => dest.FoodItem, opt => opt.Ignore());

            CreateMap<RecommendedMenu, RecommendedMenuDTO>();

            CreateMap<RecommendedMenu, DisplayMenuDTO>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.FoodItem.ItemName))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.FoodItem.Price));
            //.ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.FoodItem.Rating));

            CreateMap<RecommendedMenu, DisplayMenuDTO>()
            .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.FoodItem.ItemName))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.FoodItem.Price));
        }
    }
}
