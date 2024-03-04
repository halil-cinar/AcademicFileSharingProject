using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
    [Table("Software")]
    public class SoftwareEntity:EntityBase
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        public long FileId { get; set; }
        public long? DocumentId { get; set; }
        public long? LogoId { get; set; }
        public long UserId { get; set; }

        public bool IsAir { get; set; }


        [ForeignKey(nameof(FileId))]
        public MediaEntity File { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public MediaEntity? Document { get; set; }

        [ForeignKey(nameof(LogoId))]
        public MediaEntity? Logo { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity User { get; set; }



    }
}
