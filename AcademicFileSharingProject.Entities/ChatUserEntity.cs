using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
    [Table("ChatUser")]
    public class ChatUserEntity:EntityBase
    {
        public long ChatId { get; set; }
        public long UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity User { get; set; }

        [ForeignKey(nameof(ChatId))]
        public ChatEntity Chat { get; set; }
    }
}
