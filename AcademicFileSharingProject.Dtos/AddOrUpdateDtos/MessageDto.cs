using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    public class MessageDto:DtoBase
    {
        public long SenderUserId { get; set; }
        public long ChatId { get; set; }
        public string Message { get; set; }

        
    }
}
