using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
	public class BlogFilter 
	{
		public long? UserId { get; set; }

		public bool? IsAir { get; set; }

		public string? Title { get; set; }
		public string? Content { get; set; }
		


	}
}
