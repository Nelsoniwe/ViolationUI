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
        CustomInitializeComponent();

        InitializeComponent();
        emailEntry.TextChanged += OnEmailTextChanged;
        passwordEntry.TextChanged += OnPasswordTextChanged;
    }

    private async void CustomInitializeComponent()
    {
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

        var colorTask = _httpClient.GetAsync(
            MauiProgram.ApiEndpoint + "/api/VehicleColor");
        var markTask = _httpClient.GetAsync(
            MauiProgram.ApiEndpoint + "/api/VehicleMark");
        var typesTask = _httpClient.GetAsync(
            MauiProgram.ApiEndpoint + "/api/VehicleType");
        var violationsTask = _httpClient.GetAsync(
            MauiProgram.ApiEndpoint + "/api/Violation");
        var statusesTask = _httpClient.GetAsync(
            MauiProgram.ApiEndpoint + "/api/ApplicationStatus");

        var tasks = new List<Task<HttpResponseMessage>>() { colorTask, markTask, typesTask, violationsTask, statusesTask };

        Task.WaitAll(tasks.ToArray());

        MauiProgram.UserProfile = JsonConvert.DeserializeObject<UserProfileModel>(await userLoginResponse.Content.ReadAsStringAsync());
        MauiProgram.Colors = JsonConvert.DeserializeObject<List<VehicleColorModel>>(await colorTask.Result.Content.ReadAsStringAsync());
        MauiProgram.Marks = JsonConvert.DeserializeObject<List<VehicleMarkModel>>(await markTask.Result.Content.ReadAsStringAsync());
        MauiProgram.Types = JsonConvert.DeserializeObject<List<VehicleTypeModel>>(await typesTask.Result.Content.ReadAsStringAsync());
        MauiProgram.Violations = JsonConvert.DeserializeObject<List<ViolationModel>>(await violationsTask.Result.Content.ReadAsStringAsync());
        MauiProgram.Statuses = JsonConvert.DeserializeObject<List<ApplicationStatus>>(await statusesTask.Result.Content.ReadAsStringAsync());
    }

    private void OnBackButtonPressed(object sender, BackButtonPressedEventArgs e)
    {
        // Block the back button action
        e.Handled = true;
    }

    private static bool IgnoreCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true; // Allow any certificate
    }

    private void OnEmailTextChanged(object sender, TextChangedEventArgs e)
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

            if (userData.Roles.Contains("Admin"))
                await SecureStorage.Default.SetAsync("user_role", "Admin");
            else
                await SecureStorage.Default.SetAsync("user_role", "User");

            await Navigation.PushAsync(new ChoosePage());
        }
        else
        {
            await DisplayAlert("Помилка", "Перевірте корректність введених даних", "OK");
        }
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }
}

