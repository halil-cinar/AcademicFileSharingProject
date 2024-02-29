using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
    [Table("PostComment")]
    public class PostCommentEntity:EntityBase
    {
        public long PostId { get; set; }
        public long SenderUserId { get; set; }
        public string Content { get; set; }

        public long? MediaId { get; set; }

        [ForeignKey(nameof(SenderUserId))]
        public UserEntity SenderUser { get; set; }

        [ForeignKey(nameof(PostId))]
        public PostEntity Post { get; set; }


        [ForeignKey(nameof(MediaId))]
        public MediaEntity? Media { get; set; }
    }
}
