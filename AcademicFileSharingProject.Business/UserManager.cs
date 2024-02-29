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
    public class UserManager : ServiceBase<UserEntity>, IUserService
    {
        private readonly IIdentityService _identityService;
        private readonly IUserRoleService _userRoleService;
        private readonly IMediaService _mediaService;
        public UserManager(IEntityRepository<UserEntity> repository, IMapper mapper, BaseEntityValidator<UserEntity> validator, IIdentityService identityService, IUserRoleService userRoleService, IMediaService mediaService) : base(repository, mapper, validator)
        {
            _identityService = identityService;
            _userRoleService = userRoleService;
            _mediaService = mediaService;
        }

        public async Task<BussinessLayerResult<UserListDto>> Add(UserAddDto user)
        {
            var response = new BussinessLayerResult<UserListDto>();
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var entity = Mapper.Map<UserEntity>(user);
                    entity.CreatedTime = DateTime.Now;
                    entity.IsDeleted = false;
                    entity.Id = 0;

                    var validationResult = Validator.Validate(entity);
                    if (!validationResult.IsValid)
                    {
                        scope.Dispose();
                        foreach (var item in validationResult.Errors)
                        {
                            response.AddError(Dtos.Enums.ErrorMessageCode.UserUserAddValidationError, item.ErrorMessage);

                        }
                        return response;
                    }

                    Repository.Add(entity);


                    //AddIdentity
                    var identityResult = await _identityService.Add(new IdentityDto
                    {
                        UserId = entity.Id,
                        Password = user.Password
                    });

                    if (identityResult.ErrorMessages.Count > 0)
                    {
                        scope.Dispose();
                        response.ErrorMessages.AddRange(identityResult.ErrorMessages);
                        return response;
                    }

                    //AddRole
                    var roleResult = await _userRoleService.Add(new UserRoleDto
                    {
                        Role = user.Role,
                        UserId = entity.Id
                    });

                    if (roleResult.ErrorMessages.Count > 0)
                    {
                        scope.Dispose();
                        response.ErrorMessages.AddRange(roleResult.ErrorMessages);
                        return response;
                    }


                    response.Result = Mapper.Map<UserListDto>(entity);

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    response.AddError(Dtos.Enums.ErrorMessageCode.UserUserAddExceptionError, ex.Message);
                }
            }
            return response;

        }

        public async Task<BussinessLayerResult<UserListDto>> Delete(long id)
        {
            var response = new BussinessLayerResult<UserListDto>();
            try
            {
                Repository.SoftDelete(id);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserUserDeleteExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<UserListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<UserListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<UserListDto>(entity);
                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserUserGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<GenericLoadMoreDto<UserListDto>>> GetAll(LoadMoreFilter<UserFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<UserListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                (string.IsNullOrEmpty(filter.Filter.Title) || x.Title.Contains(filter.Filter.Title))
                && (string.IsNullOrEmpty(filter.Filter.PhoneNumber) || x.PhoneNumber.Contains(filter.Filter.PhoneNumber))
                && (string.IsNullOrEmpty(filter.Filter.Email) || x.Email.Contains(filter.Filter.Email) || x.Email2.Contains(filter.Filter.Email))
                && (string.IsNullOrEmpty(filter.Filter.FullName) || (x.Name + " " + x.Surname).Contains(filter.Filter.FullName))


                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex = firstIndex + filter.ContentCount;

                lastIndex = Math.Min(lastIndex, entities.Count);
                var values = new List<UserListDto>();
                for (int i = firstIndex; i < lastIndex; i++)
                {
                    values.Add(Mapper.Map<UserListDto>(entities[i]));
                }

                response.Result = new GenericLoadMoreDto<UserListDto>
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
                response.AddError(Dtos.Enums.ErrorMessageCode.UserUserGetAllExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<UserListDto>> Update(UserDto user)
        {
            var response = new BussinessLayerResult<UserListDto>();
            try
            {
                var entity = Repository.Get(user.Id);
                entity.Title = user.Title;
                entity.Surname = user.Surname;
                entity.PhoneNumber = user.PhoneNumber;
                entity.Name = user.Name;
                entity.Email = user.Email;
                entity.Email2 = user.Email2;
                entity.Description = user.Description;





                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.UserUserUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<UserListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserUserUpdateExceptionError, ex.Message);
            }
            return response;
        }


        public async Task<BussinessLayerResult<UserListDto>> ChangeProfilePhoto(UserDto user)
        {
            var response = new BussinessLayerResult<UserListDto>();
            try
            {
                var entity = Repository.Get(user.Id);
                

                //Media Ekleme/Güncelleme
                var mediaResult = (entity.ProfileImageId == null)
                    ? await _mediaService.Add(new MediaDto
                    {
                        File = user.ProfileImage,
                    })
                    : await _mediaService.Update(new MediaDto
                    {
                        File = user.ProfileImage,
                        Id =(long) entity.ProfileImageId
                    });

                if (mediaResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                {
                    response.ErrorMessages.AddRange(mediaResult.ErrorMessages);
                    return response;

                }

                entity.ProfileImageId = (long)mediaResult.Result;






                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.UserUserUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<UserListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserUserUpdateExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<List<UserListDto>>> SearchUser(string query)
        {
            var response = new BussinessLayerResult<List<UserListDto>> ();
            try
            {
                query=query?.ToLower();
                var entities = Repository.GetAll(x=>
                (x.Name+" "+x.Surname+" "+x.Title+" "+x.Email+" "+x.Email2).ToLower().Contains(query)
                ).Select(x =>
                {
                    x.UserRoles = null;
                    return x;
                });
                
                response.Result = Mapper.Map<List<UserListDto>>(entities);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserUserGetExceptionError, ex.Message);
            }
            return response;
        }

    }
}

