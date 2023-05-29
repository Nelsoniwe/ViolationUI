using Newtonsoft.Json;
using System.Text;
using UI.Models;

namespace UI.Pages;

public partial class ProfilePage : ContentPage
{
    private UserProfileModel userData;
    private HttpClient _httpClient;
    public ProfilePage()
    {
        InitializeComponent();

        _httpClient = new HttpClient();
        CustomInitializeComponent();
    }

    private async void UpdateButton_Clicked(object sender, EventArgs e)
    {
        ChangeUserData();
    }

    private async void DeleteButton_Clicked(object sender, EventArgs e)
    {
        bool userConfirmation = await DisplayAlert("Підтвердження", "Ви впевнені що хочете видалити власний профіль?", "Так", "Ні");
        if (userConfirmation)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, MauiProgram.ApiEndpoint + "/User/Delete");
            string key = await SecureStorage.Default.GetAsync("api_token");
            request.Headers.Add("Authorization", "Bearer " + key);
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                SecureStorage.Default.RemoveAll();
                await DisplayAlert("Успіх", "", "OK");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else
            {
                await DisplayAlert("Помилка", response.ReasonPhrase, "OK");
            }
        }
    }

    private async void ChangeUserData()
    {
        try
        {
            var newUserData = new UserProfileModel()
            {
                Email = EmailEntry.Text,
                FirstName = FirstNameEntry.Text,
                LastName = SecondNameEntry.Text,
                UserName = UserNameEntry.Text,
                Id = MauiProgram.UserProfile.Id,
            };

            string jsonData = System.Text.Json.JsonSerializer.Serialize(newUserData);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, MauiProgram.ApiEndpoint + "/api/User/UpdateUser");
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
                MauiProgram.UserProfile = newUserData;
                await Shell.Current.Navigation.PopAsync();
            }
        }
        catch (Exception exception)
        {
            await DisplayAlert("Помилка", exception.Message, "OK");
        }
    }

    private async void CustomInitializeComponent()
    {
        try
        {
            FirstNameEntry.Text = MauiProgram.UserProfile.FirstName;
            SecondNameEntry.Text = MauiProgram.UserProfile.LastName;
            UserNameEntry.Text = MauiProgram.UserProfile.UserName;
            EmailEntry.Text = MauiProgram.UserProfile.Email;

            var request = new HttpRequestMessage(HttpMethod.Get, MauiProgram.ApiEndpoint + "/api/User/Profile");
            string key = await SecureStorage.Default.GetAsync("api_token");

            request.Headers.Add("Authorization", "Bearer " + key);
            var userLoginResponse = await _httpClient.SendAsync(request);

            userData = JsonConvert.DeserializeObject<UserProfileModel>(await userLoginResponse.Content.ReadAsStringAsync());
        }
        catch (Exception exception)
        {
            await DisplayAlert("Помилка", exception.Message, "OK");
        }
    }
}