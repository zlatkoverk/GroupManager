﻿using System;
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
    }
}