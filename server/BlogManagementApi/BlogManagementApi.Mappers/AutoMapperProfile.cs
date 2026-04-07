using AutoMapper;
using BlogManagementApi.Domain.Models;
using BlogManagementApi.DTOs.AdminUsers;
using BlogManagementApi.DTOs.Posts;
using BlogManagementApi.DTOs.Users;

namespace BlogManagementApi.Mappers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Post, PostResponseDto>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src =>
                    src.User != null ? $"{src.User.FirstName} {src.User.LastName}" : src.Author));

            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<User, AdminUserResponseDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email ?? string.Empty))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());
        }
    }
}
