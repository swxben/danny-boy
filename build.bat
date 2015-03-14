@echo off

if exist "%VS120COMNTOOLS%vsvars32.bat" call "%VS120COMNTOOLS%vsvars32.bat" & goto VARSSET
if exist "%VS110COMNTOOLS%vsvars32.bat" call "%VS110COMNTOOLS%vsvars32.bat" & goto VARSSET
if exist "%VS100COMNTOOLS%vsvars32.bat" call "%VS100COMNTOOLS%vsvars32.bat" & goto VARSSET
echo "Could not detect VS version!" & goto ERROR
:VARSSET

mkdir log
mkdir lib\net40
mkdir nupkg_archive

msbuild.exe "src\dannyboy\dannyboy.csproj" /p:configuration=Release
if %ERRORLEVEL% neq 0 goto ERROR

.nuget\nuget.exe pack dannyboy.nuspec
if %ERRORLEVEL% neq 0 goto ERROR

echo *** Ready to upload to nuget.org ***
pause

for %%f in (*.nupkg) do (
	.nuget\nuget.exe push %%f
)

copy *.nupkg nupkg_archive\
del *.nupkg


echo *** SUCCESS ***
goto END


:ERROR
echo Ffffff    Aaa     Iiii  Ll                                                               
echo Ff       Aa Aa     Ii   Ll                                                               
echo Fffff   Aa   Aa    Ii   Ll                                                              
echo Ff      Aaaaaaa    Ii   Ll                                                               
echo Ff     Aa     Aa  Iiii  Llllll                                                           
echo .

:END