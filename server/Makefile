DEPLOY="./Deployment/azure/zip.sh"
APP_SERVICE="./Deployment/Azure/app_service.sh"
TEST_DB="Host=localhost;Database=tailwind;Username=rob;"

test:
	dotnet test

zip:
	source $(DEPLOY)

app_service:
	source $(APP_SERVICE)

.phony: zip app_service test