using System.Text;

namespace E_Clubs.Data;

public static class AppRoles
{
    public const string Admin = "Admin";
    public const string Director = "Director";
    public const string Teacher = "Teacher";
    public const string Student = "Student";

    public static bool Exists(string role) => role.Equals(Admin, StringComparison.OrdinalIgnoreCase) || 
                                              role.Equals(Director, StringComparison.OrdinalIgnoreCase) || 
                                              role.Equals(Teacher, StringComparison.OrdinalIgnoreCase) ||  
                                              role.Equals(Student, StringComparison.OrdinalIgnoreCase);

    public static string MergeRoles(params string[] roles)
    {
        if (roles.Length == 0)
            return Admin;
        
        var mergedRoles = new StringBuilder();

        foreach (var role in roles)
        {
            mergedRoles.Append(role).Append(',');
        }

        mergedRoles.Length -= 1;

        return mergedRoles.ToString();
    }
}