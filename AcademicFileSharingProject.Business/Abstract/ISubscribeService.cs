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
	public interface ISubscribeService
	{
		public Task<BussinessLayerResult<SubscribeListDto>> Add(SubscribeDto subscribe);
		public Task<BussinessLayerResult<SubscribeListDto>> Delete(long id);
		public Task<BussinessLayerResult<SubscribeListDto>> Update(SubscribeDto subscribe);
		public Task<BussinessLayerResult<SubscribeListDto>> Get(long id);
		public Task<BussinessLayerResult<bool>> NotifySubscribes(long subscribedUserId,string message,string subject);
		public Task<BussinessLayerResult<GenericLoadMoreDto<SubscribeListDto>>> GetAll(LoadMoreFilter<SubscribeFilter> filter);






	}
}
