using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.WebUI.Enums;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace AcademicFileSharingProject.WebUI.Controllers
{

    public class MediaController : Controller
    {
        private readonly IMediaService _mediaService;

        public MediaController(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        [HttpGet("/Media/{id}")]
        public async Task<FileResult> Index(long id)
        {
            if (id == (long)(long)EMediaKey.Faulty)
            {
                return File("~/images/404.jpg", "image/jpg");
            }
            if (id == (long)(long)EMediaKey.None)
            {
                return File("~/images/none.png", "image/jpg");
            }
            if (id == (long)(long)EMediaKey.Group)
            {
                return File("~/images/group.svg", "image/jpg");
            }
            if (id == (long)(long)EMediaKey.Person)
            {
                return File("~/images/person.jpg", "image/jpg");
            }


            var response = await _mediaService.Get(id);
            if (response.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return File(response.Result.File, response.Result.ContentType, response.Result.FileName);
            }

            return File("~/images/404.jpg", "image/jpg");
        }


        [HttpGet("/Media/")]
        public async Task<FileResult> Index()
        {
            //?ids=1,2,3
            var ids=Request.Query["ids"].ToString().Split(',').Select(x=>Convert.ToInt32(x));
            var files = new List<MediaListDto>();
            foreach (var id in ids)
            {
               var result= await _mediaService.Get(id);
                if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
                {

                files.Add(result.Result);
                }
            }
            var memoryStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var entry = zipArchive.CreateEntry(file.FileName);
                    using (var entryStream = entry.Open())
                    {
                        entryStream.Write(file.File, 0, file.File.Length);
                    }
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);
            return File(memoryStream, "application/zip", "Files.zip");
        }


    }
}
