using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using SistemaDistribuidora.Repositories.DataBaseConnection;
using SistemaDistribuidora.Repositories.Implementation;
using SistemaDistribuidora.Repositories.Interfaces;
using SistemaDistribuidora.Services; 
using SistemaDistribuidora.Services.Interfaces; // <-- Aquí está tu INavigationService
using SistemaDistribuidora.Services.Implementation;

using System;
using SistemaDistribuidora.Views;
using SistemaDistribuidora.ViewModels;
using Microsoft.Extensions.DependencyInjection.Extensions;


namespace SistemaDistribuidora
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var services = new ServiceCollection();

            // INFRAESTRUCTURA Y CACHe
            services.AddMemoryCache();
            services.AddSingleton<IAppCache, AppCache>();

            // REPOSITORIOS
            services.AddSingleton<IDataBase, DataBase>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<ICustomerRepository, CustomerRepository>();
            services.AddSingleton<IEmployeeRepository, EmployeeRepository>();
            services.AddSingleton<ISuplierRepository, SupplierRepository>();
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IPurchaseRepository, PurchaseRepository>();
            services.AddSingleton<ISaleRepository, SaleRepository>();

            // SERVICES
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<ICustomerService, CustomerService>();
            services.AddSingleton<IEmployeeService, EmployeeService>();
            services.AddSingleton<ISupplierService, SupplierService>();
            services.AddSingleton<IProductService, ProductService>();
            services.AddSingleton<IPurchaseService, PurchasesService>();
            services.AddSingleton<ISaleService, SaleService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // VIEWMODELS Y VISTAS
            services.AddTransient<MainMenuViewModel>(provider =>
                new MainMenuViewModel(provider.GetRequiredService<INavigationService>()));

            services.AddTransient<LoginViewModel>(provider =>
                    new LoginViewModel(
                    provider.GetRequiredService<IUserService>(),
                    provider.GetRequiredService<INavigationService>()));

            services.AddTransient<MainMenuView>();
            services.AddTransient<MainMenuViewModel>();

            services.AddTransient<LoginView>();
            services.AddTransient<LoginViewModel>();

            services.AddTransient<CustomerView>();
            services.AddTransient<CustomerViewModel>();

            services.AddTransient<SupplierView>();
            services.AddTransient<SupplierViewModel>();

            services.AddTransient<EmployeeView>();
            services.AddTransient<EmployeeViewModel>();

            services.AddTransient<CategoryView>();
            services.AddTransient<CategoryViewModel>();

            services.AddTransient<ProductView>();
            services.AddTransient<ProductViewModel>();

            services.AddTransient<SaleView>();
            services.AddTransient<SaleViewModel>();

            services.AddTransient<PurchaseView>();
            services.AddTransient<PurchaseViewModel>();

            ServiceProvider = services.BuildServiceProvider(); 
          

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {

                var navigationService = ServiceProvider.GetRequiredService<INavigationService>();

                var mainWindow = new MainWindow();

                mainWindow.DataContext = ServiceProvider.GetRequiredService<LoginViewModel>();

                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}