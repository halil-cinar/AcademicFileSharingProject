using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    public class PostMediaDownloadDto:DtoBase
    {
        public long PostMediaId { get; set; }
        public long UserId { get; set; }
    }
}
