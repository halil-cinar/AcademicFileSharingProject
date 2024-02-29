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
    public class PostCommentManager : ServiceBase<PostCommentEntity>, IPostCommentService
    {
        private readonly IMediaService _mediaService;
        public PostCommentManager(IEntityRepository<PostCommentEntity> repository, IMapper mapper, BaseEntityValidator<PostCommentEntity> validator, IMediaService mediaService) : base(repository, mapper, validator)
        {
            _mediaService = mediaService;
        }

        public async Task<BussinessLayerResult<PostCommentListDto>> Add(PostCommentDto comment)
        {
            var response = new BussinessLayerResult<PostCommentListDto>();
            try
            {
                var entity = Mapper.Map<PostCommentEntity>(comment);
                entity.CreatedTime = DateTime.Now;
                entity.IsDeleted = false;
                entity.Id = 0;

                if (comment.Media != null)
                {
                    //Media Ekleme/Güncelleme
                    var mediaResult =
                         await _mediaService.Add(new MediaDto
                         {
                             File = comment.Media,
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
                        response.AddError(Dtos.Enums.ErrorMessageCode.PostCommentPostCommentAddValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<PostCommentListDto>(Repository.Add(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostCommentPostCommentAddExceptionError, ex.Message);
            }
            return response;

        }

        public async Task<BussinessLayerResult<PostCommentListDto>> Delete(long id)
        {
            var response = new BussinessLayerResult<PostCommentListDto>();
            try
            {
                Repository.SoftDelete(id);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostCommentPostCommentDeleteExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<PostCommentListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<PostCommentListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<PostCommentListDto>(entity);
                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostCommentPostCommentGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<GenericLoadMoreDto<PostCommentListDto>>> GetAll(LoadMoreFilter<PostCommentFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<PostCommentListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                (string.IsNullOrEmpty(filter.Filter.Content) || x.Content.Contains(filter.Filter.Content))
                && (filter.Filter.PostId == null || filter.Filter.PostId == x.PostId)
                && (filter.Filter.SenderUserId == null || filter.Filter.SenderUserId == x.SenderUserId)
                && (filter.Filter.SharedUserId == null || filter.Filter.SharedUserId == x.Post.UserId)

                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex=firstIndex+ filter.ContentCount;

                lastIndex=Math.Min(lastIndex, entities.Count);
                var values = new List<PostCommentListDto>();
                for (int i = firstIndex ; i < lastIndex; i++)
                {
                    values.Add(Mapper.Map<PostCommentListDto>(entities[i]));
                }

                response.Result = new GenericLoadMoreDto<PostCommentListDto>
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
                response.AddError(Dtos.Enums.ErrorMessageCode.PostCommentPostCommentGetAllExceptionError, ex.Message);
            }
            return response;
        }

		public async Task<BussinessLayerResult<PostCommentListDto>> Update(PostCommentDto comment)
        {
            var response = new BussinessLayerResult<PostCommentListDto>();
            try
            {
                var entity = Repository.Get(comment.Id);
                entity.SenderUserId = comment.SenderUserId; 
                entity.PostId = comment.PostId;
                entity.Content= comment.Content;
                //entity.MediaId= comment.MediaId;
                if (comment.Media != null)
                {
                    //Media Ekleme/Güncelleme
                    var mediaResult = (entity.MediaId == null)
                        ? await _mediaService.Add(new MediaDto
                        {
                            File = comment.Media,
                        })
                        : await _mediaService.Update(new MediaDto
                        {
                            File = comment.Media,
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
                    if(entity.MediaId!= null)
                    {
                        var result =await _mediaService.Delete((long)entity.MediaId);
                        if (result.ResultStatus == Dtos.Enums.ResultStatus.Error)
                        {
                            response.ErrorMessages.AddRange(result.ErrorMessages);
                            return response;
                        }
                    }
                    entity.MediaId= null;
                }


                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.PostCommentPostCommentUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<PostCommentListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostCommentPostCommentUpdateExceptionError, ex.Message);
            }
            return response;
        }


    }
}

