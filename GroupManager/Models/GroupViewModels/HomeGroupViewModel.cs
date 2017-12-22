using System.Collections.Generic;
using GroupRepository;

namespace GroupManager.Models.GroupViewModels
{
    public class HomeGroupViewModel
    {
        public bool AnyActive { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public List<PostViewModel> Posts { get; set; }
    }
}