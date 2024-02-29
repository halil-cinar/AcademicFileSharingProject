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
using System.Transactions;

namespace AcademicFileSharingProject.Business
{
    public class ChatManager : ServiceBase<ChatEntity>, IChatService
    {
        private readonly IChatUserService _chatUserService;
        public ChatManager(IEntityRepository<ChatEntity> repository, IMapper mapper, BaseEntityValidator<ChatEntity> validator, IChatUserService chatUserService) : base(repository, mapper, validator)
        {
            _chatUserService = chatUserService;
        }

        public async Task<BussinessLayerResult<ChatListDto>> Add(ChatDto chat)
        {
            var response = new BussinessLayerResult<ChatListDto>();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (chat.ChatType == Entities.Enums.EChatType.Private)
                    {
                        var control = Repository.GetAll(x => x.ChatUsers.Count == 2 && x.ChatUsers.Select(x => x.UserId).Contains(chat.UserIds[0]) && x.ChatUsers.Select(x => x.UserId).Contains(chat.UserIds[1]));
                        if(control!=null && control.Count != 0)
                        {
                            response.Result = Mapper.Map<ChatListDto>( control.ToList()[0]);
                            return response;
                        }
                    }

                    var entity = Mapper.Map<ChatEntity>(chat);
                    entity.CreatedTime = DateTime.Now;
                    entity.IsDeleted = false;
                    entity.Id = 0;

                    var validationResult = Validator.Validate(entity);
                    if (!validationResult.IsValid)
                    {
                        scope.Dispose();
                        foreach (var item in validationResult.Errors)
                        {
                            response.AddError(Dtos.Enums.ErrorMessageCode.ChatChatAddValidationError, item.ErrorMessage);

                        }
                        return response;
                    }

                    response.Result = Mapper.Map<ChatListDto>(Repository.Add(entity));

                    //Add Chat user

                    foreach (var userId in chat.UserIds)
                    {
                        var result = await _chatUserService.Add(new ChatUserDto
                        {
                            UserId = userId,
                            ChatId = entity.Id,

                        });

                        if (result.ResultStatus == Dtos.Enums.ResultStatus.Error)
                        {
                            scope.Dispose();
                            response.ErrorMessages.AddRange(result.ErrorMessages);
                            return response;
                        }
                    }


                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    response.AddError(Dtos.Enums.ErrorMessageCode.ChatChatAddExceptionError, ex.Message);
                }
            }
            return response;

        }

        public async Task<BussinessLayerResult<ChatListDto>> Delete(long id)
        {
            var response = new BussinessLayerResult<ChatListDto>();
            try
            {
                Repository.SoftDelete(id);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.ChatChatDeleteExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<ChatListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<ChatListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<ChatListDto>(entity);

                var chatUserResult = await _chatUserService.GetAll(new LoadMoreFilter<ChatUserFilter>
                {
                    ContentCount = int.MaxValue,
                    PageCount = 0,
                    Filter = new ChatUserFilter
                    {
                        ChatId = id,
                    }
                });
                if (dto != null && chatUserResult.ResultStatus == Dtos.Enums.ResultStatus.Success)
                {
                    dto.ChatUsers = chatUserResult.Result?.Values;

                }

                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.ChatChatGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<GenericLoadMoreDto<ChatListDto>>> GetAll(LoadMoreFilter<ChatFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<ChatListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                (string.IsNullOrEmpty(filter.Filter.Title) || x.Title.Contains(filter.Filter.Title))
                && (filter.Filter.ChatType == null || filter.Filter.ChatType == x.ChatType)
                && (filter.Filter.UserId == null || x.ChatUsers.Select(x => x.UserId).Contains((long)filter.Filter.UserId))
                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex = firstIndex + filter.ContentCount;

                lastIndex = Math.Min(lastIndex, entities.Count);
                var values = new List<ChatListDto>();
                for (int i = firstIndex; i < lastIndex; i++)
                {
                    values.Add(Mapper.Map<ChatListDto>(entities[i]));
                }

                response.Result = new GenericLoadMoreDto<ChatListDto>
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
                response.AddError(Dtos.Enums.ErrorMessageCode.ChatChatGetAllExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<ChatListDto>> Update(ChatDto chat)
        {
            var response = new BussinessLayerResult<ChatListDto>();
            try
            {
                var entity = Repository.Get(chat.Id);
                entity.Title = chat.Title;
                entity.ChatType = chat.ChatType;




                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.ChatChatUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<ChatListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.ChatChatUpdateExceptionError, ex.Message);
            }
            return response;
        }
    }
}
