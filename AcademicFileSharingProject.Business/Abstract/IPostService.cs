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
    public interface IPostService
    {
        public Task<BussinessLayerResult<PostListDto>> Add(PostDto post);
        public Task<BussinessLayerResult<PostListDto>> Delete(long id);
        public Task<BussinessLayerResult<PostListDto>> Update(PostDto post);
        public Task<BussinessLayerResult<PostListDto>> Get(long id);
        public Task<BussinessLayerResult<GenericLoadMoreDto<PostListDto>>> GetAll(LoadMoreFilter<PostFilter> filter);
        public Task<BussinessLayerResult<PostListDto>> ChangeFiles(PostDto post);
        public Task<BussinessLayerResult<PostListDto>> ChangeMedia(PostDto post);


    }
}

