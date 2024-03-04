using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Dtos.Filters;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using AcademicFileSharingProject.Dtos.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Business.Abstract
{
    public interface ISoftwareService
    {
        public Task<BussinessLayerResult<SoftwareListDto>> Add(SoftwareDto software);
        public Task<BussinessLayerResult<SoftwareListDto>> Delete(long id);
        public Task<BussinessLayerResult<SoftwareListDto>> Update(SoftwareDto software);
        public Task<BussinessLayerResult<SoftwareListDto>> Get(long id);
        public Task<BussinessLayerResult<GenericLoadMoreDto<SoftwareListDto>>> GetAll(LoadMoreFilter<SoftwareFilter> filter);

        public Task<BussinessLayerResult<SoftwareListDto>> ChangeLogo(SoftwareDto software);
        public Task<BussinessLayerResult<SoftwareListDto>> ChangeDocument(SoftwareDto software);
        public Task<BussinessLayerResult<SoftwareListDto>> ChangeFile(SoftwareDto software);
    }
}
