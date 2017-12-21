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
    }
}