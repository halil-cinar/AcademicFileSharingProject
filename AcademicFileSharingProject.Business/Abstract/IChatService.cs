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
    public interface IChatService
    {
        public Task<BussinessLayerResult<ChatListDto>> Add(ChatDto chat);
        public Task<BussinessLayerResult<ChatListDto>> Delete(long id);
        public Task<BussinessLayerResult<ChatListDto>> Update(ChatDto chat);
        public Task<BussinessLayerResult<ChatListDto>> Get(long id);
        public Task<BussinessLayerResult<GenericLoadMoreDto<ChatListDto>>> GetAll(LoadMoreFilter<ChatFilter> filter);



    }
}
