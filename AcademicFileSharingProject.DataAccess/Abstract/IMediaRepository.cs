using AcademicFileSharingProject.Core.DataAccess;
using AcademicFileSharingProject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.DataAccess.Abstract
{
    public interface IMediaRepository:IEntityRepository<MediaEntity>
    {
    }
}

