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
    [Table("UserRole")]
    public class UserRoleEntity:EntityBase
    {
        public long UserId { get; set; }
        public ERoles Role { get; set; }


        [ForeignKey("UserId")]
        public UserEntity User { get; set; }
    }

}
