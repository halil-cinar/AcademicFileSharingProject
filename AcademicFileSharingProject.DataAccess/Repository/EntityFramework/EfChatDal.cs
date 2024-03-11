using AcademicFileSharingProject.Core.DataAccess.EntityFramework;
using AcademicFileSharingProject.DataAccess.Abstract;
using AcademicFileSharingProject.DataAccess.EntityFramework;
using AcademicFileSharingProject.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.DataAccess.Repository.EntityFramework
{
    public class EfChatDal:EfEntityGenericRepositoryBase<ChatEntity,DatabaseContext>,IChatRepository
    {
        public override IQueryable<ChatEntity> BaseGetAll(DatabaseContext context)
        {
            var result = base.BaseGetAll(context).Include(x => x.ChatUsers).ThenInclude(x => x.User).OrderByDescending(x=>x.Id);

            return result;


        }
    }
}
