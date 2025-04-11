@echo off
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\WslLocalhost.pfx -p WslLocalhost
dotnet dev-certs https --trust
setx WSLENV USERPROFILE/up