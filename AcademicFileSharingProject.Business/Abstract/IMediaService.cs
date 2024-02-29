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
    public interface IMediaService
    {
        public Task<BussinessLayerResult<long?>> Add(MediaDto media);
        public Task<BussinessLayerResult<bool>> Delete(long id);
        public Task<BussinessLayerResult<long?>> Update(MediaDto media);
        public Task<BussinessLayerResult<MediaListDto>> Get(long id);



    }
}

