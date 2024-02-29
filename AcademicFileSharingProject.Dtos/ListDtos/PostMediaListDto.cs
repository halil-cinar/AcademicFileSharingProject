using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.ListDtos
{
    
    public class PostMediaListDto:DtoBase
    {
        public long PostId { get; set; }
        public long MediaId { get; set; }

        
        public MediaListDto Media { get; set; }

        
        public PostListDto Post { get; set; }

    }
}
