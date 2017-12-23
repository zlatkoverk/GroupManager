using System.Data.Entity;

namespace GroupRepository
{
    public class GroupDbContext : DbContext
    {
        public GroupDbContext(string cnnstr) : base(cnnstr)
        {
        }

        public IDbSet<User> Users { get; set; }
        public IDbSet<Role> Roles { get; set; }
        public IDbSet<Post> Posts { get; set; }
        public IDbSet<Comment> Comments { get; set; }
        public IDbSet<Group> Groups { get; set; }
        /*
        public IDbSet<Invite> Invites { get; set; }
        */

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(user => user.Id);
            modelBuilder.Entity<User>().Property(user => user.Email).IsRequired();
            modelBuilder.Entity<User>().Property(user => user.Name);
            modelBuilder.Entity<User>().Property(user => user.Surname);
            modelBuilder.Entity<User>().HasMany(user => user.Roles).WithMany(role => role.Users);
            modelBuilder.Entity<User>().HasMany(user => user.Posts).WithRequired(post => post.User).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().HasMany(user => user.Comments).WithRequired(comment => comment.User);
            modelBuilder.Entity<User>().HasMany(user => user.Groups).WithMany(group => group.Users);
            modelBuilder.Entity<User>().HasOptional(user => user.ActiveGroup);

            modelBuilder.Entity<Role>().HasKey(role => role.Id);
            modelBuilder.Entity<Role>().Property(role => role.Name).IsRequired();
            modelBuilder.Entity<Role>().Property(role => role.ApproveUsers).IsRequired();
            modelBuilder.Entity<Role>().Property(role => role.Post).IsRequired();
            modelBuilder.Entity<Role>().Property(role => role.Comment).IsRequired();
            modelBuilder.Entity<Role>().HasRequired(role => role.Group).WithMany(group => group.Roles);

            modelBuilder.Entity<Post>().HasKey(post => post.Id);
            modelBuilder.Entity<Post>().Property(post => post.CreatedOn).IsRequired();
            modelBuilder.Entity<Post>().Property(post => post.ModifiedOn);
            modelBuilder.Entity<Post>().Property(post => post.Title).IsRequired();
            modelBuilder.Entity<Post>().Property(post => post.Text).IsRequired();
            modelBuilder.Entity<Post>().HasMany(post => post.Comments).WithRequired(comment => comment.Post);

            modelBuilder.Entity<Comment>().HasKey(comment => comment.Id);
            modelBuilder.Entity<Comment>().Property(comment => comment.Text).IsRequired();
            modelBuilder.Entity<Comment>().Property(comment => comment.CreatedOn).IsRequired();
            modelBuilder.Entity<Comment>().Property(comment => comment.ModifiedOn);

            modelBuilder.Entity<Group>().HasKey(group => group.Id);
            modelBuilder.Entity<Group>().Property(group => group.Name);
            modelBuilder.Entity<Group>().HasMany(group => group.Posts).WithRequired(post => post.Group).WillCascadeOnDelete(false);
            /*
            modelBuilder.Entity<Invite>().HasKey(invite => invite.Id);
            modelBuilder.Entity<Invite>().Property(invite => invite.UserId).IsRequired();
            modelBuilder.Entity<Invite>().Property(invite => invite.GroupId).IsRequired();
            modelBuilder.Entity<Invite>().Property(invite => invite.CreatedOn).IsRequired();
            modelBuilder.Entity<Invite>().Property(invite => invite.Message).IsOptional();
            */
        }
    }
}