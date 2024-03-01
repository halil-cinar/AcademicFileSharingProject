using AcademicFileSharingProject.Dtos.Abstract;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.ListDtos
{
    public class SystemSettingsListDto:DtoBase
    {
        public ESystemSetting Key { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
    }
}
