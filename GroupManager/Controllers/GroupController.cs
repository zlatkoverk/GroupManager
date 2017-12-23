using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupManager.Models;
using GroupManager.Models.GroupViewModels;
using GroupRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupManager.Controllers
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

            GroupIndexViewModel model = new GroupIndexViewModel
            {
                Groups = _repository.GetGroups(Guid.Parse(user.Id)) ?? new List<Group>(),
                ActiveGroup = _repository.GetUser(Guid.Parse(user.Id)).ActiveGroup
            };
            return View(model);
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
            _repository.AddGroup(group, Guid.Parse(user.Id));
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> SetActive(string groupId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            _repository.SetActive(Guid.Parse(user.Id), Guid.Parse(groupId));
            return RedirectToAction("Index");
        }

        

        public IActionResult AddUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddUser(AddUserViewModel model)
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Todo permission
            User addUser = _repository.GetUser(model.UserEmail);
            if (addUser == null)
            {
                model.Message = "User with that e-mail does not exist";
                return View(model);
            }

            _repository.AddUserToGroup(addUser.Id, user.ActiveGroup.Id);
            return Redirect("/home");
        }

        public async Task<IActionResult> SetNick()
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));
            ChangeNickViewModel model = new ChangeNickViewModel() { Nick = _repository.GetNick(user.Id, user.ActiveGroup.Id) };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SetNick(ChangeNickViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            _repository.SetNick(model.Nick, user.Id, user.ActiveGroup.Id);
            return Redirect("Index");
        }
    }
}