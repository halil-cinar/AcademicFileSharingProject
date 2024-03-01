using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Core.DataAccess;
using AcademicFileSharingProject.Core.ExtensionsMethods;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.Enums;
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
    public class IdentityManager : ServiceBase<IdentityEntity>, IIdentityService
    {
        private readonly INotificationService _notificationService;
        public IdentityManager(IEntityRepository<IdentityEntity> repository, IMapper mapper, BaseEntityValidator<IdentityEntity> validator, INotificationService notificationService) : base(repository, mapper, validator)
        {
            _notificationService = notificationService;
        }

        public async Task<BussinessLayerResult<IdentityListDto>> Add(IdentityDto identity)
        {
            var response = new BussinessLayerResult<IdentityListDto>();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {

                    var salt = ExtensionsMethods.GenerateRandomString(64);

                    var entity = new IdentityEntity
                    {
                        IsDeleted = false,
                        CreatedTime = DateTime.Now,
                        IsValid = true,
                        PasswordSalt = salt,
                        PasswordHash = ExtensionsMethods.CalculateMD5Hash(salt + identity.Password + salt),
                        UserId = identity.UserId,
                    };

                    var validationResult = Validator.Validate(entity);
                    if (!validationResult.IsValid)
                    {
                        scope.Dispose();
                        foreach (var item in validationResult.Errors)
                        {
                            response.AddError(Dtos.Enums.ErrorMessageCode.IdentityIdentityAddValidationError, item.ErrorMessage);

                        }

                        return response;
                    }

                    var oldEntities = Repository.GetAll(x => x.IsValid == true && x.UserId == identity.UserId);
                    oldEntities.ForEach(x =>
                    {
                        x.IsValid = false;
                        Repository.Update(x);
                    });

                    response.Result = Mapper.Map<IdentityListDto>(Repository.Add(entity));
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    response.AddError(Dtos.Enums.ErrorMessageCode.IdentityIdentityAddExceptionError, ex.Message);
                }
            }
            return response;

        }



        public async Task<BussinessLayerResult<IdentityListDto>> CheckPassword(IdentityCheckDto identity)
        {
            var response = new BussinessLayerResult<IdentityListDto>();
            try
            {

                var entity = Repository.Get(x =>
                x.IsValid == true
                && x.IsDeleted == false
                && x.User.Email == identity.Email
               
                );

                var a = ExtensionsMethods.CalculateMD5Hash(entity.PasswordSalt + identity.Password + entity.PasswordSalt);

                if (entity!=null&&entity.PasswordHash == ExtensionsMethods.CalculateMD5Hash(entity.PasswordSalt + identity.Password + entity.PasswordSalt))
                {
                    var dto = Mapper.Map<IdentityListDto>(entity);
                    response.Result = dto;
                }
                

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.IdentityIdentityGetExceptionError, ex.Message);
            }
            return response;
        }



        public async Task<BussinessLayerResult<IdentityListDto>> Update(IdentityUpdateDto identity)
        {
            var response = new BussinessLayerResult<IdentityListDto>();
            try
            {

                var oldIdentity = Repository.Get(x => x.IsValid == true && x.UserId == identity.UserId


                );


                if (oldIdentity == null || oldIdentity.PasswordHash != ExtensionsMethods.CalculateMD5Hash(oldIdentity.PasswordSalt + identity.OldPassword + oldIdentity.PasswordSalt))
                {
                    response.AddError(Dtos.Enums.ErrorMessageCode.IdentityUpdateIdentityOldIdentityNotFound,
                        "Eski kimlik bilgisi bulunamadý.Lütfen girdiðiniz bilgileri kontrol edin");
                }



                var oldEntities = Repository.GetAll(x => x.IsValid == true && x.UserId == identity.UserId);
                oldEntities.ForEach(x =>
                {
                    x.IsValid = false;
                    Repository.Update(x);
                });
                var addResponse = await Add(new IdentityDto
                {
                    CreatedTime = DateTime.Now,
                    IsDeleted = false,
                    Password = identity.Password,
                    UserId = identity.UserId,
                });

                response.Result = addResponse.Result;
                response.ErrorMessages.AddRange(addResponse.ErrorMessages);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.IdentityIdentityUpdateExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<IdentityListDto>> ForgatPassword(long userId)
        {
            var response = new BussinessLayerResult<IdentityListDto>();
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var oldEntities = Repository.GetAll(x => x.IsValid == true && x.UserId == userId);
                    oldEntities.ForEach(x =>
                    {
                        x.IsValid = false;
                        Repository.Update(x);
                    });
                    var password = ExtensionsMethods.GenerateRandomPassword(10);
                    var addResponse = await Add(new IdentityDto
                    {
                        CreatedTime = DateTime.Now,
                        IsDeleted = false,
                        Password = password,
                        UserId = userId,
                    });

                    if (addResponse.ResultStatus == ResultStatus.Error)
                    {
                        scope.Dispose();
                        response.ErrorMessages.AddRange(addResponse.ErrorMessages);
                        return response;
                    }

                    var notificationResult = await _notificationService.NotifyUserOnEmail(new NotificationDto
                    {
                        EntityId = 0,
                        CreatedTime = DateTime.Now,
                        IsDeleted = false,
                        EntityType = Entities.Enums.EEntityType.None,
                        UserId = userId,
                        Title = "Þifreniz Deðiþtirilmiþtir"
                    },
                    $"<h1><strong>Gibtü </strong></h1><h2><strong><pre> < Akademik Paylaþým Platformu > </pre></strong></h2>" +
                    $" <pre>Þifreniz {DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()} te deðiþtirilmiþtir.  \n" +
                    $"Yeni Þifreniz:{password} \n" +
                    $"Lütfen þifrenizi kimse ile paylaþmayýnýz. </pre>");
                    if (notificationResult.ResultStatus == ResultStatus.Error)
                    {
                        scope.Dispose();
                        response.ErrorMessages.AddRange(notificationResult.ErrorMessages);
                        return response;
                    }

                    response.Result = addResponse.Result;
                    scope.Complete();
                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    response.AddError(Dtos.Enums.ErrorMessageCode.IdentityIdentityUpdateExceptionError, ex.Message);
                }
            }
            return response;
        }


    }
}

