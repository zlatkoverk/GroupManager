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
            return Redirect("/home");
        }

        public async Task<IActionResult> EditPost(string postId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            User u = _repository.GetUser(Guid.Parse(user.Id));
            Post post = _repository.GetPosts(u.ActiveGroup.Id)
                .FirstOrDefault(p => p.Id == Guid.Parse(postId));

            if (post == null)
            {
                return NoContent();
            }
            if (post.User.Id != u.Id)
            {
                return Forbid();
            }

            IndexPostViewModel model = new IndexPostViewModel()
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
        public async Task<IActionResult> EditPost(IndexPostViewModel model)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            User u = _repository.GetUser(Guid.Parse(user.Id));
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //Todo permission
            if (!u.Posts.Contains(new Post() { Id = Guid.Parse(model.Id) }))
            {
                return Forbid();
            }
            Post post = new Post(model.Title, model.Text) { Id = Guid.Parse(model.Id) };
            _repository.UpdatePost(post);
            return Redirect("/home");
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

        //[HttpGet("{postId}")]
        public async Task<IActionResult> Post(string postId)
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));
            Post post = _repository.GetPosts(user.ActiveGroup.Id)
                .FirstOrDefault(p => p.Id == Guid.Parse(postId));

            PostViewModel model = new PostViewModel()
            {
                Comments = post.Comments.OrderBy(comment => comment.CreatedOn).Select(comment => new CommentViewModel(comment)).ToList(),
                Post = new IndexPostViewModel(post)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(string postId, string text)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            Comment comment = new Comment(text);
            _repository.AddComment(comment, Guid.Parse(user.Id), Guid.Parse(postId));

            return Redirect("Post?postId=" + postId);
        }
    }
}