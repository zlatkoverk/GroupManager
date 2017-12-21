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
            User user = _context.Users.Include(u=>u.Groups).Include(u=>u.Roles).FirstOrDefault(u => u.Id == userId);

            _context.Groups.Add(group);
            user?.Groups.Add(group);

            Role creator = new Role("Admin", group) { ApproveUsers = true, Comment = true, Post = true };
            _context.Roles.Add(creator);
            creator.Users.Add(user);
            user?.Roles.Add(creator);
            group.Roles.Add(creator);
            
            _context.SaveChanges();
        }

        public void AddRole(Role role, Guid groupId)
        {
            _context.Roles.Add(role);
            _context.Groups.FirstOrDefault(g=>g.Id==groupId)?.Roles.Add(role);
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
    }
}