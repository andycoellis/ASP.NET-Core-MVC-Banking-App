#### Summary

ASP.NET Core Banking Application
National Wealth Bank of Australasia

#### Application Features

- Check balances
- Modify a personal profile
- Simulate transactions such as deposits and withdrawals
- Transfer money
- Schedule payments

#### BillPay Feature
> BillPay can perform such tasks as background scheduled transactions. In this project it
utilises the `ScheduledHostService` class created for this application. This class uses `IHostedService` and `IDisposable`. It performs a interval check of 5 seconds. If at any time a transaction occurs where the holding account does not have enough funds then the BillPay lodge is deleted an and error logged to the system recorded.

#### System Requirements

**ASP.NET Core**
.NET Core SDK 3.1.101
.Net Core Runtime 3.1.1

VisualStudio Version 8.4.2 (build 59)

#### Dependencies

**Frameworks**
- Microsoft.AspNetCore.App (3.1.0)
- Microsoft.NETCore.App (3.1.0)

**NuGet** *packages*

- Microsoft.AspNetCore.Mvc.Razor.RuntimeComplilation (3.1.1)
- Microsoft.EntityFrameworkCore.Design (3.1.1)
- Microsoft.EntityFrameworkCore.SqlServer (3.1.1)
- Microsoft.EntityFrameworkCore.Tools (3.1.1)
- Microsoft.Extensions.Hosting (3.1.1)
- Microsoft.Extensions.Logging.Debug (3.1.1)
- Microsoft.VisualStudio.WebCodeGeneration.Design (3.1.1)
- SimpleHashing (1.0.3.1)
- X.PagedList.Mvc.Core (7.9.1)


#### Application Architecture
The application was built on ASP.NET Core MVC

- Folder Structure (Notes of Importance)

 Project
 
```bash
├── Attributes
│   ├── Authorisation
│
├── Controllers
│   ├── VariousControllers.cs
│
├── Data
│   ├── ContextFile.cs
│   ├── SeedData
│
├── Exceptions
│   ├── CustomSexceptions
│
├── Models
│   ├── ModelClasses.cs
│
├── Services
│   ├── BankingServices
│   │   ├── VariousServiceClasses.cs (Handle of Business Loginc)
│   │
│   ├── Validation 
│   │   ├── Validation.cs
│   │
│   ├── BankEngine.cs
│   ├── ScheduleHostService.cs
│   ├── StateService.cs
│
├── Views
│   ├── VariousViews.cshtml
│
├── appsettings.json
├── Program.cs
├── Startup.cs
```

