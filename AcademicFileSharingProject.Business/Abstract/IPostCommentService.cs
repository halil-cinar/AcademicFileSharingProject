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
    public interface IPostCommentService
    {
        public Task<BussinessLayerResult<PostCommentListDto>> Add(PostCommentDto comment);
        public Task<BussinessLayerResult<PostCommentListDto>> Delete(long id);
        public Task<BussinessLayerResult<PostCommentListDto>> Update(PostCommentDto comment);
        public Task<BussinessLayerResult<PostCommentListDto>> Get(long id);
        public Task<BussinessLayerResult<GenericLoadMoreDto<PostCommentListDto>>> GetAll(LoadMoreFilter<PostCommentFilter> filter);



    }
}

