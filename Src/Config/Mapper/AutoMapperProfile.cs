using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Src.Dtos.Category;
using Src.Dtos.Comment;
using Src.Dtos.Post;
using Src.Dtos.Tag;
using Src.Dtos.User;
using Src.Models;

namespace Src.Config.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Post Mapping
            CreateMap<Post, PostDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src.Tag))
                .ForMember(dest => dest.AppUser, opt => opt.MapFrom(src => src.AppUser))
                .ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.Comments));

            // CreateMap<PostDto, Post>()
            //     .ForMember(dest => dest.Category, opt => opt.Ignore())
            //     .ForMember(dest => dest.Tag, opt => opt.Ignore())
            //     .ForMember(dest => dest.AppUser, opt => opt.Ignore());


            // Category Mapping
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                    src.Tags != null ? src.Tags.Select(tag => new TagIdDto { TagID = tag.TagID, TagName = tag.TagName }) : null))
                .ForMember(dest => dest.Posts, opt => opt.MapFrom(src => src.Posts));

            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.Tags, opt => opt.Ignore())
                .ForMember(dest => dest.Posts, opt => opt.Ignore());


            // Tag Mapping
            CreateMap<Tag, TagDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Posts, opt => opt.Ignore());

            CreateMap<TagDto, Tag>()
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Posts, opt => opt.Ignore());


            // User Mapping
            CreateMap<AppUser, UserDto>();
            CreateMap<UserDto, AppUser>();

            // Comment Mapping
            CreateMap<Comment, CommentsDto>()
               .ForMember(dest => dest.AppUser, opt => opt.MapFrom(src => src.AppUser))
                .ForMember(dest => dest.Post, opt => opt.MapFrom(src => src.Post));
            CreateMap<CommentsDto, Comment>()
               .ForMember(dest => dest.AppUser, opt => opt.Ignore());
        }
    }
}