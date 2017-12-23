using System;

namespace GroupRepository
{
    public class Nick
    {
        public Guid Id { get; set; }
        public Group Group { get; set; }
        public User User { get; set; }
        public string Value { get; set; }

        public Nick(string nickValue, User user, Group group)
        {
            Value = nickValue;
            Id = Guid.NewGuid();
            User = user;
            Group = group;
        }

        public Nick() { }
    }
}