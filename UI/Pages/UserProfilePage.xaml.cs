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

    private UserProfileModel selectedUser;
    public UserProfilePage(int id)
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        _userId = id;
        CustomInitializeComponent();
    }

    private async Task CustomInitializeComponent()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, MauiProgram.ApiEndpoint + "/api/User/Profile/"+ _userId);
        var userResponse = await _httpClient.SendAsync(request);

        if (!userResponse.IsSuccessStatusCode)
        {
            SecureStorage.Default.RemoveAll();
            await DisplayAlert("Помилка", await userResponse.Content.ReadAsStringAsync(), "OK");
            await Shell.Current.Navigation.PopToRootAsync();
        }

        selectedUser = JsonConvert.DeserializeObject<UserProfileModel>(await userResponse.Content.ReadAsStringAsync());

        FirstNameLabel.Text = selectedUser.FirstName;
        SecondNameLabel.Text = selectedUser.LastName;
        UserNameLabel.Text = selectedUser.UserName;
        EmailLabel.Text = selectedUser.Email;
    }

    private async void UserApplicationsButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var connectionString = MauiProgram.ApiEndpoint + "/api/Application/ByUserId/" + selectedUser.Id;
            HttpRequestMessage applicationsRequest = new HttpRequestMessage(HttpMethod.Get, connectionString);

            HttpResponseMessage response = await _httpClient.SendAsync(applicationsRequest);
            var _applications = JsonConvert.DeserializeObject<List<ApplicationModel>>(await response.Content.ReadAsStringAsync());

            string role = await SecureStorage.Default.GetAsync("user_role");
            bool isAdmin = false || role == "Admin";

            var result = _applications.Select(x => new RepresentativeApplication()
            {
                Id = x.Id,
                VehicleMarkId = x.VehicleMarkId,
                VehicleMark = MauiProgram.Marks.First(y => y.Id == x.VehicleMarkId).Type,
                ViolationId = x.ViolationId,
                Violation = MauiProgram.Violations.First(y => y.Id == x.ViolationId).Type,
                VehicleTypeId = x.VehicleTypeId,
                VehicleType = MauiProgram.Types.First(y => y.Id == x.VehicleTypeId).Type,
                VehicleColorId = x.VehicleColorId,
                VehicleColor = MauiProgram.Colors.First(y => y.Id == x.VehicleColorId).Type,
                VehicleNumber = x.VehicleNumber,
                StatusId = x.StatusId,
                Status = MauiProgram.Statuses.First(y => y.Id == x.StatusId).Status,
                Geolocation = x.Geolocation,
                PublicationTime = x.PublicationTime,
                ViolationTime = x.ViolationTime,
                UserId = x.UserId,
                PhotoId = x.PhotoId,
                VideoId = x.VideoId,
                UserCanChange = x.StatusId == 1 && isAdmin,
                UserComment = x.UserComment,
                AdminComment = x.AdminComment
            }).ToList();

            if (!result.Any())
                await DisplayAlert("Список заявок порожній", "", "OK");
            else
                await Navigation.PushAsync(new ApplicationListWithSearch(result.ToList()));
        }
        catch (Exception exception)
        {
            await DisplayAlert("Помилка", exception.Message, "OK");
        }
    }
}