using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GroupManager.Models;
using GroupManager.Models.BalanceViewModels;
using GroupRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupManager.Controllers
{
    [Authorize]
    public class BalanceController : Controller
    {
        private readonly IGroupRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public BalanceController(IGroupRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            if (user.ActiveGroup == null)
            {
                return Forbid();
            }

            List<BalanceEntry> entries = _repository.GetBalanceEntries(user.ActiveGroup.Id);
            BalanceIndexViewModel model = new BalanceIndexViewModel()
            {
                Entries = entries.Select(e => new BalanceEntryViewModel()
                {
                    User = _repository.GetNick(e.User.Id, user.ActiveGroup.Id),
                    Value = e.Value,
                    Message = e.Message,
                    Time = e.Time,
                    Id = e.Id.ToString()
                }).OrderByDescending(e => e.Time).ToList(),
                CurrentBalance = entries.Sum(e => e.Value)
            };

            return View(model);
        }

        public IActionResult Add()
        {
            return View(new BalanceEntryViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(BalanceEntryViewModel model)
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Value <= 0)
            {
                ModelState.AddModelError("Value", "Value must be greater than zero");
                return View(model);
            }

            _repository.AddBalanceEntry(user.Id, user.ActiveGroup.Id, model.Value, model.Message);
            return RedirectToAction("Index");
        }

        public IActionResult Retrieve()
        {
            return View(new BalanceEntryViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Retrieve(BalanceEntryViewModel model)
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Value <= 0)
            {
                ModelState.AddModelError("Value", "Value must be greater than zero");
                return View(model);
            }

            _repository.AddBalanceEntry(user.Id, user.ActiveGroup.Id, -model.Value, model.Message);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string entryId)
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));

            Guid id;
            if (!Guid.TryParse(entryId, out id))
            {
                return Forbid();
            }

            _repository.DeleteBalanceEntry(id);
            return RedirectToAction("Index");
        }
    }
}