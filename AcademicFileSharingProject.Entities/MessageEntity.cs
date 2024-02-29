using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
    [Table("Message")]
    public class MessageEntity:EntityBase
    {
        public long SenderUserId { get; set; }
        public long ChatId { get; set; }
        public string Message { get; set; }

        [ForeignKey(nameof(SenderUserId))]
        public UserEntity SenderUser { get; set; }

        [ForeignKey(nameof(ChatId))]
        public ChatEntity Chat { get; set; }
    }
}
