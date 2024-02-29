using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.ListDtos
{
    public class SessionListDto : DtoBase
    {
        public long UserId { get; set; }
        public string Key { get; set; }
        public string IpAddress { get; set; }

        public DateTime? ExpiryDate { get; set; }
        public EDeviceType DeviceType { get; set; }


        public UserListDto User { get; set; }

    }
}
