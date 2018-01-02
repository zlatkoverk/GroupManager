using System;
using System.Collections.Generic;

namespace GroupRepository
{
    public interface IGroupRepository
    {
        /// <summary>
        /// Returns all groups in which is user.
        /// </summary>
        /// <param name="userId">ID of a user</param>
        /// <returns>All groups in which is user</returns>
        List<Group> GetGroups(Guid userId);
        /// <summary>
        /// Adds group in database and gives user all permissions.
        /// </summary>
        /// <param name="group">group to add</param>
        /// <param name="userId">id of group creator</param>
        void AddGroup(Group group, Guid userId);
        /// <summary>
        /// Add new role to database.
        /// </summary>
        /// <param name="role">a role to add</param>
        /// <param name="groupId">an id of the group</param>
        void AddRole(Role role, Guid groupId);
        /// <summary>
        /// Returns all roles in group.
        /// </summary>
        /// <param name="groupId">ID of a group</param>
        List<Role> GetRoles(Guid groupId);

        void AddUser(User user);

        User GetUser(Guid id);

        void SetActive(Guid userId, Guid groupId);

        List<Post> GetPosts(Guid groupId);

        Post GetPost(Guid postId);

        List<Comment> GetComments(Guid postId);

        void AddPost(Post post, Guid groupId, Guid userId);

        void UpdatePost(Post post);

        void AddUserToGroup(Guid userId, Guid groupId);

        User GetUser(string email);

        void AddComment(Comment comment, Guid userId, Guid postId);

        void SetNick(string value, Guid userId, Guid groupId);

        String GetNick(Guid userId, Guid groupId);

        Comment GetComment(Guid commentId);

        void UpdateComment(Comment comment);

        List<User> GetUsers(Guid groupId);

        void UpdateUser(User user);

        List<BalanceEntry> GetBalanceEntries(Guid groupId);

        void AddBalanceEntry(Guid userId, Guid groupId, double value, string message);

        void DeleteBalanceEntry(Guid id);

        List<Event> GetEvents(Guid groupId);

        void AddGroupEvent(string name, string text, DateTime time, Guid groupId);

        void RemoveEvent(Guid eventId);

        void UpdateEvent(Event e);

        void SetAttending(Guid userId, Guid eventId);
        void SetNotAttending(Guid userId, Guid eventId);
        void SetInvited(Guid userId, Guid eventId);
        void AddEventPost(Post post, Guid eventId, Guid userId);
        List<Post> GetEventPosts(Guid eventId);
    }
}