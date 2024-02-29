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
    public interface IIdentityService
    {
        public Task<BussinessLayerResult<IdentityListDto>> Add(IdentityDto identity);
       
        public Task<BussinessLayerResult<IdentityListDto>> Update(IdentityUpdateDto identity);
        public Task<BussinessLayerResult<IdentityListDto>> CheckPassword(IdentityCheckDto identity)
            ;
        public Task<BussinessLayerResult<IdentityListDto>> ForgatPassword(long userId);






    }
}

