using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
    [Table("User")]
    public class UserEntity:EntityBase
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        
        public string PhoneNumber { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; }

        public long? ProfileImageId { get; set; }
        [ForeignKey(nameof(ProfileImageId))]
        public MediaEntity? ProfileImage { get; set; }

        public virtual ICollection<UserRoleEntity> UserRoles { get; set; }


    }
}
