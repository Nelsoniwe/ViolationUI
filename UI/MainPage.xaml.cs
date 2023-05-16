using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UI.Pages;
using UI.Models;
using Microsoft.Maui.Storage;
using System.Text;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UI;

public partial class MainPage : ContentPage
{
    private string _email;
    private string _password;

    private readonly HttpClient _httpClient;

    public MainPage()
	{
        _httpClient = new HttpClient();
        ServicePointManager.ServerCertificateValidationCallback = IgnoreCertificateValidation;

        CheckCredentials();

        InitializeComponent();
        emailEntry.TextChanged += OnUsernameTextChanged;
        passwordEntry.TextChanged += OnPasswordTextChanged;
    }

    private static bool IgnoreCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true; // Allow any certificate
    }

    private void OnUsernameTextChanged(object sender, TextChangedEventArgs e)
    {
        _email = e.NewTextValue;
    }

    private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        _password = e.NewTextValue;
    }

    private async void CheckCredentials()
    {
        var token = await SecureStorage.Default.GetAsync("api_token");
        if (!String.IsNullOrEmpty(token))
        {
            await Navigation.PushAsync(new ChoosePage());
        }
    }

    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        var data = new UserModelCredentials() { Email = _email, Password = _password};
        string jsonData = JsonSerializer.Serialize(data);
        HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(MauiProgram.ApiEndpoint + "/login", content);

        if (response.IsSuccessStatusCode)
        {
            SecureStorage.Default.RemoveAll();
            var responsasdAsStringAsynce = await response.Content.ReadAsStringAsync();
            var userData = JsonConvert.DeserializeObject<LoginModel>(responsasdAsStringAsynce);

            await SecureStorage.Default.SetAsync("api_token", userData.UserToken);
            await SecureStorage.Default.SetAsync("user_id", userData.UserId.ToString());

            await Navigation.PushAsync(new ChoosePage());
        }
        else
        {
            await DisplayAlert("Error", "Wrong credentials", "OK");
        }
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}

