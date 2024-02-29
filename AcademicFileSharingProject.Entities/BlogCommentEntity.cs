using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
	[Table("BlogComment")]
	public class BlogCommentEntity : EntityBase
	{
		public long BlogId { get; set; }
		public long SenderUserId { get; set; }
		public string Content { get; set; }

		

		[ForeignKey(nameof(SenderUserId))]
		public UserEntity SenderUser { get; set; }

		[ForeignKey(nameof(BlogId))]
		public BlogEntity Blog { get; set; }


		
	}
}
