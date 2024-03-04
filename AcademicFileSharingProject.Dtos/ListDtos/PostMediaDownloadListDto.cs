using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.ListDtos
{
    public class PostMediaDownloadListDto:DtoBase
    {
        public long PostMediaId { get; set; }
        public long UserId { get; set; }

        public PostMediaListDto PostMedia { get; set; }

        public UserListDto User { get; set; }
    }
}
