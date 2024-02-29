using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
	
	public class SubscribeFilter 
	{
		public long? UserID { get; set; }

		public string? Email { get; set; }

		public long? SubscribeUserID { get; set;}


		
	

	}
}
