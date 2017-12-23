using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupManager.Models;
using GroupRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupManager.Controllers
{
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
    }
}