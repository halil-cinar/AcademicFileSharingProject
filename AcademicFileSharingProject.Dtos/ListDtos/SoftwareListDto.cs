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
    public class SoftwareListDto:DtoBase
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public long FileId { get; set; }
        public long? DocumentId { get; set; }
        public long? LogoId { get; set; }

        public long UserId { get; set; }
        public bool IsAir { get; set; }


        
        public MediaListDto File { get; set; }

        public UserListDto User { get; set; }
        public MediaListDto? Document { get; set; }

        public MediaListDto? Logo { get; set; }

    }
}
