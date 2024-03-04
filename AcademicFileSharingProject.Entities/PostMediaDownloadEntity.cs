using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
    [Table("PostMediaDownload")]
    public class PostMediaDownloadEntity:EntityBase
    {
        public long PostMediaId { get; set; }
        public long UserId { get; set; }

        [ForeignKey(nameof(PostMediaId))]
        public PostMediaEntity PostMedia { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity User { get; set; }

    }
}
