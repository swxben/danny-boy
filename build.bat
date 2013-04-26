@echo off
call "%VS100COMNTOOLS%vsvars32.bat"
mkdir log
mkdir lib\net40
mkdir nupkg_archive

msbuild.exe /ToolsVersion:4.0 "src\dannyboy\dannyboy.csproj" /p:configuration=Release
.nuget\nuget.exe pack dannyboy.nuspec

echo *** Ready to upload to nuget.org ***
pause

for %%f in (*.nupkg) do (
	.nuget\nuget.exe push %%f
)

copy *.nupkg nupkg_archive\
del *.nupkg

pause