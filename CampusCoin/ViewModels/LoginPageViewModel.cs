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
    private readonly LoginService loginService;
    private readonly EmailService emailService;
    private readonly PersistedLoginService persistedLoginService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string title;

    [EmailValidation]
    [ObservableProperty]
    private string email;

    [ObservableProperty]
    private string password;

    [ObservableProperty]
    private string newPassword;

    [ObservableProperty]
    private string confirmNewPassword;

    [ObservableProperty]
    private bool rememberMe;

    public LoginPageViewModel(LoginService loginService, EmailService emailService, PersistedLoginService persistedLoginService, IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.loginService = loginService;
        this.emailService = emailService;
        this.persistedLoginService = persistedLoginService;
        _messageOutputHandlingService = messageOutputHandlingService;
    }

    [RelayCommand]
    private async Task Login()
    {
        try
        {
            ValidateAllProperties();
            if (!HasErrors)
            {
                User matchedUser = await loginService.GetUserByEmail(Email);

                if (matchedUser == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Email not registered", "OK");
                    return;
                }
                if (SaltHashService.HashPassword(Password, matchedUser.Salt) != matchedUser.Password)
                { 
                    await Shell.Current.DisplayAlert("Error", "Incorrect Password", "OK"); 
                    return;
                }
                else 
                {
                    ResetValues();
                    await persistedLoginService.login(matchedUser.UserId);
                    if (RememberMe)
                    {
                        await persistedLoginService.SaveAuthToken();
                    }
                    // Route to post-login view
                    await Shell.Current.GoToAsync(nameof(DashboardPage));
                }
            }
            else
            {
                await _messageOutputHandlingService.OutputValidationErrorsToUser(GetErrors());
            }
        }
        catch(Exception ex) 
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error",
                $"Something went wrong :(", "OK");
            throw;
        }
        finally 
        { 
            IsBusy = false; 
        }
    }

    [RelayCommand]
    private async Task SignUp()
    {
        ResetValues();
        await Shell.Current.GoToAsync(nameof(RegistrationPage));
    }

    [RelayCommand]
    private async Task ForgotPassword()
    {
        ResetValues();
        await Shell.Current.GoToAsync(nameof(ResetPasswordPage));
    }

    private void ResetValues()
    {
        Email = null;
        Password = null;
    }

}
