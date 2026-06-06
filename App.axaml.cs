using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
//Utilizamos el dependecy injection para registrar los servicios y repositorios
using Microsoft.Extensions.DependencyInjection;
using SistemaDistribuidora.Models;
//Importamos los namespaces de los repositorios y modelos
using SistemaDistribuidora.Repositories.Implementation;
using SistemaDistribuidora.Repositories.Interfaces;
using System;

namespace SistemaDistribuidora
{
    public partial class App : Application
    {
        // Esta propiedad estática almacena nuestro contenedor ya construido
        public static IServiceProvider ServiceProvider { get; private set; } = null!;
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var Services = new ServiceCollection(); 

            Services.AddTransient<ICategoryRepository, CategoryRepository>();

            ServiceProvider = Services.BuildServiceProvider(); 

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}