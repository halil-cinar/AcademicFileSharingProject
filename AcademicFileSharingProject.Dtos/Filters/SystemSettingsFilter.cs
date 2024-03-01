using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Dtos.Filters
{
    public class SystemSettingsFilter
    {
        public ESystemSetting Key { get; set; }
        public string Name { get; set; }
    }
}
