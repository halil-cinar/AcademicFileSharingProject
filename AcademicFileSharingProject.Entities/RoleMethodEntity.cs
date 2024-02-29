using AcademicFileSharingProject.Entities.Abstract;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
    [Table("RoleMethod")]
    public class RoleMethodEntity:EntityBase
    {
        public ERoles Role { get; set; }
        public EMethod Method { get; set; }
    }
}
