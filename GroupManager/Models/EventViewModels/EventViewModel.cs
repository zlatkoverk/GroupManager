using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GroupManager.Models.GroupViewModels;
using GroupManager.Models.UserViewModels;

namespace GroupManager.Models.EventViewModels
{
    public class EventViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Text { get; set; }
        [Required]
        public DateTime Time { get; set; }
        public List<UserViewModel> UsersAttending { get; set; }
        public List<UserViewModel> UsersNotAttending { get; set; }
        public List<UserViewModel> UsersInvited { get; set; }
        public string CurrentUserStatus { get; set; }
        public List<IndexPostViewModel> Posts { get; set; }
    }
}