using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
    public class MediaFilter 
    {
        public string? FileName { get; set; }
        public string? ContentType { get; set; }

        public string? FileUrl { get; set; }

    }
}
