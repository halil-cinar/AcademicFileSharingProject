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
    [Table("SystemSettings")]
    public class SystemSettingsEntity : EntityBase
    {
        public ESystemSetting Key { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }
    }
}
