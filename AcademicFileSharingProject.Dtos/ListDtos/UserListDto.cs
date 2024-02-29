using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.ListDtos
{
    
    public class UserListDto:DtoBase
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        
        public string PhoneNumber { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; }

        public long? ProfileImageId { get; set; }
        
        public MediaListDto? ProfileImage { get; set; }

        public string FullName { get
            {
                return Name + " " + Surname;
            }
        }
        public  List<UserRoleEntity> UserRoles { get; set; }



    }
}
