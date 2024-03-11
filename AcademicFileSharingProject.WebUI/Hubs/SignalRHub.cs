using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Core.ExtensionsMethods;
using AcademicFileSharingProject.Core.WebUI;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Hubs
{
    public class SignalRHub : INotificationHub
    {
        private readonly IHubContext<SignalRHub> _hubContext;

        private readonly IUserDeviceService _userDeviceService;


        public SignalRHub(IHubContext<SignalRHub> hubContext, IUserDeviceService userDeviceService)
        {
            _hubContext = hubContext;
            _userDeviceService = userDeviceService;
        }


        public async Task<string> Register(long userId)
        {
            var result = await _userDeviceService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.UserDeviceFilter>
            {
                ContentCount = int.MaxValue,
                PageCount = 0,
                Filter = new Dtos.Filters.UserDeviceFilter
                {
                    UserId = userId,
                    DeviceType = Entities.Enums.EDeviceType.Web
                }
            });
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                var devices = result.Result.Values.OrderBy(x => x.CreatedTime).ToList();

                if (devices.Count != 0 && devices != null)
                {
                    var updateResult = await _userDeviceService.Update(new UserDeviceDto
                    {
                        UserId = userId,
                        ConnectionId = Context.ConnectionId,
                        CreatedTime = DateTime.Now,
                        DeviceType = Entities.Enums.EDeviceType.Web,
                        Id = devices.First().Id
                    });
                    //SendNotification(userId, "kaydınız alınmıştır");

                }
                else
                {
                    var addResult = await _userDeviceService.Add(new UserDeviceDto
                    {
                        UserId = userId,
                        ConnectionId = Context.ConnectionId,
                        CreatedTime = DateTime.Now,
                        DeviceType = Entities.Enums.EDeviceType.Web,

                    });
                }


            }
            return Context.ConnectionId;
        }

        public async Task<string> RegisterService(long userId)
        {
            var result = await _userDeviceService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.UserDeviceFilter>
            {
                ContentCount = int.MaxValue,
                PageCount = 0,
                Filter = new Dtos.Filters.UserDeviceFilter
                {
                    UserId = userId,
                    DeviceType = Entities.Enums.EDeviceType.WebService
                }
            });
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                var devices = result.Result.Values.OrderBy(x => x.CreatedTime).ToList();

                if (devices.Count != 0 && devices != null)
                {
                    var updateResult = await _userDeviceService.Update(new UserDeviceDto
                    {
                        UserId = userId,
                        ConnectionId = Context.ConnectionId,
                        CreatedTime = DateTime.Now,
                        DeviceType = Entities.Enums.EDeviceType.WebService,
                        Id = devices.First().Id
                    });
                    //SendNotification(userId, "service kaydınız alınmıştır");

                }
                else
                {
                    var addResult = await _userDeviceService.Add(new UserDeviceDto
                    {
                        UserId = userId,
                        ConnectionId = Context.ConnectionId,
                        CreatedTime = DateTime.Now,
                        DeviceType = Entities.Enums.EDeviceType.WebService,

                    });
                    //SendNotification(userId, "service kaydınız alınmıştır");
                }


            }
            return Context.ConnectionId;
        }




        public override async Task SendNotification(long userId, string message)
        {

            var result = await _userDeviceService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.UserDeviceFilter>
            {
                ContentCount = int.MaxValue,
                PageCount = 0,
                Filter = new Dtos.Filters.UserDeviceFilter
                {
                    UserId = userId,
                    
                }
            });



            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                var devices = result.Result.Values.OrderBy(x => x.CreatedTime).ToList();

                if (devices.Count != 0 && devices != null)
                {
                    var device = devices.First();
                    await _hubContext.Clients.Client(device.ConnectionId).SendAsync("ReceiveNotification", message);
                    // açık olmayan cihazlar
                    if (devices.Count > 1)
                    {
                        var service = devices.Last();
                        await _hubContext.Clients.Client(service.ConnectionId).SendAsync("ReceiveNotification", message);
                    }
                }


            }
        }


    }
}
