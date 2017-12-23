using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GroupRepository;
using Microsoft.AspNetCore.Mvc;

namespace GroupManager.Models.GroupViewModels
{
    public class IndexPostViewModel
    {
        [HiddenInput]
        public string Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
        public string User { get; set; }
        public DateTime TimePosted { get; set; }
        public DateTime? TimeModified { get; set; }

        public IndexPostViewModel(Post post)
        {
            Id = post.Id.ToString();
            Title = post.Title;
            Text = post.Text;
            User = post.User.Email;
            TimePosted = post.CreatedOn;
            TimeModified = post.ModifiedOn;
        }

        public IndexPostViewModel() { }
    }
}