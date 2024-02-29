using AcademicFileSharingProject.Dtos.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.ListDtos
{
    
    public class MediaListDto:DtoBase
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }

        public byte[] File { get; set; }



    }
}
