## Table of Contents
1. [General Info](#general-info)
2. [Technologies](#technologies)
3. [Installation](#installation)
4. [Collaboration](#collaboration)
5. [FAQs](#faqs)
### General Info
***
As part of my BCS L4 Software Developer Apprenticeship I was tasked with undertaking the design and creation of a business project to cover a selection of Knowledge, Skills and Behaviours (KSBs) contributing to the progression of the apprenticeship.

The project in question, was based around a task to design a software application capable of syncing user credits from ParentPay, a school-focused financial management system to MyQ X, an on-premise print-management solution designed to revolutionise printing in the workplace and school environment.

The development of the project lasted 2 years, with many revisions and amendments over time. The project was designed in Microsoft .NET Framework using the C# programming language. Implementing various in-house and third-party libraries, the application focuses on a self-service approach and uses a password protected SQL Compact v4 database, which is handled entirely by the application code. 

The application uses a global approach for the HTTP Client with any and all API calls, integrating a HTTP Wrapper and providing bespoke validation methods for each API endpoint used within the system.

Due to customer security requirements the application was configured to default to TLS 1.2 or 1.3 for any and all external API calls, throwing an error if a call is not secure. Should a customer wish to overwrite this and use older protocols such as TLS 1.1, 1.0, an option was provided in the configuration file provided. 

The system would also use a multi-option user-sync system to search for a user in MyQ X by either username, through their Active Directory user or by syncing with Azure Active Directory, also known as Microsoft Graph. 
### Screenshot
![Image text](https://www.myq-solution.com/themes/myq/img/vector/logo-myq.svg?v=230206142735)
## Technologies
***
A list of technologies used within the project:
* Azure.Core(https://www.nuget.org/packages/Azure.Core): 1.34.0
* Azure.Identity(https://www.nuget.org/packages/Azure.Identity): 1.9.0
* CsvHelper(https://joshclose.github.io/CsvHelper/): 30.0.1
* Flurl(https://flurl.dev/): Version 3.0.7 
* Flurl.Http(https://flurl.dev/): Version 3.2.4
* MailKit(http://www.mimekit.net/): Version: 4.1.0
* MimeKit(http://www.mimekit.net/): Version: 4.1.0
* Microsoft.SqlServer.Compact(https://www.microsoft.com/en-gb/download/details.aspx?id=30709): Version: 4.0.8876.1
* Newtonsoft.Json(https://www.newtonsoft.com/json): Version: 13.0.3
* NLog(https://nlog-project.org): Version: 5.2.2
* NLog.DiagnosticSource(https://nlog-project.org): Version 5.0.0
* Nlog.MailKit(https://github.com/NLog/NLog.MailKit): Version: 5.0.0
* Portable.BouncyCastle(https://www.bouncycastle.org/csharp/): Version: 2.2.1
* SharpZipLib(https://github.com/icsharpcode/SharpZipLib): Version: 1.4.2

## Installation
***
A little intro about the installation. 
```
1. Download / clone repository to a directory of your choice
2. Open project with Visual Studio
3. Ensure all third-party libaries are installed (as listed above)
4. Set configuration manager to 'Release' & Build

Notes
* The the dbInstaller and README are automatically copied to the output solution.
* The SQLCompact installer IS NOT copied and will need to be manaully copied to the '{projectDirectory}/lib' directory. 
```
## Functionality
*** 
### What can it do? 
* Advanced logging
* User retrival (Username / Active Directory, AzureAD)
* Certificate Validation / Installiation
* Custom TLS Protocol
* Use of Hostname / IP address
* Automatic MyQ Token generation
* Periodic ParentPay Till balance update
* Periodic MyQ Balance update
* Transaction tracking - Database integration
* Updatable address whitelisting
* Generate support file upon crash

## Coming Soon
***
* Licensing
## FAQs
***
A list of frequently asked questions
1. **How do I install the application on my server?**
You can install the application by following the instructions below:
* Install the application using the provided installer
* Open the install folder using the link created on your desktop and open the 'config/config.json'
* In your MyQ server, please enable credit for your chosen users / group. This can be done under MyQ > Settings > Credit > Users and Groups > Add.
* Please enable 'External Payment Providers'
* In your MyQ server, create a REST API app (MyQ > Settings > REST API Apps > Add. 
* Name the app whatever you would like and set the scope to 'credit, users'. - _Don't forget to note down the clientid, secret!_
* Fill in the MyQ section of the config with the details as you've just created. 
* Enter the server address as listed (This can be either hostname or IP address). _Please make sure the address used in located in the server alternate names (MyQ > Settings > Network > Communication Security)_
* Fill in the ParentPay information accordingly, using your school credentials. 
* Enter the name of your school into the 'InstallationName'.
* If you wish to use a TLS version other then 1.2 (default), then please specify it under 'TLSVersion', you can do this by removing the '.' between the version as shown 12 = 1.2, 11 = 1.1 (__TLS 1.3 is coming soon!__)
* For those who want to know by email when the app crashes, you can setup SMTP details to have them emailed to you.
* Once done, please save the configuration and proceed to run the application. 
* If the configuration is valid, the application will immediately download payment data from ParentPay and upload it to MyQ.

2. **Can you install this on a domain controller?** 
* _Yes, the app can be installed anywhere, providing the following requirements are met:_
* The machine can communicate with MyQ
* The machine has an internet connection and can connect to 'https://ParentPay.com'
* You have the MyQ Certificate installed on the target machine
* The target machine is domain joined (Preferred)

3. **What versions of MyQ can this be used with?**
* This application can currently be used with any version of MyQ from 8.0 upwards.
