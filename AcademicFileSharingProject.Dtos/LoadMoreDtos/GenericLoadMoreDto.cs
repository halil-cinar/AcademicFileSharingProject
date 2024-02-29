using  AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  AcademicFileSharingProject.Dtos.LoadMoreDtos
{
    public class GenericLoadMoreDto<T>:BaseLoadMoreDto
        where T : DtoBase,new()
    {
        public List<T> Values { get; set; }
    }
}
