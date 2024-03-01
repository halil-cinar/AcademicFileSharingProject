using AcademicFileSharingProject.Core.DataAccess.EntityFramework;
using AcademicFileSharingProject.DataAccess.Abstract;
using AcademicFileSharingProject.DataAccess.EntityFramework;
using AcademicFileSharingProject.Entities;
using AcademicFileSharingProject.Entities.Enums;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.DataAccess.Repository.EntityFramework
{
    public class EfSystemSettingsDal:EfEntityGenericRepositoryBase<SystemSettingsEntity,DatabaseContext>, ISystemSettingsRepository
    {
        public override IQueryable<SystemSettingsEntity> BaseGetAll(DatabaseContext context)
        {
            return base.BaseGetAll(context);
        }
    }
}
