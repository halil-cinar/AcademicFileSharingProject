using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
	[Table("Subscribe")]
	public class SubscribeDto:DtoBase
	{
		public long? UserID { get; set; }

		public string Email { get; set; }

		public long SubscribeUserID { get; set;}

		public DateTime SubscribeDate { get; set; }

		
	

	}
}
