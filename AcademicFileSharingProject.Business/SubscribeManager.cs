using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Core.DataAccess;
using AcademicFileSharingProject.Core.ExtensionsMethods;
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

namespace AcademicFileSharingProject.Business
{
	public class SubscribeManager : ServiceBase<SubscribeEntity>, ISubscribeService
	{
		public SubscribeManager(IEntityRepository<SubscribeEntity> repository, IMapper mapper, BaseEntityValidator<SubscribeEntity> validator) : base(repository, mapper, validator)
		{
		}

		public async Task<BussinessLayerResult<SubscribeListDto>> Add(SubscribeDto subscribe)
		{
			var response = new BussinessLayerResult<SubscribeListDto>();
			try
			{
				var entity = Mapper.Map<SubscribeEntity>(subscribe);
				entity.CreatedTime = DateTime.Now;
				entity.IsDeleted = false;
				entity.Id = 0;

				var validationResult = Validator.Validate(entity);
				if (!validationResult.IsValid)
				{
					foreach (var item in validationResult.Errors)
					{
						response.AddError(Dtos.Enums.ErrorMessageCode.SubscribeSubscribeAddValidationError, item.ErrorMessage);

					}
					return response;
				}

				response.Result = Mapper.Map<SubscribeListDto>(Repository.Add(entity));

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.SubscribeSubscribeAddExceptionError, ex.Message);
			}
			return response;

		}

		public async Task<BussinessLayerResult<SubscribeListDto>> Delete(long id)
		{
			var response = new BussinessLayerResult<SubscribeListDto>();
			try
			{
				Repository.SoftDelete(id);

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.SubscribeSubscribeDeleteExceptionError, ex.Message);
			}
			return response;
		}

		public async Task<BussinessLayerResult<SubscribeListDto>> Get(long id)
		{
			var response = new BussinessLayerResult<SubscribeListDto>();
			try
			{
				var entity = Repository.Get(id);
				var dto = Mapper.Map<SubscribeListDto>(entity);
				response.Result = dto;

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.SubscribeSubscribeGetExceptionError, ex.Message);
			}
			return response;
		}

		public async Task<BussinessLayerResult<GenericLoadMoreDto<SubscribeListDto>>> GetAll(LoadMoreFilter<SubscribeFilter> filter)
		{
			var response = new BussinessLayerResult<GenericLoadMoreDto<SubscribeListDto>>();
			try
			{
				var entities = (filter.Filter != null) ?
					Repository.GetAll(x =>
				(string.IsNullOrEmpty(filter.Filter.Email) || x.Email.Contains(filter.Filter.Email))
				&& (filter.Filter.SubscribeUserID == null || filter.Filter.SubscribeUserID == x.SubscribeUserID)
				&& (filter.Filter.UserID == null || filter.Filter.UserID == x.UserID)
				&& (x.IsDeleted == false)
				) : Repository.GetAll(x => x.IsDeleted == false);

				var firstIndex = filter.PageCount * filter.ContentCount;
				var lastIndex = firstIndex + filter.ContentCount;

				lastIndex = Math.Min(lastIndex, entities.Count);
				var values = new List<SubscribeListDto>();
				for (int i = firstIndex; i < lastIndex; i++)
				{
					values.Add(Mapper.Map<SubscribeListDto>(entities[i]));
				}

				response.Result = new GenericLoadMoreDto<SubscribeListDto>
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
				response.AddError(Dtos.Enums.ErrorMessageCode.SubscribeSubscribeGetAllExceptionError, ex.Message);
			}
			return response;
		}
		/// <summary>
		/// %user_name%  ifadesi bizim için kullanıcının ismine  
		/// %user_surname%  ifadesi bizim için kullanıcının soyismine  
		/// %subscribeduser_name% ifadesi bizim için abone olunan kullanıcının ismine 
		/// %subscribeduser_surname% ifadesi bizim için abone olunan kullanıcının soyismine Denk gelmektedir
		/// </summary>
		/// <param name="subscribedUserId"></param>
		/// <param name="message"></param>
		/// <param name="subject"></param>
		/// <returns></returns>
		public async Task<BussinessLayerResult<bool>> NotifySubscribes(long subscribedUserId,string message,string subject)
		{
			// %user_name%  ifadesi bizim için kullanıcının ismine  
			// %user_surname%  ifadesi bizim için kullanıcının soyismine  
			// %subscribeduser_name% ifadesi bizim için abone olunan kullanıcının ismine 
			// %subscribeduser_surname% ifadesi bizim için abone olunan kullanıcının soyismine 
			var response = new BussinessLayerResult<bool>();
			try { 
			var entities= Repository.GetAll(x=>x.SubscribeUserID==subscribedUserId&&x.IsDeleted==false);

				foreach (var item in entities)
				{
					if (item.User != null) {
						message = message.Replace("%user_name%", item.User.Name);
						message = message.Replace("%user_surname%", item.User.Surname);
						subject = subject.Replace("%user_name%", item.User.Name);
						subject = subject.Replace("%user_surname%", item.User.Surname);
					}
					else
					{
						message = message.Replace("%user_name%", "Kullanıcı");
						message = message.Replace("%user_surname%", "");
						subject = subject.Replace("%user_name%", "Kullanıcı");
						subject = subject.Replace("%user_surname%", "");
					}

					message = message.Replace("%subscribeduser_name%", item.SubcsribeUser.Name);
					message = message.Replace("%subscribeduser_surname%", item.SubcsribeUser.Surname);
					subject = subject.Replace("%subscribeduser_name%", item.SubcsribeUser.Name);
					subject = subject.Replace("%subscribeduser_surname%", item.SubcsribeUser.Surname);

					new MailSender(new SmtpValues
					{
						//Todo: Smtp değerleri girilecek
					}).SendEmail(subject, message, false, (item.User == null) ? item.Email : item.User.Email);
				}

				response.Result = true;


			}
			catch(Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.SubscribeSubscribeNotifyExceptionError, ex.Message);
			}

			return response;

		}

		public async Task<BussinessLayerResult<SubscribeListDto>> Update(SubscribeDto subscribe)
		{
			var response = new BussinessLayerResult<SubscribeListDto>();
			try
			{
				var entity = Repository.Get(subscribe.Id);
				entity.Email = subscribe.Email;
				entity.SubscribeUserID = subscribe.SubscribeUserID;
				entity.UserID = subscribe.SubscribeUserID;



				var validationResult = Validator.Validate(entity);
				if (!validationResult.IsValid)
				{
					foreach (var item in validationResult.Errors)
					{
						response.AddError(Dtos.Enums.ErrorMessageCode.SubscribeSubscribeUpdateValidationError, item.ErrorMessage);

					}
					return response;
				}

				response.Result = Mapper.Map<SubscribeListDto>(Repository.Update(entity));

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.SubscribeSubscribeUpdateExceptionError, ex.Message);
			}
			return response;
		}
	}
}
