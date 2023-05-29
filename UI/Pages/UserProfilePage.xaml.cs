using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UI.Models;

namespace UI.Pages;

public partial class UserProfilePage : ContentPage
{
    private readonly HttpClient _httpClient;
    private int _userId;
    public UserProfilePage(int id)
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        _userId = id;
        CustomInitializeComponent();
    }

    private async void CustomInitializeComponent()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, MauiProgram.ApiEndpoint + "/api/User/Profile/"+ _userId);
        var userResponse = await _httpClient.SendAsync(request);

        if (!userResponse.IsSuccessStatusCode)
        {
            SecureStorage.Default.RemoveAll();
            await DisplayAlert("Помилка", userResponse.ReasonPhrase, "OK");
            await Shell.Current.Navigation.PopToRootAsync();
        }

        var userProfile = JsonConvert.DeserializeObject<UserProfileModel>(await userResponse.Content.ReadAsStringAsync());

        FirstNameLabel.Text = userProfile.FirstName;
        SecondNameLabel.Text = userProfile.LastName;
        UserNameLabel.Text = userProfile.UserName;
        EmailLabel.Text = userProfile.Email;
    }

}