using AcademicFileSharingProject.Entities.Abstract;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Entities
{
    [Table("Chat")]
    public class ChatEntity:EntityBase
    {
        public string Title { get; set; }

        public EChatType ChatType { get; set; }

        public virtual ICollection<ChatUserEntity> ChatUsers { get; set; }

    }
}
