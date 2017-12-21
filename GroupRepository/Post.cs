using System;
using System.Collections.Generic;

namespace GroupRepository
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public List<Comment> Comments { get; set; }
        public User User { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Post))
            {
                return false;
            }
            Post other = (Post)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Post post1, Post post2)
        {
            if (ReferenceEquals(post1, post2))
            {
                return true;
            }
            if (ReferenceEquals(post1, null) || ReferenceEquals(post2, null))
            {
                return false;
            }
            return post1.Equals(post2);
        }

        public static bool operator !=(Post post1, Post post2)
        {
            return !(post1 == post2);
        }
    }
}