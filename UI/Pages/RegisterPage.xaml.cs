using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using UI.Models;
using Microsoft.Maui.ApplicationModel.Communication;

namespace UI.Pages;

public partial class RegisterPage : ContentPage
{
    private string _email;
    private string _password;
    private string _username;
    private string _firstname;
    private string _lastname;
    private string _repeatPassword;
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
        repeatPasswordEntry.TextChanged += OnRepeatPasswordTextChanged;
    }

    private async void RegisterButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            Regex regex = new Regex(MauiProgram.EmailPattern);

            if (    String.IsNullOrEmpty(_email)||
            String.IsNullOrEmpty(_password) ||
            String.IsNullOrEmpty(_username) ||
            String.IsNullOrEmpty(_firstname) ||
            String.IsNullOrEmpty(_lastname) ||
            String.IsNullOrEmpty(_repeatPassword))
            {
                await DisplayAlert("Помилка", "Не всі поля заповнені", "OK");
                return;
            }

            if (!regex.IsMatch(_email))
            {
                await DisplayAlert("Помилка", "Некоректний формат електронної пошти", "OK");
                return;
            }

            if (_password != _repeatPassword)
            {
                await DisplayAlert("Помилка", "Паролі не співпадають", "OK");
                return;
            }

            var data = new RegisterModel() { Email = _email, Password = _password, UserName = _username, FirstName = _firstname, LastName = _lastname };
            string jsonData = JsonSerializer.Serialize(data);
            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(MauiProgram.ApiEndpoint + "/register", content);

            if (response.IsSuccessStatusCode)
            {
                SecureStorage.Default.RemoveAll();
                await DisplayAlert("Успіх", "Тепер ви маєте авторизуватись", "OK");
                await Shell.Current.Navigation.PopToRootAsync();
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();

                if (errorMessage.Contains("password"))
                {
                    await DisplayAlert("Помилка", "Неправильний пароль. Повинен містити принаймні 6 символів, одну велику літеру, одну малу літеру та спеціальний символ.", "OK");
                    return;
                }
                else
                {
                    await DisplayAlert("Помилка", await response.Content.ReadAsStringAsync(), "OK");
                }
            }
        }
        catch (Exception exception)
        {
            await DisplayAlert("Помилка", exception.Message, "OK");
        }
    }


    private void OnEmailTextChanged(object sender, TextChangedEventArgs e)
    {
        _email = e.NewTextValue;

        var frame = sender as Entry;
        Regex regex = new Regex(MauiProgram.EmailPattern);

        if (!regex.IsMatch(_email))
        {
            frame.Background = Brush.Bisque;
        }
        else
        {
            frame.Background = Brush.Default;
        }
    }
    

    private void OnFirstnameTextChanged(object sender, TextChangedEventArgs e) => _firstname = e.NewTextValue;

    private void OnLastnameTextChanged(object sender, TextChangedEventArgs e) => _lastname = e.NewTextValue;

    private void OnUsernameTextChanged(object sender, TextChangedEventArgs e) => _username = e.NewTextValue;

    private void OnPasswordTextChanged(object sender, TextChangedEventArgs e) => _password = e.NewTextValue;

    private void OnRepeatPasswordTextChanged(object sender, TextChangedEventArgs e) => _repeatPassword = e.NewTextValue;
}