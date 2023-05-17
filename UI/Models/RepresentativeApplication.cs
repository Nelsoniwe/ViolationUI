﻿namespace UI.Models;

public class RepresentativeApplication
{
    public int Id { get; set; }
    public string VehicleMark { get; set; }
    public string Violation { get; set; }
    public string VehicleType { get; set; }
    public string VehicleColor { get; set; }
    public string VehicleNumber { get; set; }
    public string Status { get; set; }
    public string Geolocation { get; set; }
    public DateTime PublicationTime { get; set; }
    public DateTime ViolationTime { get; set; }
    public int UserId { get; set; }
    public int VehicleMarkId { get; set; }
    public int ViolationId { get; set; }
    public int VehicleTypeId { get; set; }
    public int VehicleColorId { get; set; }
    public int StatusId { get; set; }
    public int? PhotoId { get; set; }
    public int? VideoId { get; set; }
}