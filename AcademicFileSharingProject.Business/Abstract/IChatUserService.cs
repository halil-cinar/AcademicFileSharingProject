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
    public interface IChatUserService
    {
        public Task<BussinessLayerResult<ChatUserListDto>> Add(ChatUserDto chatuser);
        public Task<BussinessLayerResult<ChatUserListDto>> Delete(long id);
        public Task<BussinessLayerResult<ChatUserListDto>> Update(ChatUserDto chatuser);
        public Task<BussinessLayerResult<ChatUserListDto>> Get(long id);
        public Task<BussinessLayerResult<GenericLoadMoreDto<ChatUserListDto>>> GetAll(LoadMoreFilter<ChatUserFilter> filter);



    }
}

