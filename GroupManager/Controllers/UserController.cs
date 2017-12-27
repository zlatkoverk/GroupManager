using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GroupManager.Models;
using GroupManager.Models.UserViewModels;
using GroupRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupManager.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly IGroupRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IGroupRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));
            UserViewModel model = new UserViewModel()
            {
                Name = user.Name,
                Surname = user.Surname
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            user.Name = model.Name;
            user.Surname = model.Surname;

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
    }
}