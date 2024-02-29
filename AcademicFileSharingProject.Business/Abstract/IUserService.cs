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
    public interface IUserService
    {
        public Task<BussinessLayerResult<UserListDto>> Add(UserAddDto user);
        public Task<BussinessLayerResult<UserListDto>> Delete(long id);
        public Task<BussinessLayerResult<UserListDto>> Update(UserDto user);
        public Task<BussinessLayerResult<UserListDto>> Get(long id);
        public Task<BussinessLayerResult<GenericLoadMoreDto<UserListDto>>> GetAll(LoadMoreFilter<UserFilter> filter);
        public  Task<BussinessLayerResult<UserListDto>> ChangeProfilePhoto(UserDto user);
        public  Task<BussinessLayerResult<List<UserListDto>>> SearchUser(string query);

    }
}

