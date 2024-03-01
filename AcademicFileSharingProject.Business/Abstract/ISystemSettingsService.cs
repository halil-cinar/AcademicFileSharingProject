using AcademicFileSharingProject.Core.ExtensionsMethods;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.Filters;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Dtos.Result;
using AcademicFileSharingProject.Entities.Enums;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Business.Abstract
{
    public interface ISystemSettingsService
    {
        public Task<BussinessLayerResult<SystemSettingsListDto>> Get(long id);
        public Task<BussinessLayerResult<SystemSettingsListDto>> Get(ESystemSetting key);

        public Task<BussinessLayerResult<SmtpValues>> GetSmtp();
        public  Task<BussinessLayerResult<SystemSettingsListDto>> GetLogo();
        public  Task<BussinessLayerResult<GenericLoadMoreDto<SystemSettingsListDto>>> GetAll(LoadMoreFilter<SystemSettingsFilter> filter);
        public Task<BussinessLayerResult<SystemSettingsListDto>> Update(SystemSettingsDto setting);
        public Task<BussinessLayerResult<bool>> ChangeLogo(LogoDto logo);


    }
}
