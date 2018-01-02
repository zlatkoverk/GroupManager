using System.ComponentModel.DataAnnotations;
using GroupRepository;

namespace GroupManager.Models.GroupViewModels
{
    public class AddPostViewModel
    {
        public string EventId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Text { get; set; }
    }
}