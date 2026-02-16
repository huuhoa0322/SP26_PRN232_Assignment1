using AutoMapper;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Account;
using FUNewsManagement_v2_CoreAPI.DataAccess.Models;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Category;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.Tag;
using FUNewsManagement_v2_CoreAPI.BusinessLogic.DTOs.News;

namespace FUNewsManagement_v2_CoreAPI.BusinessLogic.Mappings
{
    /// <summary>
    /// AutoMapper profile cho mapping giữa entities và DTOs
    /// </summary>
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // SystemAccount <-> AccountDto
            CreateMap<SystemAccount, AccountDto>(); 
            CreateMap<AccountDto, SystemAccount>();

            // Category <-> CategoryDto
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.NewsCount, opt => opt.MapFrom(src => src.NewsArticles != null ? src.NewsArticles.Count : 0))
                .ForMember(dest => dest.ParentCategoryName, opt => opt.MapFrom(src => src.ParentCategory != null ? src.ParentCategory.CategoryName : null));
            CreateMap<CreateCategoryRequest, Category>();
            CreateMap<UpdateCategoryRequest, Category>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Tag <-> TagDto
            CreateMap<Tag, TagDto>()
                .ForMember(dest => dest.NewsCount, opt => opt.MapFrom(src => src.NewsArticles != null ? src.NewsArticles.Count : 0));
            CreateMap<CreateTagRequest, Tag>();
            CreateMap<UpdateTagRequest, Tag>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // NewsArticle <-> NewsArticleDto
            CreateMap<NewsArticle, NewsArticleDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : null))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy != null ? src.CreatedBy.AccountName : null))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags));
            CreateMap<CreateNewsArticleRequest, NewsArticle>();
            CreateMap<UpdateNewsArticleRequest, NewsArticle>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
