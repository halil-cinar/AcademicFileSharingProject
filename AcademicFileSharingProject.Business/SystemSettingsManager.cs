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
using AcademicFileSharingProject.Entities.Enums;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Business
{
    public class SystemSettingsManager : ServiceBase<SystemSettingsEntity>, ISystemSettingsService
    {
        private readonly IMediaService _mediaService;
        public SystemSettingsManager(IEntityRepository<SystemSettingsEntity> repository, IMapper mapper, BaseEntityValidator<SystemSettingsEntity> validator, IMediaService mediaService) : base(repository, mapper, validator)
        {
            _mediaService = mediaService;
        }

        public async Task<BussinessLayerResult<SystemSettingsListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<SystemSettingsListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<SystemSettingsListDto>(entity);



                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.SystemSettingsSystemSettingsGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<SystemSettingsListDto>> Get(ESystemSetting key)
        {
            var response = new BussinessLayerResult<SystemSettingsListDto>();
            try
            {
                var entity = Repository.Get(x=>x.Key==key);
                var dto = Mapper.Map<SystemSettingsListDto>(entity);



                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.SystemSettingsSystemSettingsGetExceptionError, ex.Message);
            }
            return response;
        }



        public async Task<BussinessLayerResult<SmtpValues>> GetSmtp()
        {
            var response = new BussinessLayerResult<SmtpValues>();
            try
            {
                var smtp = new SmtpValues
                {
                    SmtpDisplayAddress = Repository.Get(x => x.Key == ESystemSetting.SmtpDisplayAddress).Value,
                    SmtpDisplayName = Repository.Get(x => x.Key == ESystemSetting.SmtpDisplayName).Value,
                    SmtpEnableSsl = Convert.ToBoolean(Repository.Get(x => x.Key == ESystemSetting.SmtpEnableSsl).Value),
                    SmtpPassword = Repository.Get(x => x.Key == ESystemSetting.SmtpPassword).Value,
                    SmtpPort = Convert.ToInt32(Repository.Get(x => x.Key == ESystemSetting.SmtpPort).Value),
                    SmtpServer = Repository.Get(x => x.Key == ESystemSetting.SmtpServer).Value,
                    SmtpUsername = Repository.Get(x => x.Key == ESystemSetting.SmtpUsername).Value

                };


                response.Result = smtp;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.SystemSettingsSystemSettingsGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<SystemSettingsListDto>> GetLogo()
        {
            var response = new BussinessLayerResult<SystemSettingsListDto>();
            try
            {
                var entity = Repository.Get(x=>x.Key==ESystemSetting.Logo);
                var dto = Mapper.Map<SystemSettingsListDto>(entity);

                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.SystemSettingsSystemSettingsGetExceptionError, ex.Message);
            }
            return response;
        }
        public async Task<BussinessLayerResult<bool>> ChangeLogo(LogoDto logo)
        {
            var response = new BussinessLayerResult<bool>();
            try
            {
                var entity = Repository.Get(logo.Id);
                var mediaResult = (string.IsNullOrEmpty(entity.Value))
                    ? await _mediaService.Add(new MediaDto { File = logo.File })
                    : await _mediaService.Update(new MediaDto { File = logo.File, Id = Convert.ToInt64(entity.Value) });
                if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                {
                    response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                    return response;
                }

                entity.Value = mediaResult.Result.ToString();
                response.Result = true;
                Repository.Update(entity);
            }
            catch (Exception ex)
            {
                response.Result = false;
                response.AddError(Dtos.Enums.ErrorMessageCode.SystemSettingsSystemSettingsChangeLogoExceptionError, ex.Message);

            }
            return response;
        }


        public async Task<BussinessLayerResult<GenericLoadMoreDto<SystemSettingsListDto>>> GetAll(LoadMoreFilter<SystemSettingsFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<SystemSettingsListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                (string.IsNullOrEmpty(filter.Filter.Name) || x.Name.Contains(filter.Filter.Name))
                && (filter.Filter.Key == null || filter.Filter.Key == x.Key)
                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex = firstIndex + filter.ContentCount;

                lastIndex = Math.Min(lastIndex, entities.Count);
                var values = new List<SystemSettingsListDto>();
                for (int i = firstIndex; i < lastIndex; i++)
                {
                    values.Add(Mapper.Map<SystemSettingsListDto>(entities[i]));
                }

                response.Result = new GenericLoadMoreDto<SystemSettingsListDto>
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
                response.AddError(Dtos.Enums.ErrorMessageCode.SystemSettingsSystemSettingsGetAllExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<SystemSettingsListDto>> Update(SystemSettingsDto setting)
        {
            var response = new BussinessLayerResult<SystemSettingsListDto>();
            try
            {
                var entity = Repository.Get(setting.Id);
                entity.Value = setting.Value;
                entity.Name = setting.Name;




                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.SystemSettingsSystemSettingsUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<SystemSettingsListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.SystemSettingsSystemSettingsUpdateExceptionError, ex.Message);
            }
            return response;
        }



    }
}
