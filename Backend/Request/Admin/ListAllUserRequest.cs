namespace TravelWithCode.Request;

public class ListAllUserRequest
{
    public required string Username {get;set;}
    public required bool Admin {get;set;}
    public required int Id {get;set;}
}