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
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AcademicFileSharingProject.Business
{
    public class PostManager : ServiceBase<PostEntity>, IPostService
    {
        private readonly IPostMediaService _postMediaService;
        private readonly IMediaService _mediaService;
        private readonly ISubscribeService _subscribeService;
        public PostManager(IEntityRepository<PostEntity> repository, IMapper mapper, BaseEntityValidator<PostEntity> validator, IPostMediaService postMediaService, IMediaService mediaService, ISubscribeService subscribeService) : base(repository, mapper, validator)
        {
            _postMediaService = postMediaService;
            _mediaService = mediaService;
            _subscribeService = subscribeService;
        }

        public async Task<BussinessLayerResult<PostListDto>> Add(PostDto post)
        {
            var response = new BussinessLayerResult<PostListDto>();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {


                try

                {
                    var entity = Mapper.Map<PostEntity>(post);
                    entity.CreatedTime = DateTime.Now;
                    entity.IsDeleted = false;
                    entity.Id = 0;
                    if (post.PostImage != null)
                    {
                        var mediaresult = await _mediaService.Add(new MediaDto
                        {
                            File = post.PostImage
                        });
                        if (mediaresult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                        {
                            scope.Dispose();
                            response.ErrorMessages.AddRange(mediaresult.ErrorMessages);
                            return response;

                        }
                        entity.PostImageId = mediaresult.Result;
                    }
                    if (post.PostVideo != null)
                    {
                        var mediaresult = await _mediaService.Add(new MediaDto
                        {
                            File = post.PostVideo
                        });
                        if (mediaresult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                        {
                            scope.Dispose();
                            response.ErrorMessages.AddRange(mediaresult.ErrorMessages);
                            return response;

                        }
                        entity.PostVideoId = mediaresult.Result;
                    }



                    var validationResult = Validator.Validate(entity);
                    if (!validationResult.IsValid)
                    {   
                        scope.Dispose();
                        foreach (var item in validationResult.Errors)
                        {
                            
                            response.AddError(Dtos.Enums.ErrorMessageCode.PostPostAddValidationError, item.ErrorMessage);

                        }
                        return response;
                    }

                    response.Result = Mapper.Map<PostListDto>(Repository.Add(entity));


                    //Files
                    foreach (var file in post.Files??new List<IFormFile>())
                    {
                        var result = await _postMediaService.Add(new PostMediaDto
                        {
                            Media = file,
                            PostId = entity.Id
                        });
                        if (result.ResultStatus == Dtos.Enums.ResultStatus.Error)
                        {
                            scope.Dispose();
                            response.ErrorMessages.AddRange(result.ErrorMessages);
                            return response;
                        }
                    }

                    //Todo: Bildirim eklenecek

                    //await _subscribeService.NotifySubscribes(entity.UserId,
                    //   "Sayýn %user_name% %user_surname% \n %subscribeduser_name% %subscribeduser_surname% yeni bir gönderi paylaþmýþtýr",
                    //   "Yeni Bir Bildiriminiz Var", Entities.Enums.EEntityType.Post, entity.Id);
                    scope.Complete();


                }

                catch (Exception ex)
                {
                    scope.Dispose();
                    response.AddError(Dtos.Enums.ErrorMessageCode.PostPostAddExceptionError, ex.Message);
                }
            }
           

            return response;

        }

        public async Task<BussinessLayerResult<PostListDto>> Delete(long id)
        {
            var response = new BussinessLayerResult<PostListDto>();
            try
            {
                Repository.SoftDelete(id);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostPostDeleteExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<PostListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<PostListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<PostListDto>(entity);
                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostPostGetExceptionError, ex.Message);
            }
            return response;
        }
        
        public async Task<BussinessLayerResult<GenericLoadMoreDto<PostListDto>>> GetAll(LoadMoreFilter<PostFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<PostListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                (string.IsNullOrEmpty(filter.Filter.Content) || x.Content.Contains(filter.Filter.Content))
                && (filter.Filter.IsAir == null || filter.Filter.IsAir == x.IsAir)
                && (filter.Filter.UserId == null || filter.Filter.UserId == x.UserId)
                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex = firstIndex + filter.ContentCount;

                lastIndex = Math.Min(lastIndex, entities.Count);
                var values = new List<PostListDto>();
                for (int i = firstIndex; i < lastIndex; i++)
                {
                    values.Add(Mapper.Map<PostListDto>(entities[i]));
                }

                response.Result = new GenericLoadMoreDto<PostListDto>
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
                response.AddError(Dtos.Enums.ErrorMessageCode.PostPostGetAllExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<PostListDto>> Update(PostDto post)
        {
            var response = new BussinessLayerResult<PostListDto>();
            try
            {
                var entity = Repository.Get(post.Id);

                entity.UserId = post.UserId;
                entity.IsAir = post.IsAir;
                entity.Content = post.Content;




                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.PostPostUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<PostListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostPostUpdateExceptionError, ex.Message);
            }
            return response;
        }


        public async Task<BussinessLayerResult<PostListDto>> ChangePhoto(PostDto post)
        {
            var response = new BussinessLayerResult<PostListDto>();
            try
            {
                var entity = Repository.Get(post.Id);

                //Media Güncelleme
                var mediaResult = (entity.PostImageId != null)
                    ? await _mediaService.Update(new MediaDto
                    {
                        Id= (long)entity.PostImageId,
                        File = post.PostImage,
                    })
                    : await _mediaService.Add(new MediaDto
                    {
                        File = post.PostImage,
                    });

                if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                {
                    response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                    return response;

                }

                entity.PostImageId = (long)mediaResult.Result;





                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.PostPostUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<PostListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostPostUpdateExceptionError, ex.Message);
            }
            return response;
        }

         public async Task<BussinessLayerResult<PostListDto>> ChangeVideo(PostDto post)
        {
            var response = new BussinessLayerResult<PostListDto>();
            try
            {
                var entity = Repository.Get(post.Id);

                //Media Güncelleme
                var mediaResult = (entity.PostVideoId != null)
                    ? await _mediaService.Update(new MediaDto
                    {
                        Id= (long)entity.PostVideoId,
                        File = post.PostVideo,
                    })
                    : await _mediaService.Add(new MediaDto
                    {
                        File = post.PostVideo,
                    });

                if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                {
                    response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                    return response;

                }

                entity.PostVideoId = (long)mediaResult.Result;





                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.PostPostUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<PostListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.PostPostUpdateExceptionError, ex.Message);
            }
            return response;
        }


        public async Task<BussinessLayerResult<PostListDto>> ChangeFiles(PostDto post)
        {
            var response = new BussinessLayerResult<PostListDto>();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var entity = Repository.Get(post.Id);

                    var oldFiles = await _postMediaService.GetAll(new LoadMoreFilter<PostMediaFilter>
                    {
                        ContentCount = int.MaxValue,
                        PageCount = 0,
                        Filter = new PostMediaFilter
                        {
                            PostId = post.Id,
                        }
                    });
                    if (oldFiles.ResultStatus == Dtos.Enums.ResultStatus.Error)
                    {
                        scope.Dispose();
                        response.ErrorMessages.AddRange(oldFiles.ErrorMessages);
                        return response;    
                    }

                    var newFiles = post.Files;
                    var diffirentnewFiles = new List<IFormFile>();
                    var diffirentoldFiles = new List<PostMediaListDto>();
                    var oldFilesNamelist=oldFiles.Result.Values.Select(x=>x.Media.FileName).ToList();
                    var newFilesNameList = newFiles.Select(x => x.FileName).ToList();
                    foreach (var file in newFiles)
                    {
                        if (!oldFilesNamelist.Contains(file.FileName))
                        {
                            diffirentnewFiles.Add(file);
                        }

                    }


                    foreach (var item in oldFiles.Result.Values)
                    {
                        if (!newFilesNameList.Contains(item.Media.FileName))
                        {
                            diffirentoldFiles.Add(item);
                        }
                    }

                    foreach (var file in diffirentoldFiles)
                    {
                        var result = await _postMediaService.Delete(file.Id);
                        if (result.ResultStatus == Dtos.Enums.ResultStatus.Error)
                        {
                            scope.Dispose();
                            response.ErrorMessages.AddRange(result.ErrorMessages);
                            return response;
                        }
                    }

                    foreach (var file in diffirentnewFiles)
                    {
                        var result = await _postMediaService.Add(new PostMediaDto
                        {
                            Media= file,
                            PostId=post.Id
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
                    response.AddError(Dtos.Enums.ErrorMessageCode.PostPostUpdateExceptionError, ex.Message);
                }
            }
            return response;
        }




    }
}

