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
    public class MessageManager : ServiceBase<MessageEntity>, IMessageService
    {
        public MessageManager(IEntityRepository<MessageEntity> repository, IMapper mapper, BaseEntityValidator<MessageEntity> validator) : base(repository, mapper, validator)
        {
        }

        public async Task<BussinessLayerResult<MessageListDto>> Add(MessageDto message)
        {
            var response = new BussinessLayerResult<MessageListDto>();
            try
            {
                var entity = Mapper.Map<MessageEntity>(message);
                entity.CreatedTime = DateTime.Now;
                entity.IsDeleted = false;
                entity.Id = 0;

                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.MessageMessageAddValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<MessageListDto>(Repository.Add(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.MessageMessageAddExceptionError, ex.Message);
            }
            return response;

        }

        public async Task<BussinessLayerResult<MessageListDto>> Delete(long id)
        {
            var response = new BussinessLayerResult<MessageListDto>();
            try
            {
                Repository.SoftDelete(id);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.MessageMessageDeleteExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<MessageListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<MessageListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<MessageListDto>(entity);
                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.MessageMessageGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<GenericLoadMoreDto<MessageListDto>>> GetAll(LoadMoreFilter<MessageFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<MessageListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                (string.IsNullOrEmpty(filter.Filter.Message) || x.Message.Contains(filter.Filter.Message))
                && (filter.Filter.ChatId == null || filter.Filter.ChatId == x.ChatId)
                && (filter.Filter.SenderUserId == null || filter.Filter.SenderUserId == x.SenderUserId)
                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex = firstIndex + filter.ContentCount;

                lastIndex = Math.Min(lastIndex, entities.Count);
                var values = new List<MessageListDto>();
                for (int i = firstIndex; i < lastIndex; i++)
                {
                    values.Add(Mapper.Map<MessageListDto>(entities[i]));
                }

                response.Result = new GenericLoadMoreDto<MessageListDto>
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
                response.AddError(Dtos.Enums.ErrorMessageCode.MessageMessageGetAllExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<MessageListDto>> Update(MessageDto message)
        {
            var response = new BussinessLayerResult<MessageListDto>();
            try
            {
                var entity = Repository.Get(message.Id);
                entity.ChatId = message.ChatId;
                entity.SenderUserId = message.SenderUserId;
                entity.Message = message.Message;





                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.MessageMessageUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<MessageListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.MessageMessageUpdateExceptionError, ex.Message);
            }
            return response;
        }
    }
}

