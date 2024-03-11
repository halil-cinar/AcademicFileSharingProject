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
    public class SoftwareManager : ServiceBase<SoftwareEntity>, ISoftwareService
    {
        private readonly IMediaService _mediaService;
        public SoftwareManager(IEntityRepository<SoftwareEntity> repository, IMapper mapper, BaseEntityValidator<SoftwareEntity> validator, IMediaService mediaService) : base(repository, mapper, validator)
        {
            _mediaService = mediaService;
        }

        public async Task<BussinessLayerResult<SoftwareListDto>> Add(SoftwareDto software)
        {
            var response = new BussinessLayerResult<SoftwareListDto>();
            try
            {
                var entity = Mapper.Map<SoftwareEntity>(software);
                entity.CreatedTime = DateTime.Now;
                entity.IsDeleted = false;
                entity.Id = 0;


                if (software.File != null)
                {
                    //dosya Ekleme
                    var mediaResult =
                         await _mediaService.Add(new MediaDto
                         {
                             File = software.File,
                         })
                        ;

                    if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                    {
                        response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                        return response;

                    }

                    entity.FileId = (long)mediaResult.Result;

                }
                if (software.Logo != null)
                {
                    //Logo Ekleme
                    var mediaResult =
                         await _mediaService.Add(new MediaDto
                         {
                             File = software.Logo,
                         })
                        ;

                    if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                    {
                        response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                        return response;

                    }

                    entity.LogoId = (long)mediaResult.Result;

                }
                if (software.Document != null)
                {
                    //döküman Ekleme
                    var mediaResult =
                         await _mediaService.Add(new MediaDto
                         {
                             File = software.Document,
                         })
                        ;

                    if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                    {
                        response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                        return response;

                    }

                    entity.DocumentId = (long)mediaResult.Result;

                }



                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareAddValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<SoftwareListDto>(Repository.Add(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareAddExceptionError, ex.Message);
            }
            return response;

        }

        public async Task<BussinessLayerResult<SoftwareListDto>> Delete(long id)
        {
            var response = new BussinessLayerResult<SoftwareListDto>();
            try
            {
                Repository.SoftDelete(id);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareDeleteExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<SoftwareListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<SoftwareListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<SoftwareListDto>(entity);
                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<GenericLoadMoreDto<SoftwareListDto>>> GetAll(LoadMoreFilter<SoftwareFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<SoftwareListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                (string.IsNullOrEmpty(filter.Filter.Name) || x.Name.Contains(filter.Filter.Name))
                && (string.IsNullOrEmpty(filter.Filter.Description) || x.Description.Contains(filter.Filter.Description))
                && (filter.Filter.UserId == null || filter.Filter.UserId == x.UserId)
                && (filter.Filter.IsAir == null || filter.Filter.IsAir == x.IsAir)
                && (filter.Filter.HasDocument == null || filter.Filter.HasDocument == (x.DocumentId!=null))
                && (filter.Filter.HasLogo == null || filter.Filter.HasLogo == (x.LogoId!=null))
                && (string.IsNullOrEmpty(filter.Filter.Search) || (x.Name+" "+x.Description+" "+x.User.Name+" "+x.User.Surname).ToLower().Contains(filter.Filter.Search.ToLower()))
                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);

                entities = entities.OrderBy(x => x.Id * -1).ToList();

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex = firstIndex + filter.ContentCount;

                lastIndex = Math.Min(lastIndex, entities.Count);
                var values = new List<SoftwareListDto>();
                for (int i = firstIndex; i < lastIndex; i++)
                {
                    values.Add(Mapper.Map<SoftwareListDto>(entities[i]));
                }

                response.Result = new GenericLoadMoreDto<SoftwareListDto>
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
                response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareGetAllExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<SoftwareListDto>> Update(SoftwareDto software)
        {
            var response = new BussinessLayerResult<SoftwareListDto>();
            try
            {
                var entity = Repository.Get(software.Id);
                entity.Description = software.Description;
                entity.Name = software.Name;
                entity.IsAir = software.IsAir;





                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<SoftwareListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareUpdateExceptionError, ex.Message);
            }
            return response;
        }


        public async Task<BussinessLayerResult<SoftwareListDto>> ChangeFile(SoftwareDto software)
        {
            var response = new BussinessLayerResult<SoftwareListDto>();
            try
            {
                var entity = Repository.Get(software.Id);


                if (software.File != null)
                {
                    //File Ekleme/Güncelleme
                    var mediaResult = (entity.FileId == null)
                        ? await _mediaService.Add(new MediaDto
                        {
                            File = software.File,
                        })
                        : await _mediaService.Update(new MediaDto
                        {
                            File = software.File,
                            Id = (long)entity.FileId
                        });

                    if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                    {
                        response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                        return response;

                    }

                    entity.FileId = (long)mediaResult.Result;
                }
                else
                {
                    response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareMediaUpdateValidationError, "Dosya boş olamaz");
                    return response;
                }


                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareMediaUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<SoftwareListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareMediaUpdateExceptionError, ex.Message);
            }
            return response;
        }
    public async Task<BussinessLayerResult<SoftwareListDto>> ChangeDocument(SoftwareDto software)
        {
            var response = new BussinessLayerResult<SoftwareListDto>();
            try
            {
                var entity = Repository.Get(software.Id);


                if (software.Document != null)
                {
                    //Document Ekleme/Güncelleme
                    var mediaResult = (entity.DocumentId == null)
                        ? await _mediaService.Add(new MediaDto
                        {
                            File = software.Document,
                        })
                        : await _mediaService.Update(new MediaDto
                        {
                            File = software.Document,
                            Id = (long)entity.DocumentId
                        });

                    if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                    {
                        response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                        return response;

                    }

                    entity.DocumentId = (long)mediaResult.Result;
                }
                else
                {
                    if (entity.DocumentId != null)
                    {
                        var result = await _mediaService.Delete((long)entity.DocumentId);
                        if (result.ResultStatus == Dtos.Enums.ResultStatus.Error)
                        {
                            response.ErrorMessages.AddRange(result.ErrorMessages);
                            return response;
                        }
                    }
                    entity.DocumentId = null;
                }


                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareMediaUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<SoftwareListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareMediaUpdateExceptionError, ex.Message);
            }
            return response;
        }
      public async Task<BussinessLayerResult<SoftwareListDto>> ChangeLogo(SoftwareDto software)
        {
            var response = new BussinessLayerResult<SoftwareListDto>();
            try
            {
                var entity = Repository.Get(software.Id);


                if (software.Logo != null)
                {
                    //Logo Ekleme/Güncelleme
                    var mediaResult = (entity.LogoId == null)
                        ? await _mediaService.Add(new MediaDto
                        {
                            File = software.Logo,
                        })
                        : await _mediaService.Update(new MediaDto
                        {
                            File = software.Logo,
                            Id = (long)entity.LogoId
                        });

                    if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                    {
                        response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                        return response;

                    }

                    entity.LogoId = (long)mediaResult.Result;
                }
                else
                {
                    if (entity.LogoId != null)
                    {
                        var result = await _mediaService.Delete((long)entity.LogoId);
                        if (result.ResultStatus == Dtos.Enums.ResultStatus.Error)
                        {
                            response.ErrorMessages.AddRange(result.ErrorMessages);
                            return response;
                        }
                    }
                    entity.LogoId = null;
                }


                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareMediaUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<SoftwareListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.SoftwareSoftwareMediaUpdateExceptionError, ex.Message);
            }
            return response;
        }
    
    }
}
