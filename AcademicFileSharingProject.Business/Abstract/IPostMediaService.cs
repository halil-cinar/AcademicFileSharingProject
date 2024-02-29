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
    public interface IPostMediaService
    {
        public Task<BussinessLayerResult<PostMediaListDto>> Add(PostMediaDto postmedia);
        public Task<BussinessLayerResult<PostMediaListDto>> Delete(long id);
        public Task<BussinessLayerResult<PostMediaListDto>> Update(PostMediaDto postmedia);
        public Task<BussinessLayerResult<PostMediaListDto>> Get(long id);
        public Task<BussinessLayerResult<GenericLoadMoreDto<PostMediaListDto>>> GetAll(LoadMoreFilter<PostMediaFilter> filter);



    }
}

