# ClientTradePortal - Project Presentation

## Overview

**ClientTradePortal** is a modern web-based trading platform that enables clients to purchase Apple shares and manage their trading portfolio through an intuitive, responsive interface. Built with cutting-edge technologies, the application provides a seamless trading experience with real-time order management and comprehensive order history tracking.

![Screenshot](https://github.com/user-attachments/assets/a0cc66d1-453a-4d83-aef5-16a86b0a6638)

## Key Features

- **Apple Stock Trading**: Buy Apple shares with real-time order execution
- **Order History Management**: Track and review all trading activities
- **Account Management**: View balances, positions, and account details
- **Resilient API Communication**: Automatic retry mechanisms for reliable operations
- **Responsive UI**: Modern, material design interface that works across devices
- **State Management**: Centralized application state with predictable data flow
- **Offline Caching**: Improved performance with intelligent data caching

## Technology Stack

### Frontend Framework
- **Blazor WebAssembly** - Client-side C# web framework running in the browser via WebAssembly
- **.NET 8.0** - Latest long-term support version of .NET

### UI Components & Design
- **MudBlazor 8.13.0** - Material Design component library for Blazor
- **Bootstrap** - Responsive CSS framework for layout
- **Custom CSS** - Tailored styling for brand consistency

### State Management
- **Fluxor 6.8.0** - Redux-style state management for Blazor
  - Unidirectional data flow
  - Time-travel debugging with Redux DevTools
  - Immutable state updates

### HTTP Communication & Resilience
- **Refit 8.0.0** - Type-safe REST API client with attribute-based definitions
- **Polly 3.0.0** - Resilience and transient-fault-handling library
  - Exponential backoff retry policies
  - 30-second timeout configurations
  - Automatic error recovery

### Data & Storage
- **Blazored.LocalStorage 4.5.0** - Browser local storage integration
- **Microsoft.Extensions.Caching.Memory** - In-memory caching for performance optimization
  - 5-minute cache duration for account data
  - Reduced API calls

### Authentication (Configured)
- **Microsoft.AspNetCore.Components.WebAssembly.Authentication** - Ready for Azure AD integration

## Architecture

### Layered Architecture Pattern

```
┌─────────────────────────────────────────────┐
│     Presentation Layer (Pages/Components)   │
│         • Home.razor                        │
│         • TradingPage.razor                 │
│         • OrderHistoryPage.razor            │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│    State Management Layer (Fluxor Store)    │
│  [Actions → Effects → Reducers → State]     │
│         • AccountState/TradingState         │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│   Business Logic Layer (Services)           │
│         • AccountService                    │
│         • TradingService                    │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│   API Communication Layer (Refit)           │
│         • IAccountApiClient                 │
│         • ITradingApiClient                 │
│         • IValidationApiClient              │
└─────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────┐
│      External Backend API                   │
│    (http://localhost:5075)                  │
└─────────────────────────────────────────────┘
```

### Project Structure

```
ClientTradePortal/
├── Pages/              # Presentation layer - Routable Razor pages
├── Components/         # Reusable UI components with shared lifecycle
├── Services/           # Business logic with @Injectable services
│   ├── Account/        # Account operations & caching
│   ├── Trading/        # Trade execution logic
│   └── Http/           # Refit API client interfaces
├── Store/              # Fluxor state management (Redux pattern)
│   ├── Account/        # Account state, actions, reducers, effects
│   └── Trading/        # Trading state, actions, reducers, effects
├── Models/             # Data models and DTOs
│   ├── DTO/            # Data transfer objects
│   └── ViewModels/     # View-specific models
├── Layout/             # Master layout components
└── wwwroot/            # Static assets & configuration
```

## Key Architectural Patterns

### 1. Flux/Redux Pattern (Fluxor)
- **Unidirectional Data Flow**: Actions → Effects → Reducers → State → UI
- **Immutable State**: State never mutates directly, always replaced
- **Predictable State Changes**: All state changes go through reducers
- **Time-Travel Debugging**: Redux DevTools integration for development

### 2. Repository Pattern with Refit
- **Type-Safe API Clients**: Interface-based HTTP client definitions
- **Automatic Serialization**: JSON serialization handled automatically
- **Attribute Routing**: HTTP methods and routes defined via attributes

### 3. Resilience Patterns with Polly
- **Retry Pattern**: Exponential backoff for transient failures
- **Timeout Pattern**: 30-second timeouts prevent hanging requests
- **Circuit Breaker Ready**: Infrastructure prepared for circuit breaker patterns

### 4. Dependency Injection
- **Constructor Injection**: All dependencies injected via constructors
- **Scoped Lifetime**: Services scoped to component lifecycles
- **Testable Code**: Easy mocking for unit tests

### 5. Caching Strategy
- **Memory Caching**: IMemoryCache for frequently accessed data
- **Time-Based Expiration**: 5-minute cache for account data
- **Performance Optimization**: Reduced network calls

## Development Setup

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- Modern web browser (Chrome, Edge, Firefox)

### Configuration
API endpoint configured in `wwwroot/appsettings.json`:
```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5075"
  }
}
```

### Running the Application
```bash
dotnet restore
dotnet run
```

## Technical Highlights

### Performance
- **WebAssembly Execution**: C# code runs directly in the browser at near-native speeds
- **Memory Caching**: Reduced API calls with intelligent caching
- **Lazy Loading**: Components loaded on-demand

### Reliability
- **Automatic Retries**: Polly handles transient failures with exponential backoff
- **Timeout Protection**: 30-second timeouts prevent hanging operations
- **Error Boundaries**: Graceful error handling throughout the application

### Maintainability
- **Clean Architecture**: Clear separation of concerns across layers
- **Type Safety**: Strong typing throughout C# codebase
- **Implicit Usings**: Reduced boilerplate with .NET 8 features
- **Nullable Reference Types**: Compile-time null safety

### Developer Experience
- **Redux DevTools**: State inspection and time-travel debugging
- **Hot Reload**: Fast development with .NET 8 hot reload
- **IntelliSense**: Full IDE support with C# type system

## Future Enhancements

- Azure AD Authentication integration
- Real-time price updates with SignalR
- Multi-stock trading support
- Portfolio analytics and charts
- Mobile-responsive improvements
- Progressive Web App (PWA) capabilities

---

**Built with modern web technologies for a seamless trading experience.**
