using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.Filters;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Dtos.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Business.Abstract
{
    public interface IUserDeviceService
    {
        public Task<BussinessLayerResult<UserDeviceListDto>> Add(UserDeviceDto userDevice);
        public Task<BussinessLayerResult<UserDeviceListDto>> Delete(long id);
        public Task<BussinessLayerResult<UserDeviceListDto>> Update(UserDeviceDto userDevice);
        public Task<BussinessLayerResult<UserDeviceListDto>> Get(long id);
        public Task<BussinessLayerResult<GenericLoadMoreDto<UserDeviceListDto>>> GetAll(LoadMoreFilter<UserDeviceFilter> filter);

    }
}
