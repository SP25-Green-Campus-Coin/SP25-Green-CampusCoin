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
    bool isBusy;

    [ObservableProperty]
    string title;

    [EmailValidation]
    [ObservableProperty]
    string email;

    //[PasswordValidation]
    [ObservableProperty]
    string password;

    [ObservableProperty]
    string newPassword;

    [ObservableProperty]
    string confirmNewPassword;

    //public bool IsNotBusy => !IsBusy;

    LoginService loginService;
    EmailService emailService;
    PersistedLoginService persistedLoginService;

    public LoginPageViewModel(LoginService loginService, EmailService emailService, PersistedLoginService persistedLoginService ,IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.loginService = loginService;
        this.emailService = emailService;
        this.persistedLoginService = persistedLoginService;
        _messageOutputHandlingService = messageOutputHandlingService;
    }

    [RelayCommand]
    async Task Login()
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
                    await persistedLoginService.login(matchedUser.Email);
                    // Route to post-login view
                    await Shell.Current.GoToAsync(nameof(ExpensesPage));
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
        }
        finally 
        { 
            IsBusy = false; 
        }
    }

    [RelayCommand]
    async Task SignUp()
    {
        ResetValues();
        await Shell.Current.GoToAsync(nameof(RegistrationPage));
    }

    [RelayCommand]
    async Task ForgotPassword()
    {
        ResetValues();
        await Shell.Current.GoToAsync(nameof(ResetPasswordPage));
    }

    public void ResetValues()
    {
        Email = null;
        Password = null;
    }

}
