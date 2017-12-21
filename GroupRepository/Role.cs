using System;
using System.Collections.Generic;

namespace GroupRepository
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Post { get; set; }
        public bool Comment { get; set; }
        public bool ApproveUsers { get; set; }
        public List<User> Users { get; set; }
        public Group Group { get; set; }

        public Role()
        {
        }

        public Role(string name, Group group)
        {
            Name = name;
            Group = group;
            Users = new List<User>();
            Id = Guid.NewGuid();
        }
    }
}