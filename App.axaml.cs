using System;
using System.Linq;
using Avalonia;
using Avalonia.Threading;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using ecosystem.Services.Factory;
using ecosystem.Services.Simulation;
using ecosystem.Services.World;
using ecosystem.ViewModels;
using ecosystem.Views;
using ecosystem.Helpers;
using ecosystem.Models.Behaviors;
using ecosystem.Models.Entities.Animals;

namespace ecosystem;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var services = new ServiceCollection();
            
                // Register services
                services.AddSingleton<IWorldService, WorldService>();
                services.AddSingleton<IEntityFactory, EntityFactory>();
                services.AddSingleton<ISimulationEngine, SimulationEngine>();
                services.AddSingleton<IEntityLocator<Animal>, WorldEntityLocator<Animal>>();
                services.AddTransient<MainWindowViewModel>();

                Services = services.BuildServiceProvider();

                var viewModel = Services.GetRequiredService<MainWindowViewModel>();
            
                desktop.MainWindow = new MainWindow
                {
                    DataContext = viewModel
                };

                Dispatcher.UIThread.Post(() => viewModel.InitializeAndStart());
            }

            base.OnFrameworkInitializationCompleted();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnFrameworkInitializationCompleted: {ex}");
            throw;
        }
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}