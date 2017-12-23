using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GroupManager.Models;
using GroupManager.Models.GroupViewModels;
using GroupRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace GroupManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly IGroupRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IGroupRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));
            HomeGroupViewModel model = new HomeGroupViewModel();
            if (user.ActiveGroup == null)
            {
                model.AnyActive = false;
                return View(model);
            }
            model.AnyActive = true;
            model.Name = user.ActiveGroup.Name;
            List<Post> posts = _repository.GetPosts(user.ActiveGroup.Id);
            if (posts != null)
            {
                model.Posts = posts.Select(post => new IndexPostViewModel()
                {
                    Id = post.Id.ToString(),
                    Text = post.Text,
                    Title = post.Title,
                    TimePosted = post.CreatedOn,
                    TimeModified = post.ModifiedOn,
                    User = _repository.GetNick(post.User.Id, user.ActiveGroup.Id)
                }).ToList();
            }
            else
            {
                model.Posts=new List<IndexPostViewModel>();
            }
            return View(model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
