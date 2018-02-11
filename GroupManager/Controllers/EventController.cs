using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupManager.Models;
using GroupManager.Models.EventViewModels;
using GroupManager.Models.GroupViewModels;
using GroupManager.Models.UserViewModels;
using GroupRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupManager.Controllers
{
    [Authorize]
    public class EventController : Controller
    {
        private readonly IGroupRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventController(IGroupRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> List()
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            if (user.ActiveGroup == null)
            {
                return Forbid();
            }

            EventListViewModel model = new EventListViewModel()
            {
                Events = _repository.GetEvents(user.ActiveGroup.Id).Select(e => new EventViewModel()
                {
                    Id = e.Id.ToString(),
                    Name = e.Name,
                    Text = e.Text,
                    Time = e.Time,
                    Upcoming = DateTime.Now.CompareTo(e.Time) < 0
                }).ToList()
            };

            return View(model);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(EventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            _repository.AddGroupEvent(model.Name, model.Text, model.Time, user.ActiveGroup.Id);

            return RedirectToAction("List");
        }

        public async Task<IActionResult> Delete(string eventId)
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            _repository.RemoveEvent(Guid.Parse(eventId));

            return RedirectToAction("List");
        }

        public async Task<IActionResult> Edit(string eventId)
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            Event e = _repository.GetEvents(user.ActiveGroup.Id).FirstOrDefault(ev => ev.Id.ToString() == eventId);
            if (e == null)
            {
                return Forbid();

            }

            EventViewModel model = new EventViewModel()
            {
                Id = e.Id.ToString(),
                Name = e.Name,
                Text = e.Text,
                Time = e.Time
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            Event e = new Event()
            {
                Id = Guid.Parse(model.Id),
                Name = model.Name,
                Text = model.Text,
                Time = model.Time
            };
            _repository.UpdateEvent(e);

            return RedirectToAction("List");
        }

        public async Task<IActionResult> Details(string eventId)
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            Event e = _repository.GetEvents(user.ActiveGroup.Id).FirstOrDefault(ev => ev.Id.ToString() == eventId);
            if (e == null)
            {
                return Forbid();
            }

            Func<GroupRepository.User, UserViewModel> mapperFunc = u => new UserViewModel()
            {
                Id = u.Id.ToString(),
                Nick = _repository.GetNick(u.Id, user.ActiveGroup.Id),
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                PictureURI = u.Picture
            };

            EventViewModel model = new EventViewModel()
            {
                Id = e.Id.ToString(),
                Name = e.Name,
                Text = e.Text,
                Time = e.Time,
                UsersAttending = e.UsersAttending.Select(mapperFunc).ToList(),
                UsersNotAttending = e.UsersNotAttending.Select(mapperFunc).ToList(),
                UsersInvited = e.UsersInvited.Select(mapperFunc).ToList(),
                CurrentUserStatus = e.UsersAttending.Contains(user) ? "ATTENDING" : e.UsersNotAttending.Contains(user) ? "NOT_ATTENDING" : "INVITED",
                Posts = _repository.GetEventPosts(e.Id).Select(p => new IndexPostViewModel()
                {
                    Id = p.Id.ToString(),
                    User = p.User.Id.ToString(),
                    Text = p.Text,
                    TimeModified = p.ModifiedOn,
                    TimePosted = p.CreatedOn,
                    Title = p.Title,
                    CurrentUserCanEdit = p.User == user
                }).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> SetStatus(string eventId, string status)
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            Event e = _repository.GetEvents(user.ActiveGroup.Id).FirstOrDefault(ev => ev.Id.ToString() == eventId);
            if (e == null)
            {
                return Forbid();

            }

            switch (status)
            {
                case "ATTENDING":
                    _repository.SetAttending(user.Id, e.Id);
                    break;
                case "NOT_ATTENDING":
                    _repository.SetNotAttending(user.Id, e.Id);
                    break;
                case "INVITED":
                    _repository.SetInvited(user.Id, e.Id);
                    break;
                default:
                    return Forbid();
            }

            return RedirectToAction("Details", new { eventId = e.Id });
        }

        public IActionResult AddPost(string eventId)
        {
            return RedirectToAction("AddPost", "Post", new { eventId = eventId });
        }
    }
}