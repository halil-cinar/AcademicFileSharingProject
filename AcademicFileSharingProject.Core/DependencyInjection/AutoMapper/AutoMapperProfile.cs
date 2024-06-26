﻿
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Core.DependencyInjection.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {


            #region ChatMapping

            CreateMap<ChatListDto, ChatEntity>()
                .ReverseMap();

            CreateMap<ChatDto, ChatEntity>()
                .ReverseMap();

            #endregion



            #region ChatUserMapping

            CreateMap<ChatUserListDto, ChatUserEntity>()

                .ForMember(x => x.Chat, opt => opt.Ignore());

            CreateMap<ChatUserEntity, ChatUserListDto>()

                           .ForMember(x => x.Chat, opt => opt.Ignore());



            CreateMap<ChatUserDto, ChatUserEntity>()
                .ReverseMap();

            #endregion



            #region CommentMapping

            CreateMap<PostCommentListDto, PostCommentEntity>()
                .ReverseMap();

            CreateMap<PostCommentDto, PostCommentEntity>()
                .ReverseMap();

            #endregion



            #region IdentityMapping

            CreateMap<IdentityListDto, IdentityEntity>()
                .ReverseMap();

            CreateMap<IdentityDto, IdentityEntity>()
                .ReverseMap();

            #endregion



            #region MediaMapping

            CreateMap<MediaListDto, MediaEntity>()
                .ReverseMap();

            CreateMap<MediaDto, MediaEntity>()
                .ReverseMap();

            #endregion



            #region MessageMapping

            CreateMap<MessageEntity, MessageListDto>()
                .ForMember(x => x.Chat, opt => opt.MapFrom(x => new ChatListDto
                {
                    ChatType = x.Chat.ChatType,
                    Title = x.Chat.Title,
                    CreatedTime = x.Chat.CreatedTime,
                    Id = x.Chat.Id,
                    IsDeleted = x.Chat.IsDeleted,
                }))
                .ReverseMap();

            CreateMap<MessageDto, MessageEntity>()
                .ForMember(x => x.Chat, opt => opt.Ignore())
                .ReverseMap();

            #endregion



            #region PostMapping

            CreateMap<PostListDto, PostEntity>()
                .ReverseMap();

            CreateMap<PostDto, PostEntity>()
                .ForMember(x => x.PostMedia, x => x.Ignore())
                .ReverseMap();

            CreateMap<PostDto, PostListDto>()
                .ForMember(x => x.PostMedia, x => x.Ignore())
                .ForPath(x => x.PostMedia, x => x.Ignore());
            CreateMap<PostListDto, PostDto>()
                            .ForMember(x => x.PostMedia, x => x.Ignore())
                            .ForPath(x => x.PostMedia, x => x.Ignore());



            #endregion



            #region PostMediaMapping

            CreateMap<PostMediaListDto, PostMediaEntity>()
                .ReverseMap();

            CreateMap<PostMediaDto, PostMediaEntity>()
                .ForMember(x => x.Media, x => x.Ignore())
                .ReverseMap();

            #endregion



            #region UserMapping

            CreateMap<UserListDto, UserEntity>()
                .ReverseMap();

            CreateMap<UserDto, UserEntity>()
                .ReverseMap();
            CreateMap<UserDto, UserListDto>()
                            .ReverseMap();


            #endregion



            #region UserRoleMapping

            CreateMap<UserRoleListDto, UserRoleEntity>()
                .ReverseMap();

            CreateMap<UserRoleDto, UserRoleEntity>()
                .ReverseMap();

            #endregion




            #region BlogMapping

            CreateMap<BlogListDto, BlogEntity>()
                .ReverseMap();

            CreateMap<BlogDto, BlogEntity>()
                .ReverseMap();
            CreateMap<BlogDto, BlogListDto>()
                .ForMember(x => x.Media, x => x.Ignore())
               ;
            CreateMap<BlogListDto, BlogDto>()
                            .ForMember(x => x.Media, x => x.Ignore())
                           ;


            #endregion



            #region BlogCommentMapping

            CreateMap<BlogCommentListDto, BlogCommentEntity>()
                .ReverseMap();

            CreateMap<BlogCommentDto, BlogCommentEntity>()
                .ReverseMap();
            CreateMap<BlogCommentDto, BlogCommentListDto>()
               ;
            CreateMap<BlogCommentListDto, BlogCommentDto>()
                           ;


            #endregion


            #region PostCommentMapping

            CreateMap<PostCommentListDto, PostCommentEntity>()
                .ReverseMap();

            CreateMap<PostCommentDto, PostCommentEntity>()
                .ForMember(x => x.Media, x => x.Ignore())
                .ReverseMap();
            CreateMap<PostCommentDto, PostCommentListDto>()
                .ForMember(x => x.Media, x => x.Ignore())
               ;
            CreateMap<PostCommentListDto, PostCommentDto>()
                            .ForMember(x => x.Media, x => x.Ignore())
                           ;


            #endregion


            #region SessionMapping

            CreateMap<SessionListDto, SessionEntity>()
                .ReverseMap();

            CreateMap<SessionDto, SessionEntity>()
                .ReverseMap();
            CreateMap<SessionDto, SessionListDto>()
               ;
            CreateMap<SessionListDto, SessionDto>()
                           ;


            #endregion


            #region RoleMethodMapping

            CreateMap<RoleMethodListDto, RoleMethodEntity>()
                .ReverseMap();

            CreateMap<RoleMethodDto, RoleMethodEntity>()
                .ReverseMap();
            CreateMap<RoleMethodDto, RoleMethodListDto>()
               ;
            CreateMap<RoleMethodListDto, RoleMethodDto>()
                           ;


            #endregion



            #region SystemSettingsMapping

            CreateMap<SystemSettingsListDto, SystemSettingsEntity>()
                .ReverseMap();

            CreateMap<SystemSettingsDto, SystemSettingsEntity>()
                .ReverseMap();

            #endregion

            #region SubscribeMapping

            CreateMap<SubscribeListDto, SubscribeEntity>()
                .ReverseMap();

            CreateMap<SubscribeDto, SubscribeEntity>()
                .ReverseMap();

            #endregion

            #region NotificationMapping

            CreateMap<NotificationListDto, NotificationEntity>()
                .ReverseMap();

            CreateMap<NotificationDto, NotificationEntity>()
                .ReverseMap();

            #endregion

            #region SoftwareMapping

            CreateMap<SoftwareListDto, SoftwareEntity>()
                .ReverseMap();

            CreateMap<SoftwareDto, SoftwareEntity>()
                .ForMember(x => x.File, opt => opt.Ignore())
                .ForMember(x => x.Logo, opt => opt.Ignore())
                .ForMember(x => x.Document, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<SoftwareDto, SoftwareListDto>()
                            .ForMember(x => x.File, opt => opt.Ignore())
                            .ForMember(x => x.Logo, opt => opt.Ignore())
                            .ForMember(x => x.Document, opt => opt.Ignore())
                            ;
            CreateMap<SoftwareListDto, SoftwareDto>()
                            .ForMember(x => x.File, opt => opt.Ignore())
                            .ForMember(x => x.Logo, opt => opt.Ignore())
                            .ForMember(x => x.Document, opt => opt.Ignore())
                            ;

            #endregion

            #region PostMediaDownloadMapping

            CreateMap<PostMediaDownloadListDto, PostMediaDownloadEntity>()
                .ReverseMap();

            CreateMap<PostMediaDownloadDto, PostMediaDownloadEntity>()
                .ReverseMap();

            #endregion 
            
            #region UserDeviceMapping

            CreateMap<UserDeviceListDto, UserDeviceEntity>()
                .ReverseMap();

            CreateMap<UserDeviceDto, UserDeviceEntity>()
                .ReverseMap();

            #endregion


        }
    }
}
