using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
    public class PostMediaDownloadFilter
    {
        public long? PostMediaId { get; set; }
        public long? UserId { get; set; }
    }
}
