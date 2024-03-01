﻿using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.ListDtos;
using AcademicFileSharingProject.Dtos.LoadMoreDtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/Academician")]
    public class AcademicianController : Controller
    {
        private readonly IUserRoleService _userRoleService;
        private readonly IUserService _userService;
        private readonly ISubscribeService _subscribeService;
        private readonly IPostService _postService;
        private readonly IToastNotification _toastNotification;
       
        public AcademicianController(IToastNotification toastNotification, IUserRoleService userRoleService, IUserService userService, IPostService postService, ISubscribeService subscribeService)
        {
            _toastNotification = toastNotification;
            _userRoleService = userRoleService;
            _userService = userService;
            _postService = postService;
            _subscribeService = subscribeService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index([FromQuery] int? page)
        {
            var result = await _userRoleService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.UserRoleFilter>
            {
                ContentCount = 15,
                PageCount = page ?? 0,
                Filter = new Dtos.Filters.UserRoleFilter
                {
                    Role = Entities.Enums.ERoles.Academician
                }
            });
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                var list = result.Result.Values.Select(x => x.User).ToList();
                return View(new GenericLoadMoreDto<UserListDto>
                {
                    Values = list,
                    ContentCount = result.Result.ContentCount,
                    PageCount = result.Result.PageCount,
                    NextPage = result.Result.NextPage,
                    PrevPage = result.Result.PrevPage,
                    TotalContentCount = result.Result.TotalContentCount,
                    TotalPageCount = result.Result.TotalPageCount

                });
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Profile(long id, [FromQuery] int? page)
        {

            var result = await _userService.Get(id);
            var postResult = await _postService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.PostFilter>
            {
                ContentCount = 10,
                PageCount = page ?? 0,
                Filter = new Dtos.Filters.PostFilter
                {
                    UserId = id,
                }
            });
            if (postResult.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                ViewBag.Posts = postResult.Result;
            }
            else
            {
                var message2 = string.Join(Environment.NewLine, postResult.ErrorMessages.Select(x => x.Message).ToList());
                _toastNotification.AddErrorToastMessage(message2);
            }
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(result.Result);
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> AddSubscribe(long userId)
        {
            var result = await _subscribeService.Add(new Dtos.AddOrUpdateDtos.SubscribeDto
            {
                CreatedTime = DateTime.Now,
                UserID = 1,
                SubscribeUserID = userId,
            });
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Profile");
            }

            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return RedirectToAction("Profile");
        }


    }
}
