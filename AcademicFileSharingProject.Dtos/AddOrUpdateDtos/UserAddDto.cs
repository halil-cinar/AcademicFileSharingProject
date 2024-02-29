using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    public class UserAddDto:UserDto
    {
        public string Password { get; set; }

        public ERoles Role { get; set; }
 
    }
}
