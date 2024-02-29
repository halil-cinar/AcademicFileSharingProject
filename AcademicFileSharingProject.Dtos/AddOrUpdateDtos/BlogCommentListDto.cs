using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
	public class BlogCommentDto : DtoBase
	{
		public long BlogId { get; set; }
		public long SenderUserId { get; set; }
		public string Content { get; set; }

	


		
	}
}
