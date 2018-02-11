using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupManager.Models;
using GroupManager.Models.UserViewModels;
using GroupManager.Utilities;
using GroupRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GroupManager.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IGroupRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;


        public UserController(IGroupRepository repository, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _repository = repository;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));
            UserUploadModel model = new UserUploadModel()
            {
                Name = user.Name,
                Surname = user.Surname,
                PictureURI = user.Picture
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserUploadModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            user.Name = model.Name;
            user.Surname = model.Surname;

            if (model.Picture != null)
            {
                byte[] imageBytes;
                using (var stream = new MemoryStream())
                {
                    await model.Picture.CopyToAsync(stream);
                    imageBytes = stream.ToArray();
                }

                var uploader = new AzureStorageUtility(_configuration["storageAccountName"],
                    _configuration["storageAccountKey"]);
                user.Picture = await uploader.Upload(_configuration["storageContainerName"], imageBytes);
            }

            _repository.UpdateUser(user);

            return Redirect("/group");
        }

        public async Task<IActionResult> List()
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            if (user.ActiveGroup == null)
            {
                Redirect("/group");
            }

            return View(_repository.GetUsers(user.ActiveGroup.Id).Select(u => new UserViewModel()
            {
                Email = u.Email,
                Id = u.Id.ToString(),
                Name = u.Name,
                Nick = _repository.GetNick(u.Id, user.ActiveGroup.Id),
                Surname = u.Surname
            }).ToList());
        }

        public async Task<IActionResult> Details(string userId)
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User currentUser = _repository.GetUser(Guid.Parse(appUser.Id));
            User user = _repository.GetUser(Guid.Parse(userId));
            if (user == null)
            {
                return Forbid();
            }

            UserViewModel model = new UserViewModel()
            {
                Email = user.Email,
                Id = userId,
                Name = user.Name,
                Nick = _repository.GetNick(user.Id, currentUser.ActiveGroup.Id),
                PictureURI = user.Picture,
                Surname = user.Surname
            };

            return View(model);
        }
    }
}