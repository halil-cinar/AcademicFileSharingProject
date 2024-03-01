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
	public interface INotificationService
	{
		public Task<BussinessLayerResult<NotificationListDto>> Add(NotificationDto notification);
		public Task<BussinessLayerResult<NotificationListDto>> Delete(long id);
		public Task<BussinessLayerResult<NotificationListDto>> Update(NotificationDto notification);
		public Task<BussinessLayerResult<NotificationListDto>> Get(long id);
		public Task<BussinessLayerResult<GenericLoadMoreDto<NotificationListDto>>> GetAll(LoadMoreFilter<NotificationFilter> filter);


		public Task<BussinessLayerResult<NotificationListDto>> NotifyUserOnEmail(NotificationDto notification, string message);
		public Task<BussinessLayerResult<NotificationListDto>> NotifyUser(NotificationDto notification, string message);

	}
}
