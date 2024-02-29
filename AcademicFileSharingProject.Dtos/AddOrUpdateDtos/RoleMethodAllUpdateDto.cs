using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    public class RoleMethodAllUpdateDto
    {
        public ERoles Role { get; set; }
        public List<EMethod> Methods { get; set; }
    }
}
