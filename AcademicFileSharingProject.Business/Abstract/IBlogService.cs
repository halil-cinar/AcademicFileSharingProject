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
    public interface IBlogService
    {
		public Task<BussinessLayerResult<BlogListDto>> Add(BlogDto blog);
		public Task<BussinessLayerResult<BlogListDto>> Delete(long id);
		public Task<BussinessLayerResult<BlogListDto>> Update(BlogDto blog);
		public Task<BussinessLayerResult<BlogListDto>> Get(long id);
		public Task<BussinessLayerResult<GenericLoadMoreDto<BlogListDto>>> GetAll(LoadMoreFilter<BlogFilter> filter);

		public  Task<BussinessLayerResult<BlogListDto>> ChangePhoto(BlogDto blog);

    }
}
