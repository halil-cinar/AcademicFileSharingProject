using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Core.DataAccess;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.Enums;
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
    public class MediaManager : ServiceBase<MediaEntity>, IMediaService
    {
        public MediaManager(IEntityRepository<MediaEntity> repository, IMapper mapper, BaseEntityValidator<MediaEntity> validator) : base(repository, mapper, validator)
        {
        }




        string path = "wwwroot/Medias";

        public async Task<BussinessLayerResult<long?>> Add(MediaDto media)
        {
            var response = new BussinessLayerResult<long?>();
            try
            {

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(media.File.FileName);
                var filePath = Path.Combine(path, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    await media.File.CopyToAsync(fs);
                }

                var entity = new MediaEntity
                {
                    FileName = media.File.FileName,
                    ContentType = media.File.ContentType,
                    FileUrl = filePath,
                    CreatedTime= DateTime.Now,
                    IsDeleted = false
                };
                var validationResult = Validator.Validate(entity);
                if (validationResult.IsValid)
                {
                    Repository.Add(entity);
                    response.Result = entity.Id;
                }
                else
                {
                    response.Result = null;
                    foreach (var err in validationResult.Errors)
                    {
                        response.AddError(ErrorMessageCode.MediaMediaAddValidationError, err.ErrorMessage);
                    }


                }

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.AddError(ErrorMessageCode.MediaMediaAddExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<bool>> Delete(long id)
        {
            var response = new BussinessLayerResult<bool>();
            try
            {
                var entity = Repository.Get(id);
                if (entity != null)
                {
                    entity.IsDeleted = true;
                    Repository.Update(entity);
                    response.Result = true;

                }
                else
                {
                    response.Result = false;
                    response.AddError(ErrorMessageCode.MediaMediaDeleteItemNotFoundError, "");
                }

            }
            catch (Exception ex)
            {
                response.Result = false;
                response.AddError(ErrorMessageCode.MediaMediaDeleteExceptionError,
                                  ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<long?>> Update(MediaDto media)
        {
            var response = new BussinessLayerResult<long?>();
            try
            {
                var entity = Repository.Get(media.Id);
                if (entity == null)
                {
                    response.Result = null;
                    response.AddError(ErrorMessageCode.MediaMediaUpdateItemNotFoundError, "");
                    return response;
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }


                using (var fs = new FileStream(entity.FileUrl, FileMode.Truncate))
                {
                    await media.File.CopyToAsync(fs);
                }
                entity.ContentType = media.File.ContentType;
                entity.FileName = media.File.FileName;

                var validationResult = Validator.Validate(entity);
                if (validationResult.IsValid)
                {
                    Repository.Update(entity);
                    response.Result = entity.Id;
                }
                else
                {
                    response.Result = null;
                    foreach (var err in validationResult.Errors)
                    {
                        response.AddError(ErrorMessageCode.MediaMediaUpdateValidationError, err.ErrorMessage);
                    }


                }

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.AddError(ErrorMessageCode.MediaMediaUpdateExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<MediaListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<MediaListDto>();
            try
            {
                var entity = Repository.Get(id);
                if (entity == null)
                {
                    response.AddError(ErrorMessageCode.MediaMediaGetItemNotFoundError, "");

                }
                else
                {

                    var dto = new MediaListDto
                    {
                        Id = id,
                        FileName = entity.FileName,
                        ContentType = entity.ContentType,
                        File = await File.ReadAllBytesAsync(entity.FileUrl)
                    };
                    response.Result = dto;

                }

            }
            catch (Exception ex)
            {
                response.Result = null;
                response.AddError(ErrorMessageCode.MediaMediaGetExceptionError, ex.Message);
            }
            return response;

        }


  }
}

