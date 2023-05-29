namespace UI.Models;

public class ApplicationModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int VehicleMarkId { get; set; }
    public int ViolationId { get; set; }
    public int VehicleTypeId { get; set; }
    public int VehicleColorId { get; set; }
    public string VehicleNumber { get; set; }
    public int StatusId { get; set; }
    public string Geolocation { get; set; }
    public DateTime PublicationTime { get; set; }
    public DateTime ViolationTime { get; set; }
    public int? PhotoId { get; set; }
    public int? VideoId { get; set; }
    public string AdminComment { get; set; } = "";
    public string UserComment { get; set; } = "";
}