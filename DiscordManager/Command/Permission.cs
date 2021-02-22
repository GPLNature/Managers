using System.Collections.Generic;
using System.Linq;

namespace DiscordManager.Command
{
  public class Permission
  {
    // 
    private readonly Dictionary<string, LinkedList<string>> _permissions;

    internal Permission()
    {
      _permissions = new Dictionary<string, LinkedList<string>>
      {
        ["Owner"] = new(new[] {"Owner", "Admin", "Everyone"}.ToList()),
        ["Admin"] = new(new[] {"Admin", "Everyone"}),
        ["Everyone"] = new(new[] {"Everyone"})
      };
    }

    /// <summary>
    ///   Add Permission
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="perm"></param>
    /// <exception cref="ManagerException"> if perm is null or empty</exception>
    public void AddPermission(string roleName, string perm)
    {
      Checker.NotNull(roleName, nameof(roleName));
      Checker.NotNull(perm, nameof(perm));

      if (perm.Length == 0)
        throw new ManagerException("Empty Permission Name");
      if (!_permissions.TryGetValue(roleName, out var perms))
        throw new ManagerArgumentException(roleName, nameof(roleName));
      perms.AddLast(perm);
    }

    /// <summary>
    ///   Add Permissions
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="perms"></param>
    /// <exception cref="ManagerException"> If perms is empty or null</exception>
    public void AddPermissions(string roleName, IEnumerable<string> perms)
    {
      Checker.NotNull(roleName, nameof(roleName));
      Checker.CheckIsEmpty(perms, nameof(perms));

      if (!_permissions.TryGetValue(roleName, out var perm))
        throw new ManagerArgumentException(roleName, nameof(roleName));

      foreach (var permName in perms) perm.AddLast(permName);
    }

    /// <summary>
    ///   Remove Permission
    /// </summary>
    /// ///
    /// <param name="roleName"></param>
    /// <param name="perm"></param>
    /// <exception cref="ManagerException"> if perm is null </exception>
    public void RemovePermission(string roleName, string perm)
    {
      Checker.NotNull(roleName, nameof(roleName));
      Checker.NotNull(perm, nameof(perm));

      if (perm.Length == 0)
        throw new ManagerException("Empty Permission Name");
      if (roleName == "Everyone")
        throw new ManagerException("Cannot Remove Default Permission");

      if (!_permissions.TryGetValue(roleName, out var perms))
        throw new ManagerArgumentException(roleName, nameof(roleName));

      perms.Remove(perm);
    }

    internal LinkedList<string> GetPermission(string roleName)
    {
      if (!_permissions.TryGetValue(roleName, out var value))
        throw new ManagerArgumentException(roleName, nameof(roleName));

      return value;
    }

    internal LinkedList<string> GetDefaultPermission()
    {
      return GetPermission("Everyone");
    }
  }
}