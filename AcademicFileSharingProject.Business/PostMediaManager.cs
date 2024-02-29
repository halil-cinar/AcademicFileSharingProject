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
    public class PostMediaManager : ServiceBase<PostMediaEntity>, IPostMediaService
    {

        private readonly IMediaService _mediaService;

        public PostMediaManager(IEntityRepository<PostMediaEntity> repository, IMapper mapper, BaseEntityValidator<PostMediaEntity> validator, IMediaService mediaService) : base(repository, mapper, validator)
        {
            _mediaService = mediaService;
        }

        public async Task<BussinessLayerResult<PostMediaListDto>> Add(PostMediaDto postmedia)
        {
            var response = new BussinessLayerResult<PostMediaListDto>();
            try
            {
                var entity = Mapper.Map<PostMediaEntity>(postmedia);
                entity.CreatedTime = DateTime.Now;
                entity.IsDeleted = false;
                entity.Id = 0;

                //Media Ekleme
                var mediaResult = await _mediaService.Add(new MediaDto
                {
                    File = postmedia.Media,
                });

                if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                {
                    response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                    return response;    

                }

                entity.MediaId = (long)mediaResult.Result;



                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaPostMediaAddValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<PostMediaListDto>(Repository.Add(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaPostMediaAddExceptionError, ex.Message);
            }
            return response;

        }

        public async Task<BussinessLayerResult<PostMediaListDto>> Delete(long id)
        {
            var response = new BussinessLayerResult<PostMediaListDto>();
            try
            {
                Repository.SoftDelete(id);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaPostMediaDeleteExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<PostMediaListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<PostMediaListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<PostMediaListDto>(entity);
                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaPostMediaGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<GenericLoadMoreDto<PostMediaListDto>>> GetAll(LoadMoreFilter<PostMediaFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<PostMediaListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                (filter.Filter.MediaId == null || filter.Filter.MediaId == x.MediaId)
                && (filter.Filter.PostId == null || filter.Filter.PostId == x.PostId)
                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex=firstIndex+ filter.ContentCount;

                lastIndex=Math.Min(lastIndex, entities.Count);
                var values = new List<PostMediaListDto>();
                for (int i = firstIndex ; i < lastIndex; i++)
                {
                    values.Add(Mapper.Map<PostMediaListDto>(entities[i]));
                }

                response.Result = new GenericLoadMoreDto<PostMediaListDto>
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
                response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaPostMediaGetAllExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<PostMediaListDto>> Update(PostMediaDto postmedia)
        {
            var response = new BussinessLayerResult<PostMediaListDto>();
            try
            {
                var entity = Repository.Get(postmedia.Id);
                entity.PostId = postmedia.Id;
                
                var mediaResult = await _mediaService.Update(new MediaDto
                {
                    File = postmedia.Media,
                });

                if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                {
                    response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                    return response;

                }

                entity.MediaId = (long)mediaResult.Result;



                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaPostMediaUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<PostMediaListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaPostMediaUpdateExceptionError, ex.Message);
            }
            return response;
        }
    }
}

