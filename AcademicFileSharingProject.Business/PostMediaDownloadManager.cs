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
using System.Transactions;

namespace AcademicFileSharingProject.Business
{
    public class PostMediaDownloadManager : ServiceBase<PostMediaDownloadEntity>, IPostMediaDownloadService
    {
        private readonly IPostService _postService;
        public PostMediaDownloadManager(IEntityRepository<PostMediaDownloadEntity> repository, IMapper mapper, BaseEntityValidator<PostMediaDownloadEntity> validator, IPostService postService) : base(repository, mapper, validator)
        {
            _postService = postService;
        }


        public async Task<BussinessLayerResult<PostMediaDownloadListDto>> Add(PostMediaDownloadDto postMediaDownload)
        {
            var response = new BussinessLayerResult<PostMediaDownloadListDto>();
            try
            {
                var entity = Mapper.Map<PostMediaDownloadEntity>(postMediaDownload);
                entity.CreatedTime = DateTime.Now;
                entity.IsDeleted = false;
                entity.Id = 0;

                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaDownloadPostMediaDownloadAddValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<PostMediaDownloadListDto>(Repository.Add(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaDownloadPostMediaDownloadAddExceptionError, ex.Message);
            }
            return response;

        }
        public async Task<BussinessLayerResult<List<long>>> AddByPost(PostMediaDownloadDto postMediaDownload)
        {
            var response = new BussinessLayerResult<List<long>>();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var postResult = await _postService.Get(postMediaDownload.PostMediaId);
                    if (postResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                    {
                        scope.Dispose();
                        response.ErrorMessages.AddRange(postResult.ErrorMessages);
                        return response;
                    }
                    foreach (var item in postResult.Result.PostMedias)
                    {

                        var entity = new PostMediaDownloadEntity
                        {
                            CreatedTime = DateTime.Now,
                            IsDeleted = false,
                            PostMediaId = item.Id,
                            UserId = postMediaDownload.UserId,

                        };
                        var validationResult = Validator.Validate(entity);
                        if (!validationResult.IsValid)
                        {
                            scope.Dispose();
                            foreach (var err in validationResult.Errors)
                            {
                                response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaDownloadPostMediaDownloadAddValidationError, err.ErrorMessage);

                            }
                            return response;
                        }

                        Repository.Add(entity);
                    }
                    response.Result = postResult.Result.PostMedias.Select(x => x.Id).ToList();

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaDownloadPostMediaDownloadAddExceptionError, ex.Message);
                }
            }
            return response;

        }

        public async Task<BussinessLayerResult<PostMediaDownloadListDto>> Delete(long id)
        {
            var response = new BussinessLayerResult<PostMediaDownloadListDto>();
            try
            {
                Repository.SoftDelete(id);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaDownloadPostMediaDownloadDeleteExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<PostMediaDownloadListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<PostMediaDownloadListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<PostMediaDownloadListDto>(entity);
                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaDownloadPostMediaDownloadGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<GenericLoadMoreDto<PostMediaDownloadListDto>>> GetAll(LoadMoreFilter<PostMediaDownloadFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<PostMediaDownloadListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                 (filter.Filter.PostMediaId == null || filter.Filter.PostMediaId == x.PostMediaId)
                && (filter.Filter.UserId == null || filter.Filter.UserId == x.UserId)
                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex = firstIndex + filter.ContentCount;

                lastIndex = Math.Min(lastIndex, entities.Count);
                var values = new List<PostMediaDownloadListDto>();
                for (int i = firstIndex; i < lastIndex; i++)
                {
                    values.Add(Mapper.Map<PostMediaDownloadListDto>(entities[i]));
                }

                response.Result = new GenericLoadMoreDto<PostMediaDownloadListDto>
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
                response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaDownloadPostMediaDownloadGetAllExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<PostMediaDownloadListDto>> Update(PostMediaDownloadDto postMediaDownload)
        {
            var response = new BussinessLayerResult<PostMediaDownloadListDto>();
            try
            {
                var entity = Repository.Get(postMediaDownload.Id);
                entity.PostMediaId = postMediaDownload.PostMediaId;
                entity.UserId = postMediaDownload.UserId;




                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaDownloadPostMediaDownloadUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<PostMediaDownloadListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostMediaDownloadPostMediaDownloadUpdateExceptionError, ex.Message);
            }
            return response;
        }


    }
}
