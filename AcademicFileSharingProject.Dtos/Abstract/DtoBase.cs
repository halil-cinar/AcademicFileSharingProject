using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Abstract
{
    public abstract class DtoBase
    {
        public long Id { get; set; }

        public DateTime CreatedTime { get; set; }

        public bool IsDeleted { get; set; }
    }
}
