using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
	public class NotificationFilter 
	{
		public long? UserId { get; set; }
		public string? Title { get; set; }

		public long? EntityId { get; set; }

		public EEntityType? EntityType { get; set; }

		

	}
}
