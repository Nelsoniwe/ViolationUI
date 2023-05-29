using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UI.Models;

namespace UI.Pages;

public partial class ApplicationListWithSearch : ContentPage
{
    private readonly HttpClient _httpClient;
    private List<ApplicationModel> _applications;
    private UserProfileModel _userProfile;

    public ApplicationListWithSearch(List<RepresentativeApplication> applications)
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        ApplicationsView.ItemsSource = applications;
    }

    private async void DetailsButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            Button button = (Button)sender;
            RepresentativeApplication selectedItem = (RepresentativeApplication)button.BindingContext;

            await Navigation.PushAsync(new ApplicationDetails(selectedItem));
        }
        catch (Exception exception)
        {
            await DisplayAlert("Помилка", exception.Message, "OK");
        }
    }

    private async void ResolveButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            Button button = (Button)sender;
            RepresentativeApplication selectedItem = (RepresentativeApplication)button.BindingContext;

            await Navigation.PushAsync(new ChangeApplicationDataPage(selectedItem));
        }
        catch (Exception exception)
        {
            await DisplayAlert("Помилка", exception.Message, "OK");
        }
    }
}