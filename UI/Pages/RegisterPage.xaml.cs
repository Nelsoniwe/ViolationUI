using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using UI.Models;

namespace UI.Pages;

public partial class RegisterPage : ContentPage
{
    private string _email;
    private string _password;
    private string _username;
    private string _firstname;
    private string _lastname;
    private readonly HttpClient _httpClient;

    public RegisterPage()
	{
		InitializeComponent();
        _httpClient = new HttpClient();

        emailEntry.TextChanged += OnEmailTextChanged;
        usernameEntry.TextChanged += OnUsernameTextChanged;
        passwordEntry.TextChanged += OnPasswordTextChanged;
        firstnameEntry.TextChanged += OnFirstnameTextChanged;
        lastnameEntry.TextChanged += OnLastnameTextChanged;
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        var data = new RegisterModel() { Email = _email, Password = _password, UserName = _username, FirstName = _firstname, LastName = _lastname};
        string jsonData = JsonSerializer.Serialize(data);
        HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(MauiProgram.ApiEndpoint + "/register", content);

        if (response.IsSuccessStatusCode)
        {
            SecureStorage.Default.RemoveAll();
            await DisplayAlert("Success", "Now you should login", "OK");
            await Shell.Current.Navigation.PopToRootAsync();
        }
        else
        {
            await DisplayAlert("Error", response.ReasonPhrase, "OK");
        }
    }

    private void OnEmailTextChanged(object sender, TextChangedEventArgs e)
    {
        _email = e.NewTextValue;
    }

    private void OnFirstnameTextChanged(object sender, TextChangedEventArgs e)
    {
        _firstname = e.NewTextValue;
    }

    private void OnLastnameTextChanged(object sender, TextChangedEventArgs e)
    {
        _lastname = e.NewTextValue;
    }

    private void OnUsernameTextChanged(object sender, TextChangedEventArgs e)
    {
        _username = e.NewTextValue;
    }

    private void OnPasswordTextChanged(object sender, TextChangedEventArgs e)
    {
        _password = e.NewTextValue;
    }
}