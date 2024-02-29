using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.ListDtos
{
	public class BlogCommentListDto : DtoBase
	{
		public long BlogId { get; set; }
		public long SenderUserId { get; set; }
		public string Content { get; set; }

		

		public UserListDto SenderUser { get; set; }

		public BlogListDto Blog { get; set; }


		
	}
}
