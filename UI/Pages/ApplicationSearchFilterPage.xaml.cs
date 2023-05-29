using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UI.Models;

namespace UI.Pages;

public partial class ApplicationSearchFilterPage : ContentPage
{

    private readonly HttpClient _httpClient;
    private DateTime _violationDate = DateTime.MinValue;
    private DateTime _publicationDate = DateTime.MinValue;

    public ApplicationSearchFilterPage()
    {
        InitializeComponent();

        _httpClient = new HttpClient();

        CustomInitializeComponent();

        PublicationDate.DateSelected += PublicationDatePicker_DateSelected;
        ViolationDate.DateSelected += ViolationDatePicker_DateSelected;
    }

    private void PublicationDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        _publicationDate = e.NewDate;
    }

    private void ViolationDatePicker_DateSelected(object sender, DateChangedEventArgs e)
    {
        _violationDate = e.NewDate;
    }

    private async void CustomInitializeComponent()
    {
        VehicleTypePicker.ItemsSource = MauiProgram.Types.Select(x => x.Type).ToList();
        VehicleColorPicker.ItemsSource = MauiProgram.Colors.Select(x => x.Type).ToList();
        VehicleMarkPicker.ItemsSource = MauiProgram.Marks.Select(x => x.Type).ToList();
        ViolationPicker.ItemsSource = MauiProgram.Violations.Select(x => x.Type).ToList();
        StatusPicker.ItemsSource = MauiProgram.Statuses.Select(x => x.Status).ToList();
    }

    private async void SearchButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            var applicationFilter = new ApplicationFilter()
            {
                VehicleMarkId = MauiProgram.Marks.FirstOrDefault(x => x.Type == VehicleMarkPicker.SelectedItem)?.Id,
                ViolationId = MauiProgram.Violations.FirstOrDefault(x => x.Type == ViolationPicker.SelectedItem)?.Id,
                VehicleTypeId = MauiProgram.Types.FirstOrDefault(x => x.Type == VehicleTypePicker.SelectedItem)?.Id,
                VehicleColorId = MauiProgram.Colors.FirstOrDefault(x => x.Type == VehicleColorPicker.SelectedItem)?.Id,
                StatusId = MauiProgram.Statuses.FirstOrDefault(x => x.Status == StatusPicker.SelectedItem)?.Id,
                PublicationTime = _publicationDate,
                ViolationTime = _violationDate,
                VehicleNumber = VehicleNumberEntry.Text
            };

            var connectionString = MauiProgram.ApiEndpoint + "/api/Application/ByFilter?" + applicationFilter.GetFilterString();
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
            });

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