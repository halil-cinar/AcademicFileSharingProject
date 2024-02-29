using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Core.DataAccess;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.Filters;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Dtos.Result;
using AcademicFileSharingProject.Entities;
using AcademicFileSharingProject.Entities.Abstract;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Business
{
    public class ChatUserManager : ServiceBase<ChatUserEntity>, IChatUserService
    {
        public ChatUserManager(IEntityRepository<ChatUserEntity> repository, IMapper mapper, BaseEntityValidator<ChatUserEntity> validator) : base(repository, mapper, validator)
        {
        }

        public async Task<BussinessLayerResult<ChatUserListDto>> Add(ChatUserDto chatuser)
        {
            var response = new BussinessLayerResult<ChatUserListDto>();
            try
            {
                var entity = Mapper.Map<ChatUserEntity>(chatuser);
                entity.CreatedTime = DateTime.Now;
                entity.IsDeleted = false;
                entity.Id = 0;

                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.ChatUserChatUserAddValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<ChatUserListDto>(Repository.Add(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.ChatUserChatUserAddExceptionError, ex.Message);
            }
            return response;

        }

        public async Task<BussinessLayerResult<ChatUserListDto>> Delete(long id)
        {
            var response = new BussinessLayerResult<ChatUserListDto>();
            try
            {
                Repository.SoftDelete(id);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.ChatUserChatUserDeleteExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<ChatUserListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<ChatUserListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<ChatUserListDto>(entity);
                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.ChatUserChatUserGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<GenericLoadMoreDto<ChatUserListDto>>> GetAll(LoadMoreFilter<ChatUserFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<ChatUserListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                (filter.Filter.ChatId == null || filter.Filter.ChatId == x.ChatId)
                && (filter.Filter.UserId == null || filter.Filter.UserId == x.UserId)
                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex = firstIndex + filter.ContentCount;

                lastIndex = Math.Min(lastIndex, entities.Count);
                var values = new List<ChatUserListDto>();
                for (int i = firstIndex; i < lastIndex; i++)
                {
                    var item = entities[i];



                    values.Add(new ChatUserListDto
                    {
                        ChatId = item.ChatId,
                        UserId = item.UserId,
                        CreatedTime = item.CreatedTime,
                        Chat = new ChatListDto
                        {
                            ChatType = item.Chat.ChatType,
                            CreatedTime = item.CreatedTime,
                            Title = item.Chat.Title,
                            ChatUsers = entities.Select(x => new ChatUserListDto
                            {
                                ChatId = x.ChatId,
                                UserId = x.UserId,
                                CreatedTime = x.CreatedTime,
                                User = Mapper.Map<UserListDto>(x.User),
                                IsDeleted = x.IsDeleted,
                                Id = x.Id,
                            }).ToList(),
                            Id= item.Chat.Id,
                        },
                        User = Mapper.Map<UserListDto>(item.User),
                        Id = item.Id,
                        IsDeleted = item.IsDeleted
                    }); ;
                }

                response.Result = new GenericLoadMoreDto<ChatUserListDto>
                {
                    Values = values,
                    ContentCount = filter.ContentCount,
                    NextPage = lastIndex < entities.Count,
                    TotalPageCount = Convert.ToInt32(Math.Ceiling(entities.Count / (double)filter.ContentCount)),
                    TotalContentCount = entities.Count,
                    PageCount = filter.PageCount > Convert.ToInt32(Math.Ceiling(entities.Count / (double)filter.ContentCount))
                    ? Convert.ToInt32(Math.Ceiling(entities.Count / (double)filter.ContentCount))
                    : filter.PageCount,
                    PrevPage = firstIndex > 0


                };

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.ChatUserChatUserGetAllExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<ChatUserListDto>> Update(ChatUserDto chatuser)
        {
            var response = new BussinessLayerResult<ChatUserListDto>();
            try
            {
                var entity = Repository.Get(chatuser.Id);
                entity.ChatId = chatuser.Id;
                entity.UserId = chatuser.UserId;





                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.ChatUserChatUserUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<ChatUserListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.ChatUserChatUserUpdateExceptionError, ex.Message);
            }
            return response;
        }
    }
}

