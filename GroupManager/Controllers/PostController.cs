﻿using System;
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
    public class PostController : Controller
    {
        private readonly IGroupRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(IGroupRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        public IActionResult AddPost(string eventId)
        {
            return View(new AddPostViewModel() { EventId = eventId });
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
            if (string.IsNullOrEmpty(model.EventId))
            {
                _repository.AddPost(post, _repository.GetUser(Guid.Parse(user.Id)).ActiveGroup.Id, Guid.Parse(user.Id));
                return Redirect("/home");
            }
            else
            {
                _repository.AddEventPost(post, Guid.Parse(model.EventId), Guid.Parse(user.Id));
                return Redirect("/event/Details?eventId=" + model.EventId);
            }
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
            post = _repository.GetPost(post.Id);
            if (post.Event != null)
            {
                return Redirect("/event/Details?eventId=" + post.Event.Id);
            }
            return Redirect("/home");
        }

        public async Task<IActionResult> Index(string postId)
        {
            ApplicationUser appUser = await _userManager.GetUserAsync(HttpContext.User);
            User user = _repository.GetUser(Guid.Parse(appUser.Id));
            Post post = _repository.GetPost(Guid.Parse(postId));

            if (post == null || post.Group.Id != user.ActiveGroup.Id)
            {
                return Forbid();
            }

            PostViewModel model = new PostViewModel()
            {
                Comments = _repository.GetComments(post.Id).Select(comment => new CommentViewModel()
                {
                    Id = comment.Id.ToString(),
                    PostId = post.Id.ToString(),
                    Text = comment.Text,
                    User = comment.User.Id.ToString(),
                    TimePosted = comment.CreatedOn,
                    TimeModified = comment.ModifiedOn
                }).ToList(),
                Post = new IndexPostViewModel()
                {
                    Id = post.Id.ToString(),
                    Title = post.Title,
                    Text = post.Text,
                    User = post.User.Id.ToString(),
                    TimePosted = post.CreatedOn,
                    TimeModified = post.ModifiedOn,
                    Event = post.Event?.Id.ToString()
                }
            };

            return View(model);
        }
    }
}