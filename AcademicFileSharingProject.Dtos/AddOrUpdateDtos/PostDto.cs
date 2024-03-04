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
    public class PostDto:DtoBase
    {
        public long UserId { get; set; }
        public string Content { get; set; }

        public IFormFile PostImage { get; set; }
        public IFormFile PostVideo { get; set; }

        public List<IFormFile> Files { get; set; }
        public bool IsAir { get; set; }

       

        

    }
}
