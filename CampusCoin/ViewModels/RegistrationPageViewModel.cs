using CampusCoin.Services;
using CampusCoin.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CampusCoin.Views;
using System.Collections.Generic;
using CampusCoin.Validation;
using Microsoft.Data.SqlClient;

namespace CampusCoin.ViewModels;

public partial class RegistrationPageViewModel : ObservableValidator
{

    private readonly IMessageOutputHandlingService _messageOutputHandlingService;
    private readonly RegistrationService registrationService;
    private readonly EmailService emailService;
    private readonly PersistedLoginService persistedLoginService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    private bool isBusy;

    [ObservableProperty]
    private string title;

    [EmailValidation]
    [ObservableProperty]
    private string email;

    [PasswordValidation]
    [ObservableProperty]
    private string password;

    [PhoneNumberValidation]
    [ObservableProperty]
    private string phoneNumber;

    [FirstnameValidation]
    [ObservableProperty]
    private string firstName;

    [LastnameValidation]
    [ObservableProperty]
    private string lastName;

    [ObservableProperty]
    private string verificationCode;

    [ObservableProperty]
    private bool isVerificationVisible;

    [ObservableProperty]
    private string errorText;

    [ObservableProperty]
    private List<String> errorList;

    private bool IsNotBusy => !IsBusy;
    private bool VerificationEntered = false;

    private ObservableCollection<User> UsersCollection { get; } = new();

    public RegistrationPageViewModel(RegistrationService registrationService, EmailService emailService, PersistedLoginService persistedLoginService ,IMessageOutputHandlingService messageOutputHandlingService)
    {
        this.registrationService = registrationService;
        this.emailService = emailService;
        this.persistedLoginService = persistedLoginService;
        _messageOutputHandlingService=messageOutputHandlingService;
        this.isVerificationVisible = false;
    }

    [RelayCommand]
    private async Task GetUsersAsync()
    {
        try
        {
            var users = await registrationService.GetUsers();

            if (UsersCollection.Count != 0)
                UsersCollection.Clear();

            foreach (var user in users)
                UsersCollection.Add(user);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            await Shell.Current.DisplayAlert("Error!",
                $"Unable to get users: {ex.Message}", "OK");
        }
        finally
        {
        }
    }

    [RelayCommand]
    private async Task Registration()
    {
        ErrorText = "";
        try
        {
            var user = new User();
            await GetUsersAsync();
            user = SetUserVals(user);
            
            ValidateAllProperties();
            if (!HasErrors)
            {
                user = HashUserPassword(user);
                await emailService.SendVerificationEmail(user.Email);
                SetVisibilityOfVerification(true);
                await Shell.Current.DisplayAlert("Code sent", "Verification Code was sent to: " + user.Email + "\n\nPlease allow up to 3 minutes for code to arrive", "OK");


                while (true)
                {
                    // Wait for the verification code to be entered
                    while (!VerificationEntered)
                    {
                        await Task.Delay(100); // Wait for .1 second before checking again
                    }

                    if (VerificationCode == emailService.verificationCode.ToString())
                    {
                        try
                        {
                            await registrationService.RegisterUser(user);
                            await EmailService.SendRegistrationSuccessEmail(user.Email);
                        }
                        catch (Exception ex)
                        {
                            // if it is a duplicate email error
                            Debug.WriteLine(ex);
                            if (ex.ToString().Contains("UniqueEmail"))
                            {
                                await Shell.Current.DisplayAlert("Error", "Email already registered", "OK");
                                return;
                            }
                            else
                            {
                                await Shell.Current.DisplayAlert("Error", "Something went wrong. Please try again.", "OK");
                                return;
                            }
                        }
                        finally
                        {
                            IsBusy = false;
                        }

                        await Shell.Current.DisplayAlert("Account registered!", "Your account has been successfully registered", "OK");
                        ResetValues();
                        SetVisibilityOfVerification(false);
                        await Shell.Current.GoToAsync(nameof(MainPage));
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Error", "Invalid verification code. Please try again.", "OK");
                        VerificationCode = null;
                        VerificationEntered = false; 
                    }
                }
            }
            else
            {
                await _messageOutputHandlingService.OutputValidationErrorsToUser(GetErrors());
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }
    }

    [RelayCommand]
    private void Verification()
    {
        VerificationEntered = true;
    }

    private User SetUserVals(User user)
    {
        user.Email = Email;
        user.Salt = SaltHashService.GenerateSalt();
        user.Password = Password;
        user.PhoneNumber = PhoneNumber;
        user.FirstName = FirstName;
        user.LastName = LastName;
        user.AuthToken = PersistedLoginService.generateAuthToken();
        return user;
    }

    private User HashUserPassword(User user)
    {
        user.Password = SaltHashService.HashPassword(Password, user.Salt);
        return user;
    }

    private void SetVisibilityOfVerification(bool visibileStatus)
    {
        IsVerificationVisible = visibileStatus;
    }

    private void ResetValues()
    {
        Email = null;
        Password = null;
        PhoneNumber = null;
        FirstName = null;
        LastName = null;
        VerificationCode = null;
        VerificationEntered = false;
    }
}
