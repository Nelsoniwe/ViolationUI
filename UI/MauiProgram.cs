using UI.Models;

namespace UI;

public static class MauiProgram
{
    public static string ApiEndpoint = "http://10.0.2.2:5000";
    public static List<VehicleColorModel> Colors;
    public static List<ApplicationStatus> Statuses;
    public static List<VehicleMarkModel> Marks;
    public static List<VehicleTypeModel> Types;
    public static List<ViolationModel> Violations;
    public static UserProfileModel UserProfile;

    public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			}).UseMauiMaps();

        return builder.Build();
	}


}
