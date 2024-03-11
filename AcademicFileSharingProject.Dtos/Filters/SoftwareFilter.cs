using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
    public class SoftwareFilter
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

       
        public bool? HasDocument { get; set; }
        public bool? HasLogo { get; set; }

        public long? UserId { get; set; }
        public bool? IsAir { get; set; }
        public string? Search { get; set; }

    }
}
