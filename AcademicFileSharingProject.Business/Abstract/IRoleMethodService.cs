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
    public interface IRoleMethodService
    {
        public Task<BussinessLayerResult<RoleMethodListDto>> Add(RoleMethodDto roleMethod);
        public Task<BussinessLayerResult<RoleMethodListDto>> Delete(long id);
        public Task<BussinessLayerResult<RoleMethodListDto>> Update(RoleMethodDto roleMethod);
        public Task<BussinessLayerResult<RoleMethodListDto>> Get(long id);
        public Task<BussinessLayerResult<GenericLoadMoreDto<RoleMethodListDto>>> GetAll(LoadMoreFilter<RoleMethodFilter> filter);
        public Task<BussinessLayerResult<RoleMethodListDto>> UpdateAll(RoleMethodAllUpdateDto roleMethod);

    }
}
