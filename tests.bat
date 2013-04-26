@echo off
.\packages\NUnit.Runners.2.6.2\tools\nunit-console.exe .\src\swxben.dannyboy.tests\bin\Debug\swxben.dannyboy.tests.dll
del /Q TestResult.xml
pause