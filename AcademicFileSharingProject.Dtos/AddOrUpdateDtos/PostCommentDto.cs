using AcademicFileSharingProject.Dtos.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    public class PostCommentDto:DtoBase
    {
        public long PostId { get; set; }
        public long SenderUserId { get; set; }
        public string Content { get; set; }

        public IFormFile? Media { get; set; }

       
    }
}
