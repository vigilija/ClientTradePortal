using Blazored.LocalStorage;
using ClientTradePortal;
using ClientTradePortal.Services.Account;
using ClientTradePortal.Services.Auth;
using ClientTradePortal.Services.Http;
using ClientTradePortal.Services.Trading;
using Fluxor;
using Fluxor.Blazor.Web.ReduxDevTools;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Polly;
using Polly.Extensions.Http;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Root components (like Angular's bootstrap)
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration (like Angular environment files)
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? "https://localhost:7001";

// ============ SERVICES REGISTRATION (like Angular providers) ============

// MudBlazor UI Components
builder.Services.AddMudServices();

// Fluxor State Management (like NgRx)
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseRouting();
#if DEBUG
    options.UseReduxDevTools();
#endif
    Console.WriteLine(" Fluxor configured - scanning assemblies");
});

// Local Storage (like Angular localStorage wrapper)
builder.Services.AddBlazoredLocalStorage();

// HTTP Client with retry policies (like Angular HttpClient)
var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

// Base HttpClient
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

// Refit API Clients (like Angular HttpClient services)
builder.Services.AddRefitClient<IAccountApiClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(apiBaseUrl);
        c.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddPolicyHandler(retryPolicy);

builder.Services.AddRefitClient<ITradingApiClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(apiBaseUrl);
        c.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddPolicyHandler(retryPolicy);

builder.Services.AddRefitClient<IValidationApiClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(apiBaseUrl);
        c.Timeout = TimeSpan.FromSeconds(30);
    })
    .AddPolicyHandler(retryPolicy);

// Application Services (like Angular @Injectable services)
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITradingService, TradingService>();

Console.WriteLine(" Services registered");


builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

// Register Refit API Clients (Mock for now since API doesn't exist)
builder.Services.AddRefitClient<IAccountApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddPolicyHandler(retryPolicy);

builder.Services.AddRefitClient<ITradingApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddPolicyHandler(retryPolicy);

builder.Services.AddRefitClient<IValidationApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddPolicyHandler(retryPolicy);


//builder.Services.AddAuthorizationCore();
//builder.Services.AddCascadingAuthenticationState();
//builder.Services.AddSingleton<AuthenticationStateProvider, PersistentAuthenticationStateProvider>();

//builder.Services.AddScoped<ILocalStorageService, LocalStorageService>();------------

//Interceptors(like Angular HTTP interceptors)
//builder.Services.AddScoped<AuthenticationInterceptor>(); //----------
//builder.Services.AddScoped<ErrorInterceptor>(); //------------

// Memory Cache
//builder.Services.AddMemoryCache();----------

// Build and run
await builder.Build().RunAsync();
