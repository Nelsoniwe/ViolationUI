using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using UI.Models;

namespace UI.Pages;

public partial class CreateApplicationPage : ContentPage
{
    private readonly HttpClient _httpClient;
    private string _vehicleNumber;
    private string _fullPath;
    private byte[] _fileBytes;
    private string _fileName;


    private bool isVehicleTypeChanged = false;
    private bool isVehicleMarkChanged = false;
    private bool isVehicleColorChanged = false;
    private bool isViolationChanged = false;
    private bool isVehicleNumberChanged = false;
    private bool isViolationDateChanged = false;
    private bool isCommentChanged = false;
    private bool isfileAttached = false;

    public CreateApplicationPage()
    {
        InitializeComponent();

        _httpClient = new HttpClient();

        CustomInitializeComponent();

        VehicleNumberEntry.TextChanged += OnVehicleNumberEntryChanged;
        VehicleTypePicker.SelectedIndexChanged += VehicleTypeIndexChanged;
        VehicleMarkPicker.SelectedIndexChanged += VehicleMarkIndexChanged;
        VehicleColorPicker.SelectedIndexChanged += VehicleColorIndexChanged;
        ViolationPicker.SelectedIndexChanged += ViolationIndexChanged;
        commentEntry.TextChanged += OnCommentNumberEntryChanged;
        ViolationDate.DateSelected += Field_DateSelected;
        SendButton.IsEnabled = false;
    }
    private void VehicleTypeIndexChanged(object sender, EventArgs e)
    {
        isVehicleTypeChanged = true;
        UpdateSendButtonState();
    }
    private void VehicleMarkIndexChanged(object sender, EventArgs e)
    {
        isVehicleMarkChanged = true;
        UpdateSendButtonState();
    }
    private void VehicleColorIndexChanged(object sender, EventArgs e)
    {
        isVehicleColorChanged = true;
        UpdateSendButtonState();
    }
    private void ViolationIndexChanged(object sender, EventArgs e)
    {
        isViolationChanged = true;
        UpdateSendButtonState();
    }
    private void Field_DateSelected(object sender, DateChangedEventArgs e)
    {
        ViolationDate.TextColor = Colors.Black;
        isViolationDateChanged = true;
        UpdateSendButtonState();
    }

    private void UpdateSendButtonState()
    {
        SendButton.IsEnabled = isVehicleTypeChanged && isVehicleMarkChanged && isVehicleColorChanged &&
                               isViolationChanged && isVehicleNumberChanged && isViolationDateChanged && isCommentChanged && isfileAttached;
    }

    private void OnVehicleNumberEntryChanged(object sender, TextChangedEventArgs e)
    {
        _vehicleNumber = e.NewTextValue;
        isVehicleNumberChanged = true;
    }

    private void OnCommentNumberEntryChanged(object sender, TextChangedEventArgs e)
    {
        isCommentChanged = true;
    }

    private async void ChooseFile_Clicked(object sender, EventArgs e)
    {
        try
        {
            var fileResult = await FilePicker.PickAsync();

            if (fileResult != null)
            {
                if (fileResult.FileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase) ||
                    fileResult.FileName.EndsWith("png", StringComparison.OrdinalIgnoreCase) ||
                    fileResult.FileName.EndsWith("mp4", StringComparison.OrdinalIgnoreCase) ||
                    fileResult.FileName.EndsWith("avi", StringComparison.OrdinalIgnoreCase) ||
                    fileResult.FileName.EndsWith("mov", StringComparison.OrdinalIgnoreCase))
                {
                    await using var stream = await fileResult.OpenReadAsync();

                    using MemoryStream memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    _fileBytes = memoryStream.ToArray();
                    _fileName = fileResult.FileName;
                }

                isfileAttached = true;
                UpdateSendButtonState();
            }
        }
        catch (Exception exception)
        {
            await DisplayAlert("Помилка", exception.Message, "OK");
        }
    }

    private async void SendButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            ApplicationModel application = new ApplicationModel()
            {
                UserId = MauiProgram.UserProfile.Id,
                VehicleMarkId = MauiProgram.Marks.FirstOrDefault(x => x.Type == VehicleMarkPicker.SelectedItem).Id,
                ViolationId = MauiProgram.Violations.FirstOrDefault(x => x.Type == ViolationPicker.SelectedItem).Id,
                VehicleTypeId = MauiProgram.Types.FirstOrDefault(x => x.Type == VehicleTypePicker.SelectedItem).Id,
                VehicleColorId = MauiProgram.Colors.FirstOrDefault(x => x.Type == VehicleColorPicker.SelectedItem).Id,
                VehicleNumber = _vehicleNumber,
                StatusId = 1,
                Geolocation = "string",
                PublicationTime = DateTime.Now,
                ViolationTime = ViolationDate.Date,
                PhotoId = 1,
                VideoId = 1,
                UserComment = commentEntry.Text
            };

            using var formData = new MultipartFormDataContent();
            bool isPhoto = false;

            if (_fileName.EndsWith("jpg", StringComparison.OrdinalIgnoreCase))
            {
                var photoContent = new ByteArrayContent(_fileBytes);
                photoContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                formData.Add(photoContent, "photo", _fileName);
                isPhoto = true;
            }
            if (_fileName.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase))
            {
                var photoContent = new ByteArrayContent(_fileBytes);
                photoContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                formData.Add(photoContent, "photo", _fileName);
                isPhoto = true;
            }
            else if (_fileName.EndsWith("png", StringComparison.OrdinalIgnoreCase))
            {
                var photoContent = new ByteArrayContent(_fileBytes);
                photoContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                formData.Add(photoContent, "photo", _fileName);
                isPhoto = true;
            }
            else if (_fileName.EndsWith("mp4", StringComparison.OrdinalIgnoreCase))
            {
                var photoContent = new ByteArrayContent(_fileBytes);
                photoContent.Headers.ContentType = new MediaTypeHeaderValue("video/mp4");
                formData.Add(photoContent, "video", _fileName);
            }
            else if (_fileName.EndsWith("avi", StringComparison.OrdinalIgnoreCase))
            {
                var photoContent = new ByteArrayContent(_fileBytes);
                photoContent.Headers.ContentType = new MediaTypeHeaderValue("video/avi");
                formData.Add(photoContent, "video", _fileName);
            }
            else if (_fileName.EndsWith("mov", StringComparison.OrdinalIgnoreCase))
            {
                var photoContent = new ByteArrayContent(_fileBytes);
                photoContent.Headers.ContentType = new MediaTypeHeaderValue("video/mov");
                formData.Add(photoContent, "video", _fileName);
            }
            string key = await SecureStorage.Default.GetAsync("api_token");
            HttpResponseMessage fileResponse;

            if (isPhoto)
            {
                HttpRequestMessage fileRequest = new HttpRequestMessage(HttpMethod.Post, MauiProgram.ApiEndpoint + "/api/Photo");
                fileRequest.Content = formData;
                fileRequest.Headers.Add("Authorization", "Bearer " + key);
                fileResponse = await _httpClient.SendAsync(fileRequest);
            }
            else
            {
                HttpRequestMessage fileRequest = new HttpRequestMessage(HttpMethod.Post, MauiProgram.ApiEndpoint + "/api/Video");
                fileRequest.Content = formData;
                fileRequest.Headers.Add("Authorization", "Bearer " + key);
                fileResponse = await _httpClient.SendAsync(fileRequest);
            }

            string fileId = "";
            if (fileResponse.IsSuccessStatusCode)
            {
                fileId = JsonConvert.DeserializeObject<string>(await fileResponse.Content.ReadAsStringAsync());
            }
            else
            {
                SecureStorage.Default.RemoveAll();
                await DisplayAlert("Помилка", await fileResponse.Content.ReadAsStringAsync(), "OK");
                await Shell.Current.Navigation.PopToRootAsync();
            }

            if (isPhoto)
                application.PhotoId = Convert.ToInt32(fileId);
            else
                application.VideoId = Convert.ToInt32(fileId);

            string jsonData = System.Text.Json.JsonSerializer.Serialize(application);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, MauiProgram.ApiEndpoint + "/api/Application");
            request.Content = content;

            request.Headers.Add("Authorization", "Bearer " + key);

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                SecureStorage.Default.RemoveAll();
                await DisplayAlert("Помилка", await response.Content.ReadAsStringAsync(), "OK");
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

    private async Task CustomInitializeComponent()
    {
        try
        {
            VehicleTypePicker.ItemsSource = MauiProgram.Types.Select(x => x.Type).ToList();
            VehicleColorPicker.ItemsSource = MauiProgram.Colors.Select(x => x.Type).ToList();
            VehicleMarkPicker.ItemsSource = MauiProgram.Marks.Select(x => x.Type).ToList();
            ViolationPicker.ItemsSource = MauiProgram.Violations.Select(x => x.Type).ToList();
        }
        catch (Exception exception)
        {
            await DisplayAlert("Помилка", exception.Message, "OK");
        }
    }
}