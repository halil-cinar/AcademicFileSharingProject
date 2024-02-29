using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.ListDtos
{
    
    public class MessageListDto:DtoBase
    {
        public long SenderUserId { get; set; }
        public long ChatId { get; set; }
        public string Message { get; set; }

        
        public UserListDto SenderUser { get; set; }

        
        public ChatListDto Chat { get; set; }
    }
}
