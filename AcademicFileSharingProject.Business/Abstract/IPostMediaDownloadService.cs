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
    public interface IPostMediaDownloadService
    {
        public Task<BussinessLayerResult<PostMediaDownloadListDto>> Add(PostMediaDownloadDto postmedia);
        public Task<BussinessLayerResult<List<long>>> AddByPost(PostMediaDownloadDto postmedia); //return postmedia ıds
        public Task<BussinessLayerResult<PostMediaDownloadListDto>> Delete(long id);
        public Task<BussinessLayerResult<PostMediaDownloadListDto>> Update(PostMediaDownloadDto postmedia);
        public Task<BussinessLayerResult<PostMediaDownloadListDto>> Get(long id);
        public Task<BussinessLayerResult<GenericLoadMoreDto<PostMediaDownloadListDto>>> GetAll(LoadMoreFilter<PostMediaDownloadFilter> filter);

    }
}
