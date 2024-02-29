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
	public class BlogListDto:DtoBase
	{
		public long UserId { get; set; }

		public bool IsAir { get; set; }
		public DateTime ReleaseDate { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public long? MediaId { get; set; }


		public UserListDto User { get; set; }

		public MediaListDto Media { get; set; }

        public  List<BlogCommentListDto> BlogComments { get; set; }


    }
}
