using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Internal;

namespace GroupRepository
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public List<Role> Roles { get; set; }
        public List<Post> Posts { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Group> Groups { get; set; }
        public Group ActiveGroup { get; set; }

        public User(string email, string id)
        {
            Email = email;
            Roles = new List<Role>();
            Posts = new List<Post>();
            Comments = new List<Comment>();
            Groups = new List<Group>();
            Id = Guid.Parse(id);
        }

        public User()
        {
        }

        public override bool Equals(object obj)
        {
            if (!(obj is User))
            {
                return false;
            }
            User other = (User)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(User usr1, User usr2)
        {
            if (ReferenceEquals(usr1, usr2))
            {
                return true;
            }
            if (ReferenceEquals(usr1, null) || ReferenceEquals(usr2, null))
            {
                return false;
            }
            return usr1.Equals(usr2);
        }

        public static bool operator !=(User usr1, User usr2)
        {
            return !(usr1 == usr2);
        }

        public override string ToString()
        {
            return Email;
        }
    }
}