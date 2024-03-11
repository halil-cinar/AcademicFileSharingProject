using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    public class UserDeviceDto:DtoBase
    {
        public long UserId { get; set; }
        public EDeviceType DeviceType { get; set; }
        public string ConnectionId { get; set; }
    }
}
