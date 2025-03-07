@echo off
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\WslLocalHost.pfx -p WslLocalHost
dotnet dev-certs https --trust
setx WSLENV USERPROFILE/up