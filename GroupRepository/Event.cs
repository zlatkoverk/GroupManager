using System;
using System.Collections.Generic;

namespace GroupRepository
{
    public class Event
    {
        public Guid Id { get; set; }
        public Group Group { get; set; }
        public List<User> UsersAttending { get; set; }
        public List<User> UsersNotAttending { get; set; }
        public List<User> UsersInvited { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public List<Post> Posts { get; set; }

        public Event(string name, string text, DateTime time)
        {
            Id = Guid.NewGuid();
            Name = name;
            Text = text;
            Time = time;
            UsersAttending = new List<User>();
            UsersInvited = new List<User>();
            UsersNotAttending = new List<User>();
            Posts = new List<Post>();
        }

        public Event() { }

        public override bool Equals(object obj)
        {
            if (!(obj is Event))
            {
                return false;
            }
            Event other = (Event)obj;
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(Event event1, Event event2)
        {
            if (ReferenceEquals(event1, event2))
            {
                return true;
            }
            if (event1 is null || event2 is null)
            {
                return false;
            }
            return event1.Equals(event2);
        }

        public static bool operator !=(Event event1, Event event2)
        {
            return !(event1 == event2);
        }
    }
}