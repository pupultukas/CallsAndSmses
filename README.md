# CallsAndSmses
.NET Core EF MVC technologies example project:

* msisdn database created localy on MSSQL server

* EF connected to exsisting DB

* MVC used for frontend development

Models:
* Calls.cs, Sms.cs, msisdnContext.cs - EF data from database 
* MainList.cs - class for data reporting to users in views

Controllers:
HomeController with different action inside
* Index (start page)
* Top5Call (Report for TOP 5 msisdn by duration)
* Top5Sms (Report for TOP 5 msisdn by sms count)
* Count (Report for counting total calls or smses)
* AllInfo (Method for getting all data from tables)

Views:
The same as actions in HomeController
