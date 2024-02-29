using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
    [Table("PostMedia")]
    public class PostMediaEntity:EntityBase
    {
        public long PostId { get; set; }
        public long MediaId { get; set; }

        [ForeignKey(nameof(MediaId))]
        public MediaEntity Media { get; set; }

        [ForeignKey(nameof(PostId))]
        public PostEntity Post { get; set; }

    }
}
