using AcademicFileSharingProject.Dtos.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    public class SoftwareDto:DtoBase
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public IFormFile File { get; set; }
        public IFormFile? Document { get; set; }
        public IFormFile? Logo { get; set; }

        public long UserId { get; set; }
        public bool IsAir { get; set; }

    }
}
