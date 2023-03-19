namespace PikaServer.Domain.Entities;

public class Transaction : RootEntityBase
{
    public int Id { get; set; }
    public string FromAccountNo { get; set; }
    public string ToAccountNo { get; set; }
    public string Amount { get; set; }
    public string Description { get; set; }
}