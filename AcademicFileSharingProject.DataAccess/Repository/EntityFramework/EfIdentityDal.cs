using AcademicFileSharingProject.Core.DataAccess.EntityFramework;
using AcademicFileSharingProject.DataAccess.Abstract;
using AcademicFileSharingProject.DataAccess.EntityFramework;
using AcademicFileSharingProject.Entities;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.DataAccess.Repository.EntityFramework
{
    public class EfIdentityDal:EfEntityGenericRepositoryBase<IdentityEntity,DatabaseContext>,IIdentityRepository
    {
        public override IQueryable<IdentityEntity> BaseGetAll(DatabaseContext context)
        {
            return base.BaseGetAll(context).Include(x=>x.User);
        }
    }
}


