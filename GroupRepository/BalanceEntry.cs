using System;

namespace GroupRepository
{
    public class BalanceEntry
    {
        public Guid Id { get; set; }
        public double Value { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public Group Group { get; set; }
        public User User { get; set; }

        public BalanceEntry(User user, Group group, double value, string message)
        {
            User = user;
            Group = group;
            Value = value;
            Message = message;
            Id = Guid.NewGuid();
            Time = DateTime.Now;
        }

        public BalanceEntry() { }

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

        public static bool operator ==(BalanceEntry entry1, BalanceEntry entry2)
        {
            if (ReferenceEquals(entry1, entry2))
            {
                return true;
            }
            if (ReferenceEquals(entry1, null) || ReferenceEquals(entry2, null))
            {
                return false;
            }
            return entry1.Equals(entry2);
        }

        public static bool operator !=(BalanceEntry entry1, BalanceEntry entry2)
        {
            return !(entry1 == entry2);
        }
    }
}