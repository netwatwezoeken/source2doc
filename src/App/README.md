`dotnet tool uninstall --global source2doc`

`dotnet tool install --global --add-source ./src/App/nupkg source2doc`

`source2doc -s /Users/joshendriks/src/nwwz/source2doc/test/MediatrCode -l ./test/MediatrCode/bin/Debug/net8.0/MediatR.Contracts.dll ./test/MediatrCode/bin/Debug/net8.0/MediatR.dll -e MediatR.INotification MediatR.IRequest -h MediatR.INotificationHandler MediatR.IRequestHandler`

`dotnet pack /p:Version=0.0.1-beta`