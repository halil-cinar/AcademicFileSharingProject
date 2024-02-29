using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    public class IdentityUpdateDto : DtoBase
    {
        public long UserId { get; set; }
        public string Password { get; set; }
        public string OldPassword { get; set; }





    }
}
