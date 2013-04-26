@echo off
.\packages\NUnit.Runners.2.6.2\tools\nunit-console.exe .\src\dannyboy.tests\bin\Debug\dannyboy.tests.dll
del /Q TestResult.xml
pause