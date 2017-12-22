using System.Collections.Generic;
using GroupRepository;

namespace GroupManager.Models.GroupViewModels
{
    public class GroupIndexViewModel
    {
        public List<Group> Groups { get; set; }
        public Group ActiveGroup { get; set; }
    }
}