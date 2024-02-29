using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
    public class LoadMoreFilter<T> 
    {
        public int PageCount { get; set; }
        public int ContentCount { get; set; }
        public T? Filter { get; set; }
    }
}
