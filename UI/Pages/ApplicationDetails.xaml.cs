using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Pages;

public partial class ApplicationDetails : ContentPage
{
    private int? _photoId;
    private int? _videoId;
    private readonly HttpClient _httpClient;

    private RepresentativeApplication _selectedItem;
    public ApplicationDetails(RepresentativeApplication selectedItem)
    {
        InitializeComponent();
        _httpClient = new HttpClient();

        _selectedItem = selectedItem;

        VehicleMarkLabel.Text = selectedItem.VehicleMark;
        ViolationLabel.Text = selectedItem.Violation;
        VehicleTypeLabel.Text = selectedItem.VehicleType;
        VehicleColorLabel.Text = selectedItem.VehicleColor;
        VehicleNumberLabel.Text = selectedItem.VehicleNumber;
        StatusLabel.Text = selectedItem.Status;
        PublicationDateLabel.Text = selectedItem.PublicationTime.ToString();
        ViolationTimeLabel.Text = selectedItem.ViolationTime.ToString();
        CommentLabel.Text = selectedItem.UserComment;
        _photoId = selectedItem.PhotoId;
        _videoId = selectedItem.VideoId;
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
}