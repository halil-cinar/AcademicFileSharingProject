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
    [Table("UserDevice")]
    public class UserDeviceEntity:  EntityBase
    {
        public long UserId { get; set; }
        public EDeviceType DeviceType { get; set; }
        public string ConnectionId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserEntity User { get; set; }

    }
}
