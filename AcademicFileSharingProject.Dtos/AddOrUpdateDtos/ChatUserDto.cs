﻿using AcademicFileSharingProject.Dtos.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    
    public class ChatUserDto:DtoBase
    {
        public long ChatId { get; set; }
        public long UserId { get; set; }

       
    }
}
