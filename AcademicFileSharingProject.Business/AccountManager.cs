using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.Result;
using AcademicFileSharingProject.Entities.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Business
{
    public class AccountManager : IAccountService
    {
        private readonly IUserService _userService;
        private readonly IUserRoleService _userRoleService;
        private readonly IRoleMethodService _roleMethodService;
        private readonly ISessionService _sessionService;
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public AccountManager(IIdentityService identityService, ISessionService sessionService, IUserService userService, IHttpContextAccessor httpContextAccessor, IMapper mapper, IUserRoleService userRoleService, IRoleMethodService roleMethodService)
        {
            _identityService = identityService;
            _sessionService = sessionService;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userRoleService = userRoleService;
            _roleMethodService = roleMethodService;
        }

        public async Task<BussinessLayerResult<bool?>> ForgatPassword(string email)
        {
            var response = new BussinessLayerResult<bool?>();    

            var userResult = await _userService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.UserFilter>
            {
                ContentCount = 1,
                PageCount = 0,
                Filter = new Dtos.Filters.UserFilter
                {
                    Email = email,
                }
            });
            if (userResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
            {
                response.ErrorMessages.AddRange(userResult.ErrorMessages);
                return response;
            }
            if (userResult.Result == null)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.AccountForgatPasswordEmailWrongError, "Lütfen eposta adresinizi doğru giriniz.");
                return response;
            }

            var identityResult = await _identityService.ForgatPassword(userResult.Result.Values[0].Id);
            if (identityResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
            {
                response.ErrorMessages.AddRange(identityResult.ErrorMessages);
                return response;
            }
            response.Result=true;
            return response;

        }

        public async Task<BussinessLayerResult<SessionListDto>> Login(IdentityCheckDto ıdentity)
        {
            var response = new BussinessLayerResult<SessionListDto>();
            var result = await _identityService.CheckPassword(ıdentity);
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Error)
            {
                response.ErrorMessages.AddRange(result.ErrorMessages);
                return response;
            }
            if (result.Result == null)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.AccountLoginPasswordWrongError, "Kullanıcı adı veya şifre yanlış");
                return response;
            }
            DateTime? expiryDate = (!ıdentity.RememberMe) ? DateTime.Now.AddDays(1) : null;
            var sessionResult = await _sessionService.Add(new SessionDto
            {
                CreatedTime = DateTime.Now,
                DeviceType = ıdentity.DeviceType,
                ExpiryDate = expiryDate,
                IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "",
                Key = Guid.NewGuid().ToString(),
                UserId = result.Result.UserId,

            });
            if (sessionResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
            {
                response.ErrorMessages.AddRange(sessionResult.ErrorMessages);
                return response;
            }

            response.Result = sessionResult.Result;
            return response;



        }

        public async Task<BussinessLayerResult<SessionListDto>> Logout(string key)
        {
            var response = new BussinessLayerResult<SessionListDto>();
            var sessionResult = await _sessionService.Get(key);
            if (sessionResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
            {
                response.ErrorMessages.AddRange(sessionResult.ErrorMessages);
                return response;
            }

            if (sessionResult.Result == null)
            {
                response.AddError(Dtos.Enums.ErrorMessageCode.AccountLogoutPasswordWrongError, "oturum bilginiz alınamamıştır");
                return response;
            }
            var session = _mapper.Map<SessionDto>(sessionResult.Result);

            session.ExpiryDate = DateTime.Now;
            sessionResult = await _sessionService.Update(session);
            if (sessionResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
            {
                response.ErrorMessages.AddRange(sessionResult.ErrorMessages);
                return response;
            }

            response.Result = sessionResult.Result;
            return response;

        }

        public async Task<BussinessLayerResult<SessionListDto>> SignUp(UserAddDto user)
        {
            
            var response=new BussinessLayerResult<SessionListDto>();
            var userResult= await _userService.Add(user);
            if (userResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
            {
                response.ErrorMessages.AddRange(userResult.ErrorMessages);
                return response;
            }

            var sessionResult = await _sessionService.Add(new SessionDto
            {
                CreatedTime = DateTime.Now,
                DeviceType = Entities.Enums.EDeviceType.None,
                ExpiryDate = DateTime.Now.AddDays(1),
                IpAddress = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                Key = Guid.NewGuid().ToString(),
                UserId = user.Id
            });
            if (sessionResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
            {
                response.ErrorMessages.AddRange(sessionResult.ErrorMessages);
                return response;
            }

            response.Result=sessionResult.Result;
            return response;

        }

        public async Task<BussinessLayerResult<SessionListDto>> GetSession(string key)
        {
            var response = new BussinessLayerResult<SessionListDto>();
            var sessionResult = await _sessionService.Get(key);
            if (sessionResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
            {
                response.ErrorMessages.AddRange(sessionResult.ErrorMessages);
                return response;
            }

            response.Result = sessionResult.Result;
            return response;
        }

        public async Task<BussinessLayerResult<List<EMethod>>> GetUserRoleMethods(long userId)
        {
            var response=new BussinessLayerResult<List<EMethod>>();
            var userRoleResult = await _userRoleService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.UserRoleFilter>
            {
                ContentCount = int.MaxValue,
                PageCount = 0,
                Filter = new Dtos.Filters.UserRoleFilter
                {
                    UserId = userId,
                }
            });
            if (userRoleResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
            {
                response.ErrorMessages.AddRange(userRoleResult.ErrorMessages);
                return response;
            }
            var methodList=new List<EMethod>();

            foreach (var role in userRoleResult.Result.Values.Select(x=>x.Role).ToList())
            {
                var methodResult = await _roleMethodService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.RoleMethodFilter>
                {
                    ContentCount = int.MaxValue,
                    PageCount = 0,
                    Filter = new Dtos.Filters.RoleMethodFilter
                    {
                        Role = role,
                    }
                });
                if (methodResult.ResultStatus == Dtos.Enums.ResultStatus.Error)
                {
                    response.ErrorMessages.AddRange(methodResult.ErrorMessages);
                    return response;
                }
                methodList.AddRange(methodResult.Result.Values.Select(x => x.Method).ToList());

            }

            response.Result = methodList;
            return response;

        }

    }
}
