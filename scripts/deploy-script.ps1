# Define variables
$APP_NAME = "myapp"
$APP_DIR = "C:\inetpub\wwwroot\$APP_NAME"
$SERVICE_NAME = "$APP_NAME"

# Stop the running application (if any)
Write-Output "Stopping the application..."
Stop-Service -Name $SERVICE_NAME

# Copy the published application to the target directory
Write-Output "Deploying the application..."
Copy-Item -Path .\publish\* -Destination $APP_DIR -Recurse -Force

# Start the application
Write-Output "Starting the application..."
Start-Service -Name $SERVICE_NAME

Write-Output "Deployment completed successfully."