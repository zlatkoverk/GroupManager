using Microsoft.AspNetCore.Http;

namespace GroupManager.Models.UserViewModels
{
    public class UserUploadModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public IFormFile Picture { get; set; }
        public string PictureURI { get; set; }
    }
}