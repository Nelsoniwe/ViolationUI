using Newtonsoft.Json;
using System.Text;
using UI.Models;

namespace UI.Pages;

public partial class ShowOwnApplicationPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private List<ApplicationModel> _applications;
    private UserProfileModel _userProfile;

    public ShowOwnApplicationPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();

        CustomInitializeComponent();
    }

    private async void CustomInitializeComponent()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, MauiProgram.ApiEndpoint + "/api/User/GetProfile");
        string key = await SecureStorage.Default.GetAsync("api_token");

        request.Headers.Add("Authorization", "Bearer " + key);
        var userLoginResponse = await _httpClient.SendAsync(request);

        if (!userLoginResponse.IsSuccessStatusCode)
        {
            SecureStorage.Default.RemoveAll();
            await DisplayAlert("Error", userLoginResponse.ReasonPhrase, "OK");
            await Shell.Current.Navigation.PopToRootAsync();
        }
        _userProfile = JsonConvert.DeserializeObject<UserProfileModel>(await userLoginResponse.Content.ReadAsStringAsync());


        HttpRequestMessage applicationsRequest = new HttpRequestMessage(HttpMethod.Get, MauiProgram.ApiEndpoint + "/api/Application/GetByUserId?id=" + _userProfile.Id);
        applicationsRequest.Headers.Add("Authorization", "Bearer " + key);

        HttpResponseMessage response = await _httpClient.SendAsync(applicationsRequest);
        _applications = JsonConvert.DeserializeObject<List<ApplicationModel>>(await response.Content.ReadAsStringAsync());
        ApplicationsView.ItemsSource = _applications.Select(x => new RepresentativeApplication()
        {
            Id = x.Id,
            VehicleMarkId = x.VehicleMarkId,
            VehicleMark = MauiProgram.Marks.First(y => y.Id == x.VehicleMarkId).Type,
            ViolationId = x.ViolationId,
            Violation = MauiProgram.Violations.First(y => y.Id == x.ViolationId).Type,
            VehicleTypeId = x.VehicleTypeId,
            VehicleType = MauiProgram.Types.First(y => y.Id == x.VehicleTypeId).Type,
            VehicleColorId = x.VehicleColorId,
            VehicleColor = MauiProgram.Colors.First(y => y.Id == x.VehicleColorId).Type,
            VehicleNumber = x.VehicleNumber,
            StatusId = x.StatusId,
            Status = MauiProgram.Statuses.First(y => y.Id == x.StatusId).Status,
            Geolocation = x.Geolocation,
            PublicationTime = x.PublicationTime,
            ViolationTime = x.ViolationTime,
            UserId = x.UserId,
            PhotoId = x.PhotoId,
            VideoId = x.VideoId
        });
    }
}