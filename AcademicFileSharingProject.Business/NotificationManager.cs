using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Core.DataAccess;
using AcademicFileSharingProject.Core.ExtensionsMethods;
using AcademicFileSharingProject.Core.WebUI;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.Filters;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Dtos.Result;
using AcademicFileSharingProject.Entities;
using AcademicFileSharingProject.Entities.Abstract;
using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AcademicFileSharingProject.Business
{
	public class NotificationManager : ServiceBase<NotificationEntity>, INotificationService
	{
        private readonly INotificationHub _notificationHub;
		private readonly ISystemSettingsService _systemSettingsService;
        public NotificationManager(IEntityRepository<NotificationEntity> repository, IMapper mapper, BaseEntityValidator<NotificationEntity> validator, INotificationHub notificationHub, ISystemSettingsService systemSettingsService) : base(repository, mapper, validator)
        {
            this._notificationHub = notificationHub;
            _systemSettingsService = systemSettingsService;
        }

        public async Task<BussinessLayerResult<NotificationListDto>> Add(NotificationDto notification)
		{
			var response = new BussinessLayerResult<NotificationListDto>();
			try
			{
				var entity = Mapper.Map<NotificationEntity>(notification);

				entity.CreatedTime = DateTime.Now;
				entity.IsDeleted = false;
				entity.Id = 0;

				var validationResult = Validator.Validate(entity);
				if (!validationResult.IsValid)
				{
					foreach (var item in validationResult.Errors)
					{
						response.AddError(Dtos.Enums.ErrorMessageCode.NotificationNotificationAddValidationError, item.ErrorMessage);

					}
					return response;
				}

				response.Result = Mapper.Map<NotificationListDto>(Repository.Add(entity));

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.NotificationNotificationAddExceptionError, ex.Message);
			}
			return response;

		}

		public async Task<BussinessLayerResult<NotificationListDto>> Delete(long id)
		{
			var response = new BussinessLayerResult<NotificationListDto>();
			try
			{
				Repository.SoftDelete(id);

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.NotificationNotificationDeleteExceptionError, ex.Message);
			}
			return response;
		}

		public async Task<BussinessLayerResult<NotificationListDto>> Get(long id)
		{
			var response = new BussinessLayerResult<NotificationListDto>();
			try
			{
				var entity = Repository.Get(id);
				var dto = Mapper.Map<NotificationListDto>(entity);
				response.Result = dto;

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.NotificationNotificationGetExceptionError, ex.Message);
			}
			return response;
		}


		public async Task<BussinessLayerResult<NotificationListDto>> NotifyUserOnEmail(NotificationDto notification,string message)
		{
			var response = new BussinessLayerResult<NotificationListDto>();
			using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
			{
				try
				{
					var addResult = await Add(notification);
					if (addResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
					{
						scope.Dispose();
						response.ErrorMessages.AddRange(addResult.ErrorMessages);
						return response;
					}
					
					

					// %user_name%  ifadesi bizim için kullanıcının ismine  
					// %user_surname%  ifadesi bizim için kullanıcının soyismine  
					message = message.Replace("%user_name%", addResult.Result.User.Name);
					message = message.Replace("%user_surname%", addResult.Result.User.Surname);

					var smtpResult =await _systemSettingsService.GetSmtp();
					if(smtpResult.ResultStatus==Dtos.Enums.ResultStatus.Success)
					{
					new MailSender(smtpResult.Result).SendEmail(notification.Title, message,true, addResult.Result.User.Email, addResult.Result.User.Email2);

					}

					response.Result = addResult.Result;

					scope.Complete();
				}
				catch (Exception ex)
				{
					scope.Dispose();
					response.AddError(Dtos.Enums.ErrorMessageCode.NotificationNotificationGetExceptionError, ex.Message);
				}
			}
			return response;
		}

        public async Task<BussinessLayerResult<NotificationListDto>> NotifyUser(NotificationDto notification, string message)
        {
            var response = new BussinessLayerResult<NotificationListDto>();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var addResult = await Add(notification);
                    if (addResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                    {
                        scope.Dispose();
                        response.ErrorMessages.AddRange(addResult.ErrorMessages);
                        return response;
                    }



                    // %user_name%  ifadesi bizim için kullanıcının ismine  
                    // %user_surname%  ifadesi bizim için kullanıcının soyismine  
                    message = message.Replace("%user_name%", addResult.Result.User.Name);
                    message = message.Replace("%user_surname%", addResult.Result.User.Surname);

                   await _notificationHub.SendNotification(addResult.Result.UserId, message);

                    response.Result = addResult.Result;

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    response.AddError(Dtos.Enums.ErrorMessageCode.NotificationNotificationGetExceptionError, ex.Message);
                }
            }
            return response;
        }



        public async Task<BussinessLayerResult<GenericLoadMoreDto<NotificationListDto>>> GetAll(LoadMoreFilter<NotificationFilter> filter)
		{
			var response = new BussinessLayerResult<GenericLoadMoreDto<NotificationListDto>>();
			try
			{
				var entities = (filter.Filter != null) ?
					Repository.GetAll(x =>
				(string.IsNullOrEmpty(filter.Filter.Title) || x.Title.Contains(filter.Filter.Title))
				&& (filter.Filter.EntityType == null || filter.Filter.EntityType == x.EntityType)
				&& (filter.Filter.EntityId == null || filter.Filter.EntityId == x.EntityId)
				&& (filter.Filter.UserId == null || filter.Filter.UserId == x.UserId)
				&& (x.IsDeleted == false)
				) : Repository.GetAll(x => x.IsDeleted == false);

				var firstIndex = filter.PageCount * filter.ContentCount;
				var lastIndex = firstIndex + filter.ContentCount;

				lastIndex = Math.Min(lastIndex, entities.Count);
				var values = new List<NotificationListDto>();
				for (int i = firstIndex; i < lastIndex; i++)
				{
					values.Add(Mapper.Map<NotificationListDto>(entities[i]));
				}

				response.Result = new GenericLoadMoreDto<NotificationListDto>
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
				response.AddError(Dtos.Enums.ErrorMessageCode.NotificationNotificationGetAllExceptionError, ex.Message);
			}
			return response;
		}

		public async Task<BussinessLayerResult<NotificationListDto>> Update(NotificationDto notification)
		{
			var response = new BussinessLayerResult<NotificationListDto>();
			try
			{
				var entity = Repository.Get(notification.Id);
				entity.Title = notification.Title;
				entity.EntityId = notification.EntityId;
				entity.EntityType = notification.EntityType;	
				entity.UserId= notification.UserId;	


				var validationResult = Validator.Validate(entity);
				if (!validationResult.IsValid)
				{
					foreach (var item in validationResult.Errors)
					{
						response.AddError(Dtos.Enums.ErrorMessageCode.NotificationNotificationUpdateValidationError, item.ErrorMessage);

					}
					return response;
				}

				response.Result = Mapper.Map<NotificationListDto>(Repository.Update(entity));

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.NotificationNotificationUpdateExceptionError, ex.Message);
			}
			return response;
		}


	}
}
