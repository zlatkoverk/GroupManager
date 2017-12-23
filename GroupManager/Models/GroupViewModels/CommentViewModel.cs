using System;
using System.ComponentModel.DataAnnotations;
using GroupRepository;

namespace GroupManager.Models.GroupViewModels
{
    public class CommentViewModel
    {
        public string Id { get; set; }
        public string PostId { get; set; }
        [Required]
        public string Text { get; set; }
        public string User { get; set; }
        public DateTime TimePosted { get; set; }
        public DateTime? TimeModified { get; set; }

        public CommentViewModel() { }
    }
}