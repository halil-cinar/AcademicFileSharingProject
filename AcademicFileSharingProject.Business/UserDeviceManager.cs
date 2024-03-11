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

namespace AcademicFileSharingProject.Business
{
    public class UserDeviceManager:ServiceBase<UserDeviceEntity>, IUserDeviceService
    {
        public UserDeviceManager(IEntityRepository<UserDeviceEntity> repository, IMapper mapper, BaseEntityValidator<UserDeviceEntity> validator) : base(repository, mapper, validator)
        {
        }


        public async Task<BussinessLayerResult<UserDeviceListDto>> Add(UserDeviceDto userDevice)
        {
            var response = new BussinessLayerResult<UserDeviceListDto>();
            try
            {
                var entity = Mapper.Map<UserDeviceEntity>(userDevice);
                entity.CreatedTime = DateTime.Now;
                entity.IsDeleted = false;
                entity.Id = 0;

                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.UserDeviceUserDeviceAddValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<UserDeviceListDto>(Repository.Add(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserDeviceUserDeviceAddExceptionError, ex.Message);
            }
            return response;

        }


        public async Task<BussinessLayerResult<UserDeviceListDto>> Delete(long id)
        {
            var response = new BussinessLayerResult<UserDeviceListDto>();
            try
            {
                Repository.SoftDelete(id);

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserDeviceUserDeviceDeleteExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<UserDeviceListDto>> Get(long id)
        {
            var response = new BussinessLayerResult<UserDeviceListDto>();
            try
            {
                var entity = Repository.Get(id);
                var dto = Mapper.Map<UserDeviceListDto>(entity);
                response.Result = dto;

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserDeviceUserDeviceGetExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<GenericLoadMoreDto<UserDeviceListDto>>> GetAll(LoadMoreFilter<UserDeviceFilter> filter)
        {
            var response = new BussinessLayerResult<GenericLoadMoreDto<UserDeviceListDto>>();
            try
            {
                var entities = (filter.Filter != null) ?
                    Repository.GetAll(x =>
                 (filter.Filter.DeviceType == null || filter.Filter.DeviceType == x.DeviceType)
                && (filter.Filter.UserId == null || filter.Filter.UserId == x.UserId)
                && (filter.Filter.UserIds == null || filter.Filter.UserIds.Contains( x.UserId))
                && (string.IsNullOrEmpty(filter.Filter.ConnectionId) || x.ConnectionId.Contains(filter.Filter.ConnectionId))

                && (x.IsDeleted == false)
                ) : Repository.GetAll(x => x.IsDeleted == false);

                var firstIndex = filter.PageCount * filter.ContentCount;
                var lastIndex = firstIndex + filter.ContentCount;

                lastIndex = Math.Min(lastIndex, entities.Count);
                var values = new List<UserDeviceListDto>();
                for (int i = firstIndex; i < lastIndex; i++)
                {
                    values.Add(Mapper.Map<UserDeviceListDto>(entities[i]));
                }

                response.Result = new GenericLoadMoreDto<UserDeviceListDto>
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
                response.AddError(Dtos.Enums.ErrorMessageCode.UserDeviceUserDeviceGetAllExceptionError, ex.Message);
            }
            return response;
        }

        public async Task<BussinessLayerResult<UserDeviceListDto>> Update(UserDeviceDto userDevice)
        {
            var response = new BussinessLayerResult<UserDeviceListDto>();
            try
            {
                var entity = Repository.Get(userDevice.Id);
                entity.DeviceType = userDevice.DeviceType;
                entity.ConnectionId = userDevice.ConnectionId;




                var validationResult = Validator.Validate(entity);
                if (!validationResult.IsValid)
                {
                    foreach (var item in validationResult.Errors)
                    {
                        response.AddError(Dtos.Enums.ErrorMessageCode.UserDeviceUserDeviceUpdateValidationError, item.ErrorMessage);

                    }
                    return response;
                }

                response.Result = Mapper.Map<UserDeviceListDto>(Repository.Update(entity));

            }
            catch (Exception ex)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.UserDeviceUserDeviceUpdateExceptionError, ex.Message);
            }
            return response;
        }
    }
}
