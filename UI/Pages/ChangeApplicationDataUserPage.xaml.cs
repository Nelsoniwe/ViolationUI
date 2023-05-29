using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Pages;

public partial class ChangeApplicationDataUserPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private string _vehicleNumber;
    private RepresentativeApplication _selectedItem;

    public ChangeApplicationDataUserPage(RepresentativeApplication selectedItem)
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

        _selectedItem = selectedItem;
    }

    private void OnVehicleNumberEntryChanged(object sender, TextChangedEventArgs e)
    {
        _vehicleNumber = e.NewTextValue;
    }

    private async void UpdateButton_Clicked(object sender, EventArgs e)
    {
        ChangeApplication();
    }


    private async void ChangeApplication()
    {
        try
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
                StatusId = _selectedItem.StatusId,
                Geolocation = "string",
                PublicationTime = _selectedItem.PublicationTime,
                ViolationTime = ViolationDate.Date,
                PhotoId = _selectedItem.PhotoId,
                VideoId = _selectedItem.VideoId
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
                await DisplayAlert("Помилка", response.ReasonPhrase, "OK");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else
            {
                await DisplayAlert("Успіх", "", "OK");
                await Shell.Current.Navigation.PopAsync();
            }
        }
        catch (Exception exception)
        {
            await DisplayAlert("Помилка", exception.Message, "OK");
        }
    }

    private async void DownloadAttachedFile_Clicked(object sender, EventArgs e)
    {
        try
        {
            HttpRequestMessage request;
            if (_selectedItem.PhotoId != 1)
            {
                request = new HttpRequestMessage(HttpMethod.Get, MauiProgram.ApiEndpoint + "/api/Photo/ById/" + _selectedItem.PhotoId);
                string key = await SecureStorage.Default.GetAsync("api_token");

                request.Headers.Add("Authorization", "Bearer " + key);
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Get, MauiProgram.ApiEndpoint + "/api/Video/ById/" + _selectedItem.VideoId);
                string key = await SecureStorage.Default.GetAsync("api_token");

                request.Headers.Add("Authorization", "Bearer " + key);
            }

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            var file = JsonConvert.DeserializeObject<FileModel>(await response.Content.ReadAsStringAsync());

            var filePath = Path.Combine(FileSystem.CacheDirectory, file.FileName);
            await File.WriteAllBytesAsync(filePath, file.data);

            await Launcher.OpenAsync(new OpenFileRequest
            {
                File = new ReadOnlyFile(filePath)
            });
        }
        catch (Exception exception)
        {
            await DisplayAlert("Помилка", exception.Message, "OK");
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