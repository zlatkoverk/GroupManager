using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GroupManager.Models;
using GroupManager.Models.GroupViewModels;
using GroupRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupManager.Controllers.Groups
{
    [Authorize]
    public class GroupController : Controller
    {

        private readonly IGroupRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public GroupController(IGroupRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            
            return View(new GroupIndexViewModel(){Groups = _repository.GetGroups(new Guid(user.Id))});
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddGroupViewModel model)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Group group = new Group(model.Name);
            _repository.AddGroup(group, new Guid(user.Id));
            return RedirectToAction("Index");
        }
    }
}