﻿using System;
using System.Collections.Generic;

namespace GroupRepository
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public List<Role> Roles { get; set; }
        public List<Post> Posts { get; set; }
        public List<Event> Events { get; set; }

        public Group()
        {
        }

        public Group(string name)
        {
            Name = name;
            Users = new List<User>();
            Roles = new List<Role>();
            Posts = new List<Post>();
            Events = new List<Event>();
            Id = Guid.NewGuid();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Group))
            {
                return false;
            }
            Group other = (Group)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Group group1, Group group2)
        {
            if (ReferenceEquals(group1, group2))
            {
                return true;
            }
            if (group1 is null || group2 is null)
            {
                return false;
            }
            return group1.Equals(group2);
        }

        public static bool operator !=(Group group1, Group group2)
        {
            return !(group1 == group2);
        }
    }
}