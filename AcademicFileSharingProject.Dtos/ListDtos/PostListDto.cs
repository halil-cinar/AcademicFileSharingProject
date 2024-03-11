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
    
    public class PostListDto:DtoBase
    {
        public long UserId { get; set; }
        public string Content { get; set; }

        public long? PostMediaId { get; set; }
        public bool IsAir { get; set; }


        public UserListDto User { get; set; }

        
        public MediaListDto PostMedia { get; set;}

        public  List<PostMediaListDto> PostMedias { get; set; }

    }
}
