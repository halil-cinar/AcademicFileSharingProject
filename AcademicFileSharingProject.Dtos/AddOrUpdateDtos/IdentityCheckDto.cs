using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    public class IdentityCheckDto:DtoBase
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public EDeviceType DeviceType { get; set; }
        public bool RememberMe { get; set; }
    }
}
