using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
    public class IdentityFilter 
    {
        public long? UserId { get; set; }
        public string? PasswordSalt { get; set; }
        public string? PasswordHash { get; set; }

        public bool? IsValid { get; set; }

        

    }
}
