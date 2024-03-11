using AcademicFileSharingProject.Business.Abstract;
using AcademicFileSharingProject.Dtos.AddOrUpdateDtos;
using AcademicFileSharingProject.Entities.Enums;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NToastNotify;

namespace AcademicFileSharingProject.WebUI.Controllers
{
    [Route("/Admin/Software")]
    public class AdminSoftwareController : Controller
    {
        private readonly ISoftwareService _softwareService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly List<EMethod> userMethods;
        private readonly IToastNotification _toastNotification;
        private readonly IAccountService _accountService;
        private readonly long? loginUserId = null;


        public AdminSoftwareController(ISoftwareService softwareService, IToastNotification toastNotification, IMapper mapper, IHttpContextAccessor contextAccessor, IAccountService accountService)
        {
            _softwareService = softwareService;
            _toastNotification = toastNotification;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _accountService = accountService;
            userMethods = new List<EMethod>();

            var session = _contextAccessor.HttpContext.Request.Cookies["SessionKey"];
            if (session != null)
            {
                var result = _accountService.GetSession(session);
                result.Wait();
                if (result.Result.ResultStatus == Dtos.Enums.ResultStatus.Success)
                {
                    if (result.Result.Result == null)
                    {
                        session = null;
                    }
                    else
                    {
                        loginUserId = result.Result.Result.UserId;
                        var roleResult = _accountService.GetUserRoleMethods(result.Result.Result.UserId);
                        roleResult.Wait();
                        if (roleResult.Result.ResultStatus == Dtos.Enums.ResultStatus.Success)
                        {
                            userMethods = roleResult.Result.Result;
                            ViewBag.CanUpdate = !(!userMethods.Contains(EMethod.SoftwareUpdate) || !userMethods.Contains(EMethod.SoftwareAllUpdate));
                            ViewBag.CanDelete = !(!userMethods.Contains(EMethod.SoftwareRemove) || !userMethods.Contains(EMethod.SoftwareAllRemove));

                        }
                        else
                        {
                            var message = string.Join(Environment.NewLine, roleResult.Result.ErrorMessages.Select(m => m.Message));
                            _toastNotification.AddErrorToastMessage(message);
                        }

                    }
                }
                else
                {
                    var message = string.Join(Environment.NewLine, result.Result.ErrorMessages.Select(m => m.Message));
                    _toastNotification.AddErrorToastMessage(message);
                }
            }
            if (session == null)
            {
                _toastNotification.AddAlertToastMessage("Bu kısma girmek için yetkiniz yoktur");

                _contextAccessor.HttpContext.Response.Redirect("/");
            }
        }

        public async Task<IActionResult> Index()
        {

            if (!userMethods.Contains(EMethod.SoftwareAllList) || !userMethods.Contains(EMethod.SoftwareList))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _softwareService.GetAll(new Dtos.Filters.LoadMoreFilter<Dtos.Filters.SoftwareFilter>
            {
                ContentCount = 10,
                PageCount = 0,
                Filter = new Dtos.Filters.SoftwareFilter
                {
                    UserId = (!userMethods.Contains(EMethod.SoftwareAllList)) ? loginUserId : null
                }
            });
            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(result.Result);
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);

            return View();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Detail(long id)
        {
            if (!userMethods.Contains(EMethod.SoftwareDetail))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _softwareService.Get(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return View(result.Result);
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();


        }


        [HttpGet("Update/{id}")]
        public async Task<IActionResult> Update(long id)
        {

            var result = await _softwareService.Get(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((result?.Result?.UserId != loginUserId && !userMethods.Contains(EMethod.SoftwareAllRemove)) || !userMethods.Contains(EMethod.SoftwareRemove))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                return View(result.Result);
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();


        }

        [HttpPost("Update/{id}")]
        public async Task<IActionResult> Update(SoftwareDto software)
        {
            if ((software.UserId != loginUserId && !userMethods.Contains(EMethod.SoftwareAllUpdate)) || !userMethods.Contains(EMethod.SoftwareUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _softwareService.Update(software);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();


        }


        [HttpGet("Add")]
        public async Task<IActionResult> Add()
        {
            if (!userMethods.Contains(EMethod.SoftwareAdd))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            return View();
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add(SoftwareDto software)
        {
            if (!userMethods.Contains(EMethod.SoftwareAdd))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            software.UserId = (long)loginUserId;

            var result = await _softwareService.Add(software);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View(software);


        }


        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {

            if (((await _softwareService.Get(id))?.Result?.UserId != loginUserId && !userMethods.Contains(EMethod.SoftwareAllRemove)) || !userMethods.Contains(EMethod.SoftwareRemove))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }

            var result = await _softwareService.Delete(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return RedirectToAction("Index");
        }


        [HttpGet("ChangeLogo/{id}")]
        public async Task<IActionResult> ChangeLogo(long id)
        {
            var result = await _softwareService.Get(id);


            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((result.Result.UserId != loginUserId && !userMethods.Contains(EMethod.SoftwareAllUpdate)) || !userMethods.Contains(EMethod.SoftwareUpdate))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                return View(new SoftwareDto
                {
                    Id = id,

                });
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }


        [HttpGet("ChangeFile/{id}")]
        public async Task<IActionResult> ChangeFile(long id)
        {
            var result = await _softwareService.Get(id);


            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((result.Result.UserId != loginUserId && !userMethods.Contains(EMethod.SoftwareAllUpdate)) || !userMethods.Contains(EMethod.SoftwareUpdate))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                return View(new SoftwareDto
                {
                    Id = id,

                });
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }
        [HttpGet("ChangeDocument/{id}")]
        public async Task<IActionResult> ChangeDocument(long id)
        {
            var result = await _softwareService.Get(id);


            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((result.Result.UserId != loginUserId && !userMethods.Contains(EMethod.SoftwareAllUpdate)) || !userMethods.Contains(EMethod.SoftwareUpdate))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                return View(new SoftwareDto
                {
                    Id = id,

                });
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View();
        }


        [HttpPost("ChangeLogo/{id}")]
        public async Task<IActionResult> ChangeLogo(SoftwareDto software)
        {
            if ((software.UserId != loginUserId && !userMethods.Contains(EMethod.SoftwareAllUpdate)) || !userMethods.Contains(EMethod.SoftwareUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _softwareService.ChangeLogo(software);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View(software);


        }



        [HttpPost("ChangeFile/{id}")]
        public async Task<IActionResult> ChangeFile(SoftwareDto software)
        {
            if ((software.UserId != loginUserId && !userMethods.Contains(EMethod.SoftwareAllUpdate)) || !userMethods.Contains(EMethod.SoftwareUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _softwareService.ChangeFile(software);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View(software);


        }



        [HttpPost("ChangeDocument/{id}")]
        public async Task<IActionResult> ChangeDocument(SoftwareDto software)
        {
            if ((software.UserId != loginUserId && !userMethods.Contains(EMethod.SoftwareAllUpdate)) || !userMethods.Contains(EMethod.SoftwareUpdate))
            {
                _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                return Redirect("/");
            }
            var result = await _softwareService.ChangeDocument(software);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return View(software);


        }




        [HttpGet("ChangeIsAir/{id}")]
        public async Task<IActionResult> ChangeIsAir(long id)
        {

            var result = await _softwareService.Get(id);

            if (result.ResultStatus == Dtos.Enums.ResultStatus.Success)
            {
                if ((result.Result.UserId != loginUserId && !userMethods.Contains(EMethod.SoftwareAllUpdate)) || !userMethods.Contains(EMethod.SoftwareUpdate))
                {
                    _toastNotification.AddAlertToastMessage("Yetkiniz Bulunmamaktadır");
                    return Redirect("/");
                }
                result.Result.IsAir = !result.Result.IsAir;
                await Update(_mapper.Map<SoftwareDto>(result.Result));
                return RedirectToAction("Index");
            }
            var message = string.Join(Environment.NewLine, result.ErrorMessages.Select(x => x.Message).ToList());
            _toastNotification.AddErrorToastMessage(message);
            return RedirectToAction("Index");
        }

    }
}
