using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
    public class RoleMethodFilter
    {
        public ERoles? Role { get; set; }
        public EMethod? Method { get; set; }
    }
}
