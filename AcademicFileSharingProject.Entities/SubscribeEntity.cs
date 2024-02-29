using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
	[Table("Subscribe")]
	public class SubscribeEntity:EntityBase
	{
		public long? UserID { get; set; }

		public string Email { get; set; }

		public long SubscribeUserID { get; set;}

		public DateTime SubscribeDate { get; set; }

		[ForeignKey(nameof(SubscribeUserID))]
		public UserEntity SubcsribeUser { get; set; }
		[ForeignKey(nameof(UserID))]
		public UserEntity User { get; set; }

	}
}
