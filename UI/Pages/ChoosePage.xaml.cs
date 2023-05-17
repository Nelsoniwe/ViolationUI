using System.ComponentModel;

namespace UI.Pages;

public partial class ChoosePage : ContentPage
{
    public ChoosePage()
	{
        InitializeComponent();

        NavigationPage.SetHasBackButton(this, false);
        CustomInitializeComponent();
    }

    private async void CustomInitializeComponent()
    {
        string role = await SecureStorage.Default.GetAsync("user_role");
        if (role == "Admin")
        {
            ResolveApplications.IsVisible = true;
        }
        else
        {
            ResolveApplications.IsVisible = false;
        }
    }

    protected override bool OnBackButtonPressed()
    {
        return true;
    }

    private async void CreateApplicationButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CreateApplicationPage());
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