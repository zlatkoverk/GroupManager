using System.ComponentModel.DataAnnotations;

namespace GroupManager.Models.GroupViewModels
{
    public class AddUserViewModel
    {
        [EmailAddress]
        [Required]
        public string UserEmail { get; set; }

        public string Message { get; set; }
    }
}