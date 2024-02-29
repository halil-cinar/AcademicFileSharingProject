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
    public class EfUserDal:EfEntityGenericRepositoryBase<UserEntity,DatabaseContext>,IUserRepository
    {
        public override IQueryable<UserEntity> BaseGetAll(DatabaseContext context)
        {
            var a = base.BaseGetAll(context).Include(x=>x.ProfileImage).Include(x=>x.UserRoles);
            return a;
        }
    }
}


