@ECHO OFF

ECHO Removing old nupkg
del /S *.nupkg

ECHO Packing the NuGet release files
..\.nuget\NuGet.exe Pack Lecoati.LeBlender.nuspec -Prop Configuration=Release

ECHO Publish NuGet
..\.nuget\NuGet.exe Push Lecoati.LeBlender*.nupkg

PAUSE
