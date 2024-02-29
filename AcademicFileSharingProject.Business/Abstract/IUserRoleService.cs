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
    public interface IUserRoleService
    {
        public Task<BussinessLayerResult<UserRoleListDto>> Add(UserRoleDto userrole);
        public Task<BussinessLayerResult<UserRoleListDto>> Delete(long id);
        public Task<BussinessLayerResult<UserRoleListDto>> Update(UserRoleDto userrole);
        public Task<BussinessLayerResult<UserRoleListDto>> Get(long id);
        public Task<BussinessLayerResult<GenericLoadMoreDto<UserRoleListDto>>> GetAll(LoadMoreFilter<UserRoleFilter> filter);
        public Task<BussinessLayerResult<UserRoleListDto>> UpdateAll(UserRoleAllUpdateDto userrole);



    }
}

