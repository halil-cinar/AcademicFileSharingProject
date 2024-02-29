using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.ListDtos
{
    
    public class IdentityListDto:DtoBase
    {
        public long UserId { get; set; }
       
        

        
        public UserListDto User { get; set; }

    }
}
