using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Dtos.Enums;
using AcademicFileSharingProject.Entities;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.ListDtos
{
    
    public class ChatListDto:DtoBase
    {
        public string Title { get; set; }

        public EChatType ChatType { get; set; }
        public  List<ChatUserListDto> ChatUsers { get; set; }
    }
}
