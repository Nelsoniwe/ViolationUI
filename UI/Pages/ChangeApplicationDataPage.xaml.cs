﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        VehicleNumberEntry.Background = String.IsNullOrEmpty(_vehicleNumber) ? Brush.Bisque : Brush.Default;

        UpdateSendButtonState();

    }
    private void UpdateSendButtonState()
    {
        ApproveButton.IsEnabled = !String.IsNullOrEmpty(VehicleNumberEntry.Text);
        RejectButton.IsEnabled = !String.IsNullOrEmpty(VehicleNumberEntry.Text);
        UpdateButton.IsEnabled = !String.IsNullOrEmpty(VehicleNumberEntry.Text);
    }


    private async void ApproveButton_Clicked(object sender, EventArgs e)
    {
        ChangeApplication(3);
    }

    private async void UpdateButton_Clicked(object sender, EventArgs e)
    {
        ChangeApplication(0);
    }

    private async void RejectButton_Clicked(object sender, EventArgs e)
    {
        ChangeApplication(2);
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

    private async void ChangeApplication(int status)
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
                StatusId = status == 0 ? _selectedItem.StatusId : status,
                Geolocation = "string",
                PublicationTime = PublicationDate.Date,
                ViolationTime = ViolationDate.Date,
                PhotoId = _selectedItem.PhotoId,
                VideoId = _selectedItem.VideoId,
                UserComment = _selectedItem.UserComment,
                AdminComment = AdminCommentEntry.Text ?? String.Empty
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
        VehicleTypePicker.ItemsSource = MauiProgram.Types.Select(x => x.Type).ToList();
        VehicleColorPicker.ItemsSource = MauiProgram.Colors.Select(x => x.Type).ToList();
        VehicleMarkPicker.ItemsSource = MauiProgram.Marks.Select(x => x.Type).ToList();
        ViolationPicker.ItemsSource = MauiProgram.Violations.Select(x => x.Type).ToList();
    }
}