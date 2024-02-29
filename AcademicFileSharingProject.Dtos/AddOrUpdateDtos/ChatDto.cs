using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.AddOrUpdateDtos
{
    public class ChatDto:DtoBase
    {
        public string Title { get; set; }

        public EChatType ChatType { get; set; }

        public List<long> UserIds { get; set; }



    }
}
