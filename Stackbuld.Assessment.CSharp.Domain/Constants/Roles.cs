namespace Stackbuld.Assessment.CSharp.Domain.Constants;

public class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
    public const string Merchant = "Merchant";

    public static readonly List<string> List = [Admin, User, Merchant];
}