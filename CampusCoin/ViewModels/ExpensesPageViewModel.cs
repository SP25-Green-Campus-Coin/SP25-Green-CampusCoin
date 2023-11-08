using CampusCoin.Services;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CampusCoin.ViewModels
{
    public partial class ExpensesPageViewModel : ObservableValidator
    {
        private readonly IMessageOutputHandlingService _messageOutputHandlingService;

        [ObservableProperty]
        double bills;

        [ObservableProperty]
        double food;

        [ObservableProperty]
        double auto;

        [ObservableProperty]
        double entertainment;

        [ObservableProperty]
        double investments;

        [ObservableProperty]
        double misc;

        [ObservableProperty]
        string email;
    }
}
