using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Core.DataAccess;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.Filters;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Dtos.Result;
using AcademicFileSharingProject.Entities;
using AcademicFileSharingProject.Entities.Abstract;
using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AcademicFileSharingProject.Business
{
    public class RoleMethodManager : ServiceBase<RoleMethodEntity>, IRoleMethodService
    {
        public RoleMethodManager(IEntityRepository<RoleMethodEntity> repository, IMapper mapper, BaseEntityValidator<RoleMethodEntity> validator) : base(repository, mapper, validator)
        {
        }


        public async Task<BussinessLayerResult<RoleMethodListDto>> Add(RoleMethodDto roleMethod)
        {
            var response = new BussinessLayerResult<RoleMethodListDto>();
            try
            {
                var entity = Mapper.Map<RoleMethodEntity>(roleMethod);
                entity.CreatedTime = DateTime.Now;
                entity.IsDeleted = false;
                entity.Id = 0;

                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.RoleMethodRoleMethodAddValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<RoleMethodListDto>(Repository.Add(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.RoleMethodRoleMethodAddExceptionError, ex.Message);
            }
            return response;

        }


        public async Task<BussinessLayerResult<RoleMethodListDto>> Delete(long id)
        {
            var response = new BussinessLayerResult<RoleMethodListDto>();
            try
            {
                Repository.SoftDelete(id);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.RoleMethodRoleMethodDeleteExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<RoleMethodListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<RoleMethodListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<RoleMethodListDto>(entity);
                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.RoleMethodRoleMethodGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<GenericLoadMoreDto<RoleMethodListDto>>> GetAll(LoadMoreFilter<RoleMethodFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<RoleMethodListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                 (filter.Filter.Method == null || filter.Filter.Method == x.Method)
                && (filter.Filter.Role == null || filter.Filter.Role == x.Role)
                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex = firstIndex + filter.ContentCount;

                lastIndex = Math.Min(lastIndex, entities.Count);
                var values = new List<RoleMethodListDto>();
                for (int i = firstIndex; i < lastIndex; i++)
                {
                    values.Add(Mapper.Map<RoleMethodListDto>(entities[i]));
                }

                response.Result = new GenericLoadMoreDto<RoleMethodListDto>
                {
                    Values = values,
                    ContentCount = filter.ContentCount,
                    NextPage = lastIndex < entities.Count,
                    TotalPageCount = Convert.ToInt32(Math.Ceiling(entities.Count / (double)filter.ContentCount)),
                    TotalContentCount = entities.Count,
                    PageCount = filter.PageCount > Convert.ToInt32(Math.Ceiling(entities.Count / (double)filter.ContentCount))
                    ? Convert.ToInt32(Math.Ceiling(entities.Count / (double)filter.ContentCount))
                    : filter.PageCount,
                    PrevPage = firstIndex > 0


                };

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.RoleMethodRoleMethodGetAllExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<RoleMethodListDto>> Update(RoleMethodDto roleMethod)
        {
            var response = new BussinessLayerResult<RoleMethodListDto>();
            try
            {
                var entity = Repository.Get(roleMethod.Id);
                entity.Method = roleMethod.Method;
                entity.Role = roleMethod.Role;




                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.RoleMethodRoleMethodUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<RoleMethodListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.RoleMethodRoleMethodUpdateExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<RoleMethodListDto>> UpdateAll(RoleMethodAllUpdateDto roleMethod)
        {
            var response = new BussinessLayerResult<RoleMethodListDto>();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var entities = Repository.GetAll(x => x.Role == roleMethod.Role);
                    foreach (var item in entities)
                    {
                        Repository.SoftDelete(item);
                    }

                    //Yeni kayıtlar
                    var list = roleMethod.Methods.Select(x => new RoleMethodEntity
                    {
                        IsDeleted = false,
                        Method = x,
                        Role = roleMethod.Role,
                        CreatedTime = DateTime.Now,

                    }).ToList();

                    foreach (var item in list)
                    {
                        Repository.Add(item);
                    }

                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    response.AddError(Dtos.Enums.ErrorMessageCode.RoleMethodRoleMethodUpdateExceptionError, ex.Message);
                }
                scope.Complete();
            }
            return response;
        }



    }
}
