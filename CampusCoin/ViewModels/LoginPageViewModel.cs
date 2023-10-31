﻿using CampusCoin.Services;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CampusCoin.Views;
using CampusCoin.Validation;

namespace CampusCoin.ViewModels;

public partial class LoginPageViewModel : ObservableValidator
{
    private readonly IMessageOutputHandlingService _messageOutputHandlingService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    [ObservableProperty]
    string title;

    [EmailValidation]
    [ObservableProperty]
    string email;

    //[PasswordValidation]
    [ObservableProperty]
    string password;

    public bool IsNotBusy => !IsBusy;

    LoginService loginService;

    public LoginPageViewModel(LoginService loginService, IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.loginService = loginService;
        _messageOutputHandlingService = messageOutputHandlingService;
    }

    [RelayCommand]
    async Task GetUserByEmailAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            var user = await loginService.GetUserByEmail(Email);
    
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to get users: {ex.Message}" , "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task Login()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy= true;

            var potentialUser = new Users();
            potentialUser.Email = Email;
            potentialUser.Password = Password;

            ValidateAllProperties();
            if (!HasErrors)
            {
                Users matchedUser = await loginService.GetUserByEmail(Email);

                if (matchedUser == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Email not registered", "OK");
                    return;
                }
                if (potentialUser.Password != matchedUser.Password)
                    await Shell.Current.DisplayAlert("Error", "Invalid Password", "OK");

                else
                    // Temporary route to potential post-login view
                    await Shell.Current.GoToAsync(nameof(ExpensesPage));
            }
            else
            {
                await _messageOutputHandlingService.OutputValidationErrorsToUser(GetErrors());
            }
        }
        catch(Exception ex ) 
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error",
                $"Invalid email", "OK");
        }
        finally 
        { 
            IsBusy = false; 
        }
    }
}
