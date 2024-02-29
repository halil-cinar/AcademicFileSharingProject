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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace AcademicFileSharingProject.Business
{
    public class UserRoleManager : ServiceBase<UserRoleEntity>, IUserRoleService
    {
        public UserRoleManager(IEntityRepository<UserRoleEntity> repository, IMapper mapper, BaseEntityValidator<UserRoleEntity> validator) : base(repository, mapper, validator)
        {
        }

        public async Task<BussinessLayerResult<UserRoleListDto>> Add(UserRoleDto userrole)
        {
            var response = new BussinessLayerResult<UserRoleListDto>();
            try
            {
                var entity = Mapper.Map<UserRoleEntity>(userrole);
                entity.CreatedTime = DateTime.Now;
                entity.IsDeleted = false;
                entity.Id = 0;

                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.UserRoleUserRoleAddValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<UserRoleListDto>(Repository.Add(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserRoleUserRoleAddExceptionError, ex.Message);
            }
            return response;

        }

        public async Task<BussinessLayerResult<UserRoleListDto>> Delete(long id)
        {
            var response = new BussinessLayerResult<UserRoleListDto>();
            try
            {
                Repository.SoftDelete(id);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserRoleUserRoleDeleteExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<UserRoleListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<UserRoleListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<UserRoleListDto>(entity);
                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserRoleUserRoleGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<GenericLoadMoreDto<UserRoleListDto>>> GetAll(LoadMoreFilter<UserRoleFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<UserRoleListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                (filter.Filter.UserId == null || filter.Filter.UserId == x.UserId)
                && (filter.Filter.Role == null || filter.Filter.Role == x.Role)
                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);
                entities=entities.OrderBy(x=>x.UserId).ToList();
                //Todo: Orderby bir alt katmana taþýnacak

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex = firstIndex + filter.ContentCount;

                lastIndex = Math.Min(lastIndex, entities.Count);
                var values = new List<UserRoleListDto>();
                for (int i = firstIndex; i < lastIndex; i++)
                {
                    values.Add(Mapper.Map<UserRoleListDto>(entities[i]));
                }

                response.Result = new GenericLoadMoreDto<UserRoleListDto>
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
                response.AddError(Dtos.Enums.ErrorMessageCode.UserRoleUserRoleGetAllExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<UserRoleListDto>> Update(UserRoleDto userrole)
        {
            var response = new BussinessLayerResult<UserRoleListDto>();
            try
            {
                var entity = Repository.Get(userrole.Id);
                entity.Role = userrole.Role;
                entity.UserId = userrole.UserId;



                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.UserRoleUserRoleUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<UserRoleListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserRoleUserRoleUpdateExceptionError, ex.Message);
            }
            return response;
        }
        public async Task<BussinessLayerResult<UserRoleListDto>> UpdateAll(UserRoleAllUpdateDto userrole)
        {
            var response = new BussinessLayerResult<UserRoleListDto>();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var oldEntities=Repository.GetAll(x=>x.UserId == userrole.UserId);
                    foreach (var item in oldEntities)
                    {
                        Repository.SoftDelete(item);
                    }


                    var newEntities = new List<UserRoleEntity>();
                    foreach (var item in userrole.Roles)
                    {
                        newEntities.Add(new UserRoleEntity
                        {
                            CreatedTime = DateTime.Now,
                            IsDeleted = false,
                            Role = item,
                            UserId = userrole.UserId,

                        });
                    }
                    foreach (var item in newEntities)
                    {
                        Repository.Add(item);
                    }
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    response.AddError(Dtos.Enums.ErrorMessageCode.UserRoleUserRoleUpdateExceptionError, ex.Message);
                }
            }
            return response;
        }

    }
}

