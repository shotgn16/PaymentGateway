## Table of Contents
1. [General Info](#general-info)
2. [Technologies](#technologies)
3. [Installation](#installation)
4. [Collaboration](#collaboration)
5. [FAQs](#faqs)
### General Info
***
Write down general information about your project. It is a good idea to always put a project status in the readme file. This is where you can add it. 
### Screenshot
![Image text](https://www.myq-solution.com/themes/myq/img/vector/logo-myq.svg?v=230206142735)
## Technologies
***
A list of technologies used within the project:
* Flurl(https://flurl.dev/): Version 3.0.7 
* Flurl.Http(https://flurl.dev/): Version 3.2.4
* MailKit(http://www.mimekit.net/): Version: 3.6.0
* MimeKit(http://www.mimekit.net/): Version: 3.6.0
* Microsoft.SqlServer.Compact(https://www.microsoft.com/en-gb/download/details.aspx?id=30709): Version: 4.0.8876.1
* Newtonsoft.Json(https://www.newtonsoft.com/json): Version: 13.0.3
* NLog(https://nlog-project.org): Version: 5.1.2
* Nlog.MailKit(https://github.com/NLog/NLog.MailKit): Version: 5.0.0
* Portable.BouncyCastle(https://www.bouncycastle.org/csharp/): Version: 1.9.0
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
