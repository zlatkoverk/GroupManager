using System;
using System.Threading.Tasks;
using GroupManager.Models;
using GroupManager.Models.UserViewModels;
using GroupRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GroupManager.ViewComponents
{
    public class UserViewComponent : ViewComponent
    {
        private readonly IGroupRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserViewComponent(IGroupRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            User user = _repository.GetUser(Guid.Parse(userId));
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            return View(new UserViewModel()
            {
                Email = user.Email,
                Id = user.Id.ToString(),
                Name = user.Name,
                PictureURI = user.Picture,
                Surname = user.Surname,
                Nick = _repository.GetNick(Guid.Parse(userId), _repository.GetUser(Guid.Parse(userId)).ActiveGroup.Id)
            });
        }
    }
}