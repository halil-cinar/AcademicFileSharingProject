using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
    public class UserFilter 
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        
        public string? PhoneNumber { get; set; }

        public string? Title { get; set; }





    }
}
