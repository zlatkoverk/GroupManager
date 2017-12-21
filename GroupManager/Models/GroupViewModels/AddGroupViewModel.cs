using System.ComponentModel.DataAnnotations;

namespace GroupManager.Models.GroupViewModels
{
    public class AddGroupViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}