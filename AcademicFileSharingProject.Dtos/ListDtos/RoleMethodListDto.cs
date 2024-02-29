using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.ListDtos
{
    public class RoleMethodListDto:DtoBase
    {
        public ERoles Role { get; set; }
        public EMethod Method { get; set; }
    }
}
