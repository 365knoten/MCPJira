$filePath = "publish" 


dotnet publish -c Release -o publish

Compress-Archive -Path "$($filePath)/*" -DestinationPath publish.zip -Force