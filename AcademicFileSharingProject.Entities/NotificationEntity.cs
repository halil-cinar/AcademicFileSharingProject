using AcademicFileSharingProject.Entities.Abstract;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
	[Table("Notification")]
	public class NotificationEntity:EntityBase
	{
		public long UserId { get; set; }
		public string Title { get; set; }

		public long EntityId { get; set; }

		public EEntityType EntityType { get; set; }

		[NotMapped]
		public object Entity { get; set; }

		[ForeignKey(nameof(UserId))]
		public UserEntity User { get; set; }

	}
}
