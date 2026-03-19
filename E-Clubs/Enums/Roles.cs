namespace E_Clubs.Enums;

[Flags]
public enum Roles
{
    Default = 0,
    Student = 1 << 0,
    Professor = 1 << 1,
    Director = 1 << 2,
    Admin = 1 << 3,
}