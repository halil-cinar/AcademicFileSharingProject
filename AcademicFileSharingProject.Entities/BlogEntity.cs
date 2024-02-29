using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
	[Table("Blog")]
	public class BlogEntity:EntityBase
	{
		public long UserId { get; set; }

		public bool IsAir { get; set; }
		public DateTime ReleaseDate { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public long? MediaId { get; set; }


		[ForeignKey(nameof(UserId))]
		public UserEntity User { get; set; }

		[ForeignKey(nameof(MediaId))]
		public MediaEntity? Media { get; set; }

		public virtual ICollection<BlogCommentEntity> BlogComments { get; set; }


	}
}
