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
using ecosystem.Models.Entities.Plants;

namespace ecosystem;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        
        services.AddSingleton<IWorldService, WorldService>();
        services.AddSingleton<IEntityFactory, EntityFactory>();
        services.AddSingleton<ISimulationEngine, SimulationEngine>();
        services.AddSingleton<IEntityLocator<Animal>, WorldEntityLocator<Animal>>();
        services.AddSingleton<IEntityLocator<Plant>, WorldEntityLocator<Plant>>();
        services.AddSingleton<ITimeManager, TimeManager>();
        services.AddTransient<MainWindowViewModel>();

        return services.BuildServiceProvider();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        try
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var seed = new Random().Next();
                RandomHelper.Initialize(seed);

                Services = ConfigureServices();
                var worldService = Services.GetRequiredService<IWorldService>();

                worldService.ResetGrid();

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