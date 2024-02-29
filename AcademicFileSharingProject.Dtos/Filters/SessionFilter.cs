using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
    public class SessionFilter
    {
        public long? UserId { get; set; }
        public string? Key { get; set; }
        public string? IpAddress { get; set; }

        public DateTime? MaxExpiryDate { get; set; }
        public EDeviceType? DeviceType { get; set; }
    }
}
