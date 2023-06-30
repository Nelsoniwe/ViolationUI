using Newtonsoft.Json;
using System.ComponentModel;
using UI.Models;

namespace UI.Pages;

public partial class ChoosePage : ContentPage
{
    private readonly HttpClient _httpClient;
    public ChoosePage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        NavigationPage.SetHasBackButton(this, false);
        CustomInitializeComponent();
    }

    private async Task CustomInitializeComponent()
    {
        try
        {
            string role = await SecureStorage.Default.GetAsync("user_role");
            if (role == "Admin")
            {
                ResolveApplications.IsVisible = true;
                SearchApplications.IsVisible = true;
            }
            else
            {
                ResolveApplications.IsVisible = false;
                SearchApplications.IsVisible = false;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, MauiProgram.ApiEndpoint + "/api/User/Profile");
            string key = await SecureStorage.Default.GetAsync("api_token");

            request.Headers.Add("Authorization", "Bearer " + key);
            var userLoginResponse = await _httpClient.SendAsync(request);

            if (key != null && !userLoginResponse.IsSuccessStatusCode)
            {
                SecureStorage.Default.RemoveAll();
                await DisplayAlert("Помилка", await userLoginResponse.Content.ReadAsStringAsync(), "OK");
                await Shell.Current.Navigation.PopToRootAsync();
            }

            MauiProgram.UserProfile = JsonConvert.DeserializeObject<UserProfileModel>(await userLoginResponse.Content.ReadAsStringAsync());
        }
        catch (Exception exception)
        {
            await DisplayAlert("Помилка", exception.Message, "OK");
        }
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private async void ProfileButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage());
    }

    private async void CreateApplicationButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreateApplicationPage());
    }

    private async void SearchApplicationButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ApplicationSearchFilterPage());
    }

    private async void ShowOwnApplicationButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ShowOwnApplicationPage());
    }

    private async void ResolveApplicationButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ResolveApplicationPage());
    }

    private async void LogoutButton_Clicked(object sender, EventArgs e)
    {
        SecureStorage.Default.RemoveAll();
        await Shell.Current.Navigation.PopToRootAsync();
    }
}