namespace TravelWithCode.Request;

public class EditUserRequest
{
    public required int Id {get;set;}
    public required string Username {get;set;}
    public required string Password {get;set;}
    public required bool Admin {get;set;}
}