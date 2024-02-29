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
	public interface IBlogCommentService
	{
		public Task<BussinessLayerResult<BlogCommentListDto>> Add(BlogCommentDto comment);
		public Task<BussinessLayerResult<BlogCommentListDto>> Delete(long id);
		public Task<BussinessLayerResult<BlogCommentListDto>> Update(BlogCommentDto comment);
		public Task<BussinessLayerResult<BlogCommentListDto>> Get(long id);
		public Task<BussinessLayerResult<GenericLoadMoreDto<BlogCommentListDto>>> GetAll(LoadMoreFilter<BlogCommentFilter> filter);



	}
}
