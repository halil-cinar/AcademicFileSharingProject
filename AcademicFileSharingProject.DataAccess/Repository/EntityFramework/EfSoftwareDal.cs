using AcademicFileSharingProject.Core.DataAccess.EntityFramework;
using AcademicFileSharingProject.DataAccess.Abstract;
using AcademicFileSharingProject.DataAccess.EntityFramework;
using AcademicFileSharingProject.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.DataAccess.Repository.EntityFramework
{
    public class EfSoftwareDal:EfEntityGenericRepositoryBase<SoftwareEntity,DatabaseContext>, ISoftwareRepository
    {
        public override IQueryable<SoftwareEntity> BaseGetAll(DatabaseContext context)
        {
            return base.BaseGetAll(context).Include(x => x.File).Include(x => x.Document).Include(x => x.Logo).Include(x=>x.User);
        }
    }
}
