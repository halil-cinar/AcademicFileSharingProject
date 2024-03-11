using AcademicFileSharingProject.Core.DataAccess.EntityFramework;
using AcademicFileSharingProject.DataAccess.Abstract;
using AcademicFileSharingProject.DataAccess.EntityFramework;
using AcademicFileSharingProject.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.DataAccess.Repository.EntityFramework
{
    public class EfUserDeviceDal:EfEntityGenericRepositoryBase<UserDeviceEntity,DatabaseContext>, IUserDeviceRepository
    {
        public override IQueryable<UserDeviceEntity> BaseGetAll(DatabaseContext context)
        {
            return base.BaseGetAll(context).Include(x => x.User);
        }
    }
}
