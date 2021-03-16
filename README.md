# NTcc-TransactionCore - The .NET Core port based on tcc-transaction 


## Introduction

This is the README file for NTcc-TransactionCore, .NET Core port of Java tcc-transaction. It supports .NET Core/netstandard 2.0 and later, not supported for .NET Framework.

NTcc-TransactionCore is an opensource project aimed at creating a free-for-commercial use TCC transaction, with enterprise features.

## Architecture overview
![Architecture.png](https://github.com/wzl-bxg/NTcc-TransactionCore/blob/main/TCC%20Architecture.png)

## Getting Started

### NuGet

NTcc-TransactionCore can be installed in your project with the following command.

~~~shell
 PM> Install-Package NTccTransactionCore
~~~

NTcc-TransactionCore Currently supports Oracle, SqlServer as transaction log storage, following packages are available to install:

~~~shell
 PM> Install-Package NTccTransactionCore.Oracle
 PM> Install-Package NTccTransactionCore.SqlServer
~~~

### Configuration

First, you need to configure NTcc-TransactionCore in your <font color="#28a745">`Startup.cs`</font>：

~~~c#
   public ILifetimeScope AutofacContainer { get; private set; }

   public IServiceCollection Services { get; private set; }

   public void ConfigureServices(IServiceCollection services)
   {
       //......
           
        services.AddNTccTransaction((option) =>
        {
            option.UseOracle((oracleOption) =>
            {
                oracleOption.ConnectionString = Configuration.GetConnectionString("Your ConnectionStrings");// 						configure db connectiong
            });

            option.UseCastleInterceptor(); // use Castle Interceptor
        });

        Services = services;
    }

    public void ConfigureContainer(ContainerBuilder containerBuilder)
    {
        containerBuilder.Register(Services); // register Castle Interceptor
    }

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();
    }
~~~

### DB Script

Currently only supports Oracle, SqlServer database, execute the following database script to create transaction table:

#### Oracle

~~~sql
CREATE TABLE NTCC_TRANSACTION 
(
    TRANSACTION_ID NVARCHAR2(128) NOT NULL 
	, GLOBAL_TRANSACTION_ID NVARCHAR2(128)
	, BRANCH_QUALIFIER NVARCHAR2(128) 
	, STATUS NUMBER(9, 0) NOT NULL 
	, TRANSACTION_TYPE NUMBER(9, 0) NOT NULL 
	, RETRIED_COUNT NUMBER(9, 0) NOT NULL 
	, CREATE_UTC_TIME DATE NOT NULL 
	, LAST_UPDATE_UTC_TIME DATE NOT NULL 
	, VERSION NUMBER(9, 0) NOT NULL 
	, CONTENT CLOB 
	, CONSTRAINT PK_NTCC_TRANSACTION PRIMARY KEY 
	  (
	    TRANSACTION_ID 
	  )
);
~~~

#### Sql Server

~~~sql
CREATE TABLE [dbo].[NTCC_TRANSACTION] 
(
	[TRANSACTION_ID] nvarchar(128) NOT NULL 
	,[GLOBAL_TRANSACTION_ID] nvarchar(128) NULL 
	,[BRANCH_QUALIFIER] nvarchar(128) NULL 
	,[STATUS] int NOT NULL 
	,[TRANSACTION_TYPE] int NOT NULL 
	,[RETRIED_COUNT] int NOT NULL 
	,[CREATE_UTC_TIME] datetime NOT NULL 
	,[LAST_UPDATE_UTC_TIME] datetime NOT NULL 
	,[VERSION] int NOT NULL 
	,[CONTENT] nvarchar(MAX) NULL 
	,PRIMARY KEY 
	(
		[TRANSACTION_ID]
	)
)
~~~



### In Business Logic Service

In your business service, you need implement <font color="#28a745">`INTccTransactionService`</font>:

~~~c#
   public class OrderService : IOrderService, INTccTransactionService
   {
        private readonly ILogger<OrderService> _logger;
        private readonly ICapitalProxy _capitalProxy;

        public OrderService(ILogger<OrderService> logger, ICapitalProxy capitalProxy)
        {
            _logger = logger;
            _capitalProxy = capitalProxy;
        }

        [Compensable(CancelMethod = "CancelOrder", ConfirmMethod = "ConfirmOrder")]
        public async Task<string> TryPostOrder(string input, TransactionContext transactionContext = null)
        {
              return await Task.FromResult("");
        }

        public async Task ConfirmOrder(string input, TransactionContext transactionContext = null)
        {
            await Task.CompletedTask;
        }

        public async Task CancelOrder(string input, TransactionContext transactionContext = null)
        {
            await Task.CompletedTask;
        }
   }
~~~

And add the attribute  <font color="#28a745">`[Compensable(CancelMethod = "xxx", ConfirmMethod = "xxx")]` </font>on the `Try` method:

 ~~~c#
 	[Compensable(CancelMethod = "CancelOrder", ConfirmMethod = "ConfirmOrder")]
    public async Task<string> TryPostOrder(string input, TransactionContext transactionContext = null)
    {       
        return await Task.FromResult("");
    }
 ~~~

The type of the last parameter of the <font color="#28a745">`Try Method`</font> must be <font color="#28a745">`TransactionContext`</font>, it's used to propagate transactions, and you need add <font color="#28a745">`Confirm Method`</font> and <font color="#28a745">`Cancel Method`</font> in the business logic service，the parameters of the two methods must be same as <font color="#28a745">`Try Method`</font>.

~~~C#
   public async Task ConfirmOrder(string input, TransactionContext transactionContext = null)
   {
       await Task.CompletedTask;
   }

   public async Task CancelOrder(string input, TransactionContext transactionContext = null)
   {
       await Task.CompletedTask;
   }
~~~

Then register your class that implement   <font color="#28a745">`INTccTransactionService`</font>  in  <font color="#28a745">`Startup.cs`</font> :

```c#
   public void ConfigureServices(IServiceCollection services)
   {
		services.AddTransient<IOrderService, OrderService>();
	  	services.AddTransient<OrderService>();
	}
```

### Contribute

One of the easiest ways to contribute is to participate in discussions and discuss issues. You can also contribute by submitting pull requests with code changes.