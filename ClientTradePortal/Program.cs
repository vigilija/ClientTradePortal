var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Root components 
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? "http://localhost:5075";

// MudBlazor UI Components
builder.Services.AddMudServices();

// Fluxor State Management 
builder.Services.AddFluxor(options =>
{
    options.ScanAssemblies(typeof(Program).Assembly);
    options.UseRouting();
    Console.WriteLine(" Fluxor configured - scanning assemblies");
});

// Local Storage 
builder.Services.AddBlazoredLocalStorage();

// HTTP Client with retry policies
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

// Application Services 
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITradingService, TradingService>();

// Register Refit API Clients
builder.Services.AddRefitClient<IAccountApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddPolicyHandler(retryPolicy);

builder.Services.AddRefitClient<ITradingApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddPolicyHandler(retryPolicy);

builder.Services.AddRefitClient<IValidationApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(apiBaseUrl))
    .AddPolicyHandler(retryPolicy);
builder.Services.AddMemoryCache();

// Build and run
await builder.Build().RunAsync();
