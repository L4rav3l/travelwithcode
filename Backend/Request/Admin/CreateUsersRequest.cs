namespace TravelWithCode.Request;

public class CreateUsersRequest
{
    public required string Username {get;set;}
    public required string Password {get;set;}
    public required bool Admin {get;set;}
}