using System;

namespace GroupRepository
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Post Post { get; set; }
        public User User { get; set; }


        public override bool Equals(object obj)
        {
            if (!(obj is Comment))
            {
                return false;
            }
            Comment other = (Comment)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Comment comment1, Comment comment2)
        {
            if (ReferenceEquals(comment1, comment2))
            {
                return true;
            }
            if (ReferenceEquals(comment1, null) || ReferenceEquals(comment2, null))
            {
                return false;
            }
            return comment1.Equals(comment2);
        }

        public static bool operator !=(Comment comment1, Comment comment2)
        {
            return !(comment1 == comment2);
        }
    }
}