namespace DryCleaning.Domain;

public class Client
{
    public int ClientId { get; set; }
    public required string FullName { get; set; }
    public required string PhoneNumber { get; set; }
}
