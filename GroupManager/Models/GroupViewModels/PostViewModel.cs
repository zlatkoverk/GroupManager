using System.Collections.Generic;

namespace GroupManager.Models.GroupViewModels
{
    public class PostViewModel
    {
        public IndexPostViewModel Post { get; set; }
        public List<CommentViewModel> Comments { get; set; }
    }
}