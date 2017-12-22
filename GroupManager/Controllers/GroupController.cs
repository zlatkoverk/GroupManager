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
using Microsoft.AspNetCore.Routing;

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

        public IActionResult AddPost()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPost(AddPostViewModel model)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            Post post = new Post(model.Title, model.Text);
            _repository.AddPost(post, _repository.GetUser(Guid.Parse(user.Id)).ActiveGroup.Id, Guid.Parse(user.Id));
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditPost(string postId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            User u = _repository.GetUser(Guid.Parse(user.Id));
            Post post = _repository.GetPosts(u.ActiveGroup.Id)
                .FirstOrDefault(p => p.Id == Guid.Parse(postId));

            PostViewModel model = new PostViewModel()
            {
                Id = post.Id.ToString(),
                Text = post.Text,
                Title = post.Title,
                TimeModified = post.ModifiedOn,
                TimePosted = post.CreatedOn,
                User = post.User.Email
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditPost(PostViewModel model)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            User u = _repository.GetUser(Guid.Parse(user.Id));
            if (!ModelState.IsValid || !u.Posts.Contains(new Post() {Id = Guid.Parse(model.Id)}))
            {
                return RedirectToAction("error");
            }
            Post post = new Post(model.Title, model.Text);
            post.Id = Guid.Parse(model.Id);
            _repository.UpdatePost(post);
            return RedirectToAction("Index");
        }
    }
}