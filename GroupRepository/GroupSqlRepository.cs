using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace GroupRepository
{
    public class GroupSqlRepository : IGroupRepository
    {
        private readonly GroupDbContext _context;

        public GroupSqlRepository(GroupDbContext context)
        {
            _context = context;
        }


        public List<Group> GetGroups(Guid userId)
        {
            return _context.Users.Include(u => u.Groups).FirstOrDefault(usr => usr.Id == userId)?.Groups;
        }

        public void AddGroup(Group group, Guid userId)
        {
            User user = _context.Users.Include(u => u.Groups).Include(u => u.Roles).FirstOrDefault(u => u.Id.Equals(userId));
            if (user == null)
            {
                throw new ArgumentException("User does not exist" + userId);
            }

            _context.Groups.Add(group);
            user.Groups.Add(group);

            Role creator = new Role("Admin", group) { ApproveUsers = true, Comment = true, Post = true };
            _context.Roles.Add(creator);
            creator.Users.Add(user);
            user.Roles.Add(creator);
            group.Roles.Add(creator);
            user.ActiveGroup = group;

            _context.SaveChanges();
        }

        public void AddRole(Role role, Guid groupId)
        {
            _context.Roles.Add(role);
            _context.Groups.FirstOrDefault(g => g.Id == groupId)?.Roles.Add(role);
            _context.SaveChanges();
        }

        public List<Role> GetRoles(Guid groupId)
        {
            return _context.Roles.Where(role => role.Group.Id == groupId).ToList();
        }

        public void AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetUser(Guid id)
        {
            return _context.Users.Include(u => u.ActiveGroup).Include(u => u.Roles).Include(u => u.Posts).FirstOrDefault(u => u.Id == id);
        }

        public void SetActive(Guid userId, Guid groupId)
        {
            User user = GetUser(userId);
            Group group = GetGroups(userId).FirstOrDefault(gr => gr.Id == groupId);

            user.ActiveGroup = group;
            _context.SaveChanges();
        }

        public List<Post> GetPosts(Guid groupId)
        {
            return _context.Posts.Include(post => post.Comments).Include(post => post.User)
                .Where(post => post.Group.Id == groupId).OrderByDescending(post => post.CreatedOn).ToList();
        }

        public Post GetPost(Guid postId)
        {
            return _context.Posts.Include(post => post.Comments).Include(post => post.Group).FirstOrDefault(post => post.Id == postId);
        }

        public List<Comment> GetComments(Guid postId)
        {
            return _context.Comments.Include(comment => comment.Post).Include(comment => comment.User).Where(comment => comment.Post.Id == postId).OrderBy(comment => comment.CreatedOn).ToList();
        }

        public void AddPost(Post post, Guid groupId, Guid userId)
        {
            User user = GetUser(userId);
            Group group = _context.Groups.Include(g => g.Posts).FirstOrDefault(g => g.Id == user.ActiveGroup.Id);
            post.User = user;
            post.Group = group;
            user.Posts.Add(post);
            group.Posts.Add(post);
            _context.Posts.Add(post);
            _context.SaveChanges();
        }

        public void UpdatePost(Post post)
        {
            Post oldPost = _context.Posts.FirstOrDefault(p => p.Id == post.Id);
            oldPost.Title = post.Title;
            oldPost.Text = post.Text;
            oldPost.ModifiedOn = DateTime.Now;
            _context.SaveChanges();
        }

        public void AddUserToGroup(Guid userId, Guid groupId)
        {
            User user = _context.Users.Include(u => u.Groups).FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new ArgumentException("User does not exist" + userId);
            }
            Group group = _context.Groups.Include(g => g.Users).FirstOrDefault(g => g.Id == groupId);
            if (group == null)
            {
                throw new ArgumentException("Group does not exist" + groupId);
            }
            user.Groups.Add(group);
            group.Users.Add(user);
            _context.SaveChanges();
        }

        public User GetUser(string email)
        {
            return _context.Users.FirstOrDefault(user => user.Email == email);
        }

        public void AddComment(Comment comment, Guid userId, Guid postId)
        {
            User user = _context.Users.Include(u => u.Comments).FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new ArgumentException("User does not exist" + userId);
            }
            Post post = _context.Posts.Include(p => p.Comments).FirstOrDefault(p => p.Id == postId);
            if (post == null)
            {
                throw new ArgumentException("Group does not exist" + postId);
            }
            user.Comments.Add(comment);
            comment.User = user;
            post.Comments.Add(comment);
            comment.Post = post;
            _context.SaveChanges();
        }

        public void SetNick(string value, Guid userId, Guid groupId)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                throw new ArgumentException("User does not exist" + userId);
            }
            Group group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
            if (group == null)
            {
                throw new ArgumentException("Group does not exist" + groupId);
            }
            Nick nick = _context.Nicks.FirstOrDefault(nck => nck.User.Id == userId && nck.Group.Id == groupId);
            if (nick == null)
            {
                nick = new Nick(value, user, group);
                _context.Nicks.Add(nick);
            }
            else
            {
                nick.Value = value;
            }
            _context.SaveChanges();
        }

        public string GetNick(Guid userId, Guid groupId)
        {
            Nick nick = _context.Nicks.FirstOrDefault(nck => nck.User.Id == userId && nck.Group.Id == groupId);
            if (nick == null)
            {
                return _context.Users.FirstOrDefault(user => user.Id == userId)?.Email;
            }
            return nick.Value;
        }
    }
}