using System.ComponentModel.DataAnnotations;

namespace GroupManager.Models.GroupViewModels
{
    public class ChangeNickViewModel
    {
        [Required]
        public string Nick { get; set; }
        public string Message { get; set; }
    }
}