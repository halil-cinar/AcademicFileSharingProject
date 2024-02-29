using  AcademicFileSharingProject.Dtos.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  AcademicFileSharingProject.Dtos.Errors
{
    public class ErrorMessage
    {
        public ErrorMessageCode ErrorCode { get; set; }
        public string Message { get; set; }
    }
}
