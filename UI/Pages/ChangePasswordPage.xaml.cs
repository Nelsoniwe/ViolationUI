using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Pages;

public partial class ChangePasswordPage : ContentPage
{
    private HttpClient _httpClient;
    public ChangePasswordPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
    }

    private async void UpdatePassword_Clicked(object sender, EventArgs e)
    {
        var changePasswordModel = new ChangePasswordModel();
        changePasswordModel.CurrentPassword = CurrentPasswordEntry.Text;
        changePasswordModel.NewPassword = NewPasswordEntry.Text;
        
        string jsonData = System.Text.Json.JsonSerializer.Serialize(changePasswordModel);
        HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");


        var request = new HttpRequestMessage(HttpMethod.Put, MauiProgram.ApiEndpoint + "/api/User/ChangePassword");
        request.Content = content;
        string key = await SecureStorage.Default.GetAsync("api_token");

        request.Headers.Add("Authorization", "Bearer " + key);
        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            await DisplayAlert("Помилка", "Ваш поточний пароль невірний або новий пароль недійсний. " +
                                    "Новий пароль має бути щонайменше 6 символів, містити 1 велику та 1 малу літери та спеціальні символи", "OK");
        }
        else
        {
            await DisplayAlert("Успіх", "", "OK");
            await Shell.Current.Navigation.PopAsync();
        }
    }
}