﻿using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
    [Table("Post")]
    public class PostEntity:EntityBase
    {
        public long UserId { get; set; }
        public string Content { get; set; }

        public long? PostImageId { get; set; }
        public bool IsAir { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity User { get; set; }

        [ForeignKey(nameof(PostImageId))]
        public MediaEntity? PostImage { get; set;}

        public virtual ICollection<PostMediaEntity> PostMedias { get; set; }


    }
}
