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
using System.Xml.Linq;

namespace AcademicFileSharingProject.Business
{
	public class BlogManager : ServiceBase<BlogEntity>, IBlogService
	{
		private readonly IMediaService _mediaService;
        public BlogManager(IEntityRepository<BlogEntity> repository, IMapper mapper, BaseEntityValidator<BlogEntity> validator, IMediaService mediaService) : base(repository, mapper, validator)
        {
            _mediaService = mediaService;
        }

        public async Task<BussinessLayerResult<BlogListDto>> Add(BlogDto blog)
		{
			var response = new BussinessLayerResult<BlogListDto>();
			try
			{
				var entity = Mapper.Map<BlogEntity>(blog);
				entity.CreatedTime = DateTime.Now;
				entity.IsDeleted = false;
				entity.Id = 0;
				entity.ReleaseDate = DateTime.Now;


                if (blog.Media != null)
                {
                    //Media Ekleme
                    var mediaResult =
                         await _mediaService.Add(new MediaDto
                         {
                             File = blog.Media,
                         })
                        ;

                    if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                    {
                        response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                        return response;

                    }

                    entity.MediaId = (long)mediaResult.Result;

                }



                var validationResult = Validator.Validate(entity);
				if (!validationResult.IsValid)
				{
					foreach (var item in validationResult.Errors)
					{
						response.AddError(Dtos.Enums.ErrorMessageCode.BlogBlogAddValidationError, item.ErrorMessage);

					}
					return response;
				}

				response.Result = Mapper.Map<BlogListDto>(Repository.Add(entity));

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.BlogBlogAddExceptionError, ex.Message);
			}
			return response;

		}

		public async Task<BussinessLayerResult<BlogListDto>> Delete(long id)
		{
			var response = new BussinessLayerResult<BlogListDto>();
			try
			{
				Repository.SoftDelete(id);

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.BlogBlogDeleteExceptionError, ex.Message);
			}
			return response;
		}

		public async Task<BussinessLayerResult<BlogListDto>> Get(long id)
		{
			var response = new BussinessLayerResult<BlogListDto>();
			try
			{
				var entity = Repository.Get(id);
				var dto = Mapper.Map<BlogListDto>(entity);
				response.Result = dto;

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.BlogBlogGetExceptionError, ex.Message);
			}
			return response;
		}

		public async Task<BussinessLayerResult<GenericLoadMoreDto<BlogListDto>>> GetAll(LoadMoreFilter<BlogFilter> filter)
		{
			var response = new BussinessLayerResult<GenericLoadMoreDto<BlogListDto>>();
			try
			{
				var entities = (filter.Filter != null) ?
					Repository.GetAll(x =>
				(string.IsNullOrEmpty(filter.Filter.Title) || x.Title.Contains(filter.Filter.Title))
				&&(string.IsNullOrEmpty(filter.Filter.Content) || x.Content.Contains(filter.Filter.Content))
				&& (filter.Filter.UserId == null || filter.Filter.UserId == x.UserId)
				&& (filter.Filter.IsAir == null || filter.Filter.IsAir == x.IsAir)
				&& (x.IsDeleted == false)
				) : Repository.GetAll(x => x.IsDeleted == false);

				entities=entities.OrderBy(x=>x.Id*-1).ToList();

				var firstIndex = filter.PageCount * filter.ContentCount;
				var lastIndex = firstIndex + filter.ContentCount;

				lastIndex = Math.Min(lastIndex, entities.Count);
				var values = new List<BlogListDto>();
				for (int i = firstIndex; i < lastIndex; i++)
				{
					values.Add(Mapper.Map<BlogListDto>(entities[i]));
				}

				response.Result = new GenericLoadMoreDto<BlogListDto>
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
				response.AddError(Dtos.Enums.ErrorMessageCode.BlogBlogGetAllExceptionError, ex.Message);
			}
			return response;
		}

		public async Task<BussinessLayerResult<BlogListDto>> Update(BlogDto blog)
		{
			var response = new BussinessLayerResult<BlogListDto>();
			try
			{
				var entity = Repository.Get(blog.Id);
				entity.Title = blog.Title;
				entity.Content = blog.Content;
				entity.IsAir = blog.IsAir;
			




				var validationResult = Validator.Validate(entity);
				if (!validationResult.IsValid)
				{
					foreach (var item in validationResult.Errors)
					{
						response.AddError(Dtos.Enums.ErrorMessageCode.BlogBlogUpdateValidationError, item.ErrorMessage);

					}
					return response;
				}

				response.Result = Mapper.Map<BlogListDto>(Repository.Update(entity));

			}
			catch (Exception ex)
			{
				response.AddError(Dtos.Enums.ErrorMessageCode.BlogBlogUpdateExceptionError, ex.Message);
			}
			return response;
		}


        public async Task<BussinessLayerResult<BlogListDto>> ChangePhoto(BlogDto blog)
        {
            var response = new BussinessLayerResult<BlogListDto>();
            try
            {
                var entity = Repository.Get(blog.Id);


                if (blog.Media != null)
                {
                    //Media Ekleme/Güncelleme
                    var mediaResult = (entity.MediaId == null)
                        ? await _mediaService.Add(new MediaDto
						{
                            File = blog.Media,
                        })
                        : await _mediaService.Update(new MediaDto
						{
                            File = blog.Media,
                            Id = (long)entity.MediaId
                        });

                    if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                    {
                        response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                        return response;

                    }

                    entity.MediaId = (long)mediaResult.Result;
                }
                else
                {
                    if (entity.MediaId != null)
                    {
                        var result = await _mediaService.Delete((long)entity.MediaId);
                        if (result.ResultStatus == Dtos.Enums.ResultStatus.Error)
                        {
                            response.ErrorMessages.AddRange(result.ErrorMessages);
                            return response;
                        }
                    }
                    entity.MediaId = null;
                }


                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.BlogBlogUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<BlogListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.BlogBlogUpdateExceptionError, ex.Message);
            }
            return response;
        }


    }
}
