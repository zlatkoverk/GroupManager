using System;

namespace GroupRepository
{
    public class Invite
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Message { get; set; }
    }
}