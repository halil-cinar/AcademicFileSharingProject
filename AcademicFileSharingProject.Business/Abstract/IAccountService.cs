using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.Result;
using AcademicFileSharingProject.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Business.Abstract
{
    public interface IAccountService
    {
        public Task<BussinessLayerResult<SessionListDto>> Login(IdentityCheckDto ıdentity);
        public Task<BussinessLayerResult<SessionListDto>> Logout(string key);
        public Task<BussinessLayerResult<SessionListDto>> SignUp(UserAddDto user);

        public Task<BussinessLayerResult<bool?>> ForgatPassword(string email);

        public  Task<BussinessLayerResult<SessionListDto>> GetSession(string key);
        public  Task<BussinessLayerResult<List<EMethod>>> GetUserRoleMethods(long userId);
    }
}
