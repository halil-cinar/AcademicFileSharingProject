using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  AcademicFileSharingProject.Dtos.LoadMoreDtos
{
    public class BaseLoadMoreDto
    {
        public int PageCount { get; set; }
        public int TotalContentCount { get; set; }

        public int ContentCount { get; set; }

        public int TotalPageCount { get; set; }

        public bool NextPage { get; set; }
        public bool PrevPage { get; set; }


    }
}
