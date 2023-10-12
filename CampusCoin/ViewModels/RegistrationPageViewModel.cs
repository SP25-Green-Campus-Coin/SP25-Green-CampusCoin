using CommunityToolkit.Mvvm.ComponentModel;

namespace CampusCoin.ViewModels;
    
public partial class RegistrationPageViewModel : ObservableObject
{
    public string Title => "Registration Page";
    public string Description => "Create a CampusCoin Account";
    public string SubmitRegistrationText => "Register";
    //[RelayCommand]
    //void SubmitRegistrationCommand() {}
}

