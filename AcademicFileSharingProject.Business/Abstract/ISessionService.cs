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
    public interface ISessionService
    {
        public Task<BussinessLayerResult<SessionListDto>> Add(SessionDto session);
        public Task<BussinessLayerResult<SessionListDto>> Delete(long id);
        public Task<BussinessLayerResult<SessionListDto>> Update(SessionDto session);
        public Task<BussinessLayerResult<SessionListDto>> Get(long id);
        public Task<BussinessLayerResult<SessionListDto>> Get(string key);
        public Task<BussinessLayerResult<GenericLoadMoreDto<SessionListDto>>> GetAll(LoadMoreFilter<SessionFilter> filter);

    }
}
