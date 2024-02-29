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
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Business
{
	public class BlogCommentManager : ServiceBase<BlogCommentEntity>, IBlogCommentService
	{
		public BlogCommentManager(IEntityRepository<BlogCommentEntity> repository, IMapper mapper, BaseEntityValidator<BlogCommentEntity> validator) : base(repository, mapper, validator)
		{
		}

		public async Task<BussinessLayerResult<BlogCommentListDto>> Add(BlogCommentDto comment)
		{
			var response = new BussinessLayerResult<BlogCommentListDto>();
			try
			{
				var entity = Mapper.Map<BlogCommentEntity>(comment);
				entity.CreatedTime = DateTime.Now;
				entity.IsDeleted = false;
				entity.Id = 0;

				var validationResult = Validator.Validate(entity);
				if (!validationResult.IsValid)
				{
					foreach (var item in validationResult.Errors)
					{
						response.AddError(Dtos.Enums.ErrorMessageCode.BlogCommentBlogCommentAddValidationError, item.ErrorMessage);

					}
					return response;
				}

				response.Result = Mapper.Map<BlogCommentListDto>(Repository.Add(entity));

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.BlogCommentBlogCommentAddExceptionError, ex.Message);
			}
			return response;

		}

		public async Task<BussinessLayerResult<BlogCommentListDto>> Delete(long id)
		{
			var response = new BussinessLayerResult<BlogCommentListDto>();
			try
			{
				Repository.SoftDelete(id);

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.BlogCommentBlogCommentDeleteExceptionError, ex.Message);
			}
			return response;
		}

		public async Task<BussinessLayerResult<BlogCommentListDto>> Get(long id)
		{
			var response = new BussinessLayerResult<BlogCommentListDto>();
			try
			{
				var entity = Repository.Get(id);
				var dto = Mapper.Map<BlogCommentListDto>(entity);
				response.Result = dto;

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.BlogCommentBlogCommentGetExceptionError, ex.Message);
			}
			return response;
		}

		public async Task<BussinessLayerResult<GenericLoadMoreDto<BlogCommentListDto>>> GetAll(LoadMoreFilter<BlogCommentFilter> filter)
		{
			var response = new BussinessLayerResult<GenericLoadMoreDto<BlogCommentListDto>>();
			try
			{
				var entities = (filter.Filter != null) ?
					Repository.GetAll(x =>
				(string.IsNullOrEmpty(filter.Filter.Content) || x.Content.Contains(filter.Filter.Content))
				&& (filter.Filter.BlogId == null || filter.Filter.BlogId == x.BlogId)
				&& (filter.Filter.BloggerId == null || filter.Filter.BloggerId == x.Blog.UserId)
				&& (filter.Filter.SenderUserId == null || filter.Filter.SenderUserId == x.SenderUserId)

				&& (x.IsDeleted == false)
				) : Repository.GetAll(x => x.IsDeleted == false);

				var firstIndex = filter.PageCount * filter.ContentCount;
				var lastIndex = firstIndex + filter.ContentCount;

				lastIndex = Math.Min(lastIndex, entities.Count);
				var values = new List<BlogCommentListDto>();
				for (int i = firstIndex; i < lastIndex; i++)
				{
					values.Add(Mapper.Map<BlogCommentListDto>(entities[i]));
				}

				response.Result = new GenericLoadMoreDto<BlogCommentListDto>
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
				response.AddError(Dtos.Enums.ErrorMessageCode.BlogCommentBlogCommentGetAllExceptionError, ex.Message);
			}
			return response;
		}


		public async Task<BussinessLayerResult<BlogCommentListDto>> Update(BlogCommentDto comment)
		{
			var response = new BussinessLayerResult<BlogCommentListDto>();
			try
			{
				var entity = Repository.Get(comment.Id);
				entity.SenderUserId = comment.SenderUserId;
				entity.BlogId = comment.BlogId;
				entity.Content = comment.Content;



				var validationResult = Validator.Validate(entity);
				if (!validationResult.IsValid)
				{
					foreach (var item in validationResult.Errors)
					{
						response.AddError(Dtos.Enums.ErrorMessageCode.BlogCommentBlogCommentUpdateValidationError, item.ErrorMessage);

					}
					return response;
				}

				response.Result = Mapper.Map<BlogCommentListDto>(Repository.Update(entity));

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.BlogCommentBlogCommentUpdateExceptionError, ex.Message);
			}
			return response;
		}
	}
}
