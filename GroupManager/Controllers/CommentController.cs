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
    public class CommentController : Controller
    {
        private readonly IGroupRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(IGroupRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(string postId, string text)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            Comment comment = new Comment(text);
            _repository.AddComment(comment, Guid.Parse(user.Id), Guid.Parse(postId));

            return Redirect("/Post?postId=" + postId);
        }

        public async Task<IActionResult> Edit(string commentId)
        {
            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);

            Comment comment = _repository.GetComment(Guid.Parse(commentId));
            if (comment.User.Id != _repository.GetUser(Guid.Parse(user.Id)).Id)
            {
                return Forbid();
            }

            CommentViewModel model = new CommentViewModel()
            {
                Id = comment.Id.ToString(),
                Text = comment.Text,
                PostId = comment.Post.Id.ToString()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
            Comment comment = _repository.GetComment(Guid.Parse(model.Id));

            if (comment.User.Id != _repository.GetUser(Guid.Parse(user.Id)).Id)
            {
                return Forbid();
            }

            comment.Text = model.Text;
            _repository.UpdateComment(comment);
            return Redirect("/Post?postId=" + model.PostId);
        }
    }
}