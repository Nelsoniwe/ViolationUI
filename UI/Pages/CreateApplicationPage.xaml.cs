using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using UI.Models;

namespace UI.Pages;

public partial class CreateApplicationPage : ContentPage
{
    private List<VehicleColorModel> _colors;
    private List<VehicleMarkModel> _marks;
    private List<VehicleTypeModel> _types;
    private List<ViolationModel> _violations;
    private readonly HttpClient _httpClient;
    private UserProfileModel _userProfile;
    private string _vehicleNumber;

    public CreateApplicationPage()
    {
        InitializeComponent();

        _httpClient = new HttpClient();

        CustomInitializeComponent();

        VehicleNumberEntry.TextChanged += OnVehicleNumberEntryChanged;
    }

    private void OnVehicleNumberEntryChanged(object sender, TextChangedEventArgs e)
    {
        _vehicleNumber = e.NewTextValue;
    }

    private async void SendButton_Clicked(object sender, EventArgs e)
    {
        ApplicationModel application = new ApplicationModel()
        {
            UserId = MauiProgram.UserProfile.Id,
            VehicleMarkId = MauiProgram.Marks.FirstOrDefault(x=>x.Type == VehicleMarkPicker.SelectedItem).Id,
            ViolationId = MauiProgram.Violations.FirstOrDefault(x => x.Type == ViolationPicker.SelectedItem).Id,
            VehicleTypeId = MauiProgram.Types.FirstOrDefault(x => x.Type == VehicleTypePicker.SelectedItem).Id,
            VehicleColorId = MauiProgram.Colors.FirstOrDefault(x => x.Type == VehicleColorPicker.SelectedItem).Id,
            VehicleNumber = _vehicleNumber,
            StatusId = 1,
            Geolocation = "string",
            PublicationTime = DateTime.Now,
            ViolationTime = ViolationDate.Date,
            PhotoId = 1,
            VideoId = 1
        };

        string jsonData = System.Text.Json.JsonSerializer.Serialize(application);
        HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, MauiProgram.ApiEndpoint + "/api/Application");
        request.Content = content;
        string key = await SecureStorage.Default.GetAsync("api_token");

        request.Headers.Add("Authorization", "Bearer " + key);

        HttpResponseMessage response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            SecureStorage.Default.RemoveAll();
            await DisplayAlert("Error", response.ReasonPhrase, "OK");
            await Shell.Current.Navigation.PopToRootAsync();
        }
        else
        {
            await DisplayAlert("Success", "", "OK");
            await Shell.Current.Navigation.PopAsync();
        }
    }

    private async void CustomInitializeComponent()
    {
        VehicleTypePicker.ItemsSource = MauiProgram.Types.Select(x=>x.Type).ToList();
        VehicleColorPicker.ItemsSource = MauiProgram.Colors.Select(x => x.Type).ToList();
        VehicleMarkPicker.ItemsSource = MauiProgram.Marks.Select(x => x.Type).ToList();
        ViolationPicker.ItemsSource = MauiProgram.Violations.Select(x => x.Type).ToList();
    }
}