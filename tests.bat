@echo off
.\packages\NUnit.Runners.2.6.2\tools\nunit-console.exe .\src\Swxben.DataAccess.Tests\bin\Debug\Swxben.DataAccess.Tests.dll
del /Q TestResult.xml
pause