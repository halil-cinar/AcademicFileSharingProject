using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
    [Table("Media")]
    public class MediaEntity:EntityBase
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }

        public string FileUrl { get; set; }

    }
}
