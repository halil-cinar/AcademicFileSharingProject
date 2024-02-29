using AcademicFileSharingProject.Dtos.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    public class UserDto:DtoBase
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Email2 { get; set; }
        
        public string PhoneNumber { get; set; }

        public string Title { get; set; }
        public string? Description { get; set; }

        public IFormFile ProfileImage { get; set; }
       



    }
}
