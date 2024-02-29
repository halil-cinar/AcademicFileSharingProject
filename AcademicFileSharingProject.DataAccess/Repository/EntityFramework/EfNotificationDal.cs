using AcademicFileSharingProject.Core.DataAccess.EntityFramework;
using AcademicFileSharingProject.DataAccess.Abstract;
using AcademicFileSharingProject.DataAccess.EntityFramework;
using AcademicFileSharingProject.Entities;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.DataAccess.Repository.EntityFramework
{
	public class EfNotificationDal:EfEntityGenericRepositoryBase<NotificationEntity,DatabaseContext>,INotificationRepository
	{

		public override IQueryable<NotificationEntity> BaseGetAll(DatabaseContext context)
		{
			var values=base.BaseGetAll(context).Include(x => x.User);
			foreach(var item in values)
			{
				if (item.EntityType == EEntityType.Message)
				{
					item.Entity = context.Set<MessageEntity>().Find(item.EntityId);
				}
			 	else if (item.EntityType == EEntityType.Post)
				{
					item.Entity = context.Set<PostEntity>().Find(item.EntityId);
				}
				else if (item.EntityType == EEntityType.Blog)
				{
					item.Entity = context.Set<BlogEntity>().Find(item.EntityId);
				}

			}
			return values;

		}

	}
}
