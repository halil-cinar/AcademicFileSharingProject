using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Dtos.Enums;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    public class UserRoleDto:DtoBase
    {
        public long UserId { get; set; }
        public ERoles Role { get; set; }


    }

}
