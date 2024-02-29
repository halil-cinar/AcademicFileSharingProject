using AcademicFileSharingProject.Core.DataAccess;
using AcademicFileSharingProject.Entities.Abstract;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Business.Abstract
{
    public abstract class ServiceBase<TEntity>
        where TEntity : EntityBase, new()
    {
        protected IEntityRepository<TEntity> Repository { get; private set; }

        protected IMapper Mapper { get; private set; }

        protected BaseEntityValidator<TEntity> Validator { get; private set; }

        protected ServiceBase(IEntityRepository<TEntity> repository, IMapper mapper, BaseEntityValidator<TEntity> validator)
        {
            Repository = repository;
            Mapper = mapper;
            Validator = validator;
        }


    }
}
