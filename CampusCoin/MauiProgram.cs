﻿using System.Diagnostics;
using CampusCoin.Services;
using CampusCoin.ViewModels;
using CampusCoin.Views;
using Microsoft.EntityFrameworkCore;
using CampusCoin.Models;
using SkiaSharp.Views.Maui.Controls.Hosting;
using RegistrationPage = CampusCoin.Views.RegistrationPage;

namespace CampusCoin;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        try
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseSkiaSharp()
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("fa_solid.ttf", "FontAwesome");
                })
                .RegisterDatabase()
                .RegisterServices()
                .RegisterViewModels()
                .RegisterViews();

            return builder.Build();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            throw;
        }


     
    }
    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<MainPageViewModel>();
        mauiAppBuilder.Services.AddSingleton<GraphTestPageViewModel>();
        mauiAppBuilder.Services.AddSingleton<RegistrationPageViewModel>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder
            .Services
            .AddSingleton<MainPage>();
        mauiAppBuilder
            .Services
            .AddSingleton<GraphTestPage>();
        mauiAppBuilder
            .Services
            .AddTransient<RegistrationPage>();


        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder
            .Services
            .AddSingleton<IMessageOutputHandlingService, MessageOutputHandlingService>();
        mauiAppBuilder
            .Services
            .AddSingleton<Models.RegistrationPage>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterDatabase(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder
            .Services
            .AddDbContextFactory<CampusCoinContext>(options =>
            {
                //todo: replace with environment variable or config file rather than hard coded connection string
                options.UseSqlServer("Server=tcp:sp25greenseniorproject.database.windows.net,1433;Initial Catalog=CampusCoin;Persist Security Info=False;User ID=sp25green;Password=kennesaw123@;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            });

        return mauiAppBuilder;
    }   
}
