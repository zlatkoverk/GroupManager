using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace GroupManager.Models.GroupViewModels
{
    public class PostViewModel
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
    }
}