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
    public interface IMessageService
    {
        public Task<BussinessLayerResult<MessageListDto>> Add(MessageDto message);
        public Task<BussinessLayerResult<MessageListDto>> Delete(long id);
        public Task<BussinessLayerResult<MessageListDto>> Update(MessageDto message);
        public Task<BussinessLayerResult<MessageListDto>> Get(long id);
        public Task<BussinessLayerResult<GenericLoadMoreDto<MessageListDto>>> GetAll(LoadMoreFilter<MessageFilter> filter);



    }
}

