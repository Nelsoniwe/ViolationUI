using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Pages;

public partial class ChangeApplicationDataPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private string _vehicleNumber;
    private RepresentativeApplication _selectedItem;

    public ChangeApplicationDataPage(RepresentativeApplication selectedItem)
    {
        InitializeComponent();

        _httpClient = new HttpClient();

        CustomInitializeComponent();

        VehicleNumberEntry.TextChanged += OnVehicleNumberEntryChanged;

        VehicleMarkPicker.SelectedItem = selectedItem.VehicleMark;
        ViolationPicker.SelectedItem = selectedItem.Violation;
        VehicleTypePicker.SelectedItem = selectedItem.VehicleType;
        VehicleColorPicker.SelectedItem = selectedItem.VehicleColor;
        VehicleNumberEntry.Text = selectedItem.VehicleNumber;
        ViolationDate.Date = selectedItem.ViolationTime;
        PublicationDate.Date = selectedItem.PublicationTime;

        _selectedItem = selectedItem;
    }

    private void OnVehicleNumberEntryChanged(object sender, TextChangedEventArgs e)
    {
        _vehicleNumber = e.NewTextValue;
    }

    private async void ApproveButton_Clicked(object sender, EventArgs e)
    {
        ChangeApplication("Approved");
    }

    private async void UpdateButton_Clicked(object sender, EventArgs e)
    {
        ChangeApplication(null);
    }

    private async void RejectButton_Clicked(object sender, EventArgs e)
    {
        ChangeApplication("Rejected");
    }

    private async void ChangeApplication(string status)
    {
        ApplicationModel application = new ApplicationModel()
        {
            Id = _selectedItem.Id,
            UserId = _selectedItem.UserId,
            VehicleMarkId = MauiProgram.Marks.FirstOrDefault(x => x.Type == VehicleMarkPicker.SelectedItem).Id,
            ViolationId = MauiProgram.Violations.FirstOrDefault(x => x.Type == ViolationPicker.SelectedItem).Id,
            VehicleTypeId = MauiProgram.Types.FirstOrDefault(x => x.Type == VehicleTypePicker.SelectedItem).Id,
            VehicleColorId = MauiProgram.Colors.FirstOrDefault(x => x.Type == VehicleColorPicker.SelectedItem).Id,
            VehicleNumber = _vehicleNumber,
            StatusId = String.IsNullOrEmpty(status) ? _selectedItem.StatusId : MauiProgram.Statuses.FirstOrDefault(x => x.Status == status).Id,
            Geolocation = "string",
            PublicationTime = PublicationDate.Date,
            ViolationTime = ViolationDate.Date,
            PhotoId = 1,
            VideoId = 1
        };

        string jsonData = System.Text.Json.JsonSerializer.Serialize(application);
        HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, MauiProgram.ApiEndpoint + "/api/Application");
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
        VehicleTypePicker.ItemsSource = MauiProgram.Types.Select(x => x.Type).ToList();
        VehicleColorPicker.ItemsSource = MauiProgram.Colors.Select(x => x.Type).ToList();
        VehicleMarkPicker.ItemsSource = MauiProgram.Marks.Select(x => x.Type).ToList();
        ViolationPicker.ItemsSource = MauiProgram.Violations.Select(x => x.Type).ToList();
    }
}