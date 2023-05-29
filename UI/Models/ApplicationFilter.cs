namespace UI.Models;

public class ApplicationFilter
{
    public int? VehicleMarkId { get; set; }
    public int? ViolationId { get; set; }
    public int? VehicleTypeId { get; set; }
    public int? VehicleColorId { get; set; }
    public string VehicleNumber { get; set; }
    public int? StatusId { get; set; }
    public DateTime? PublicationTime { get; set; }
    public DateTime? ViolationTime { get; set; }

    public string GetFilterString()
    {
        var filters = new List<string>();

        if (VehicleMarkId != null)
        {
            filters.Add($"VehicleMarkId={VehicleMarkId}");
        }

        if (ViolationId != null)
        {
            filters.Add($"ViolationId={ViolationId}");
        }

        if (VehicleTypeId != null)
        {
            filters.Add($"VehicleTypeId={VehicleTypeId}");
        }

        if (VehicleColorId != null)
        {
            filters.Add($"VehicleColorId={VehicleColorId}");
        }

        if (!string.IsNullOrEmpty(VehicleNumber))
        {
            filters.Add($"VehicleNumber={VehicleNumber}");
        }

        if (StatusId != null)
        {
            filters.Add($"StatusId={StatusId}");
        }

        if (PublicationTime != DateTime.MinValue)
        {
            filters.Add($"PublicationTime={PublicationTime}");
        }

        if (ViolationTime != DateTime.MinValue)
        {
            filters.Add($"ViolationTime={ViolationTime}");
        }

        return string.Join("&", filters);
    }
}