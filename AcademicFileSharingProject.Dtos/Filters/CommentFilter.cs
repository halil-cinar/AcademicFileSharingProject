using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
    public class PostCommentFilter 
    {
        public long? PostId { get; set; }
        public long? SenderUserId { get; set; }
        public long? SharedUserId { get; set; }
        public string? Content { get; set; }

        

       
    }
}
