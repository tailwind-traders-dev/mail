DEPLOY="./Deployment/azure/zip.sh"
APP_SERVICE="./Deployment/Azure/app_service.sh"
TEST_DB="Host=localhost;Database=tailwind;Username=rob;"


test: db seed
	dotnet test Tailwind.Mail.csproj

db:
	psql tailwind < ../db/db.sql --quiet

seed:
	psql tailwind < ../db/seed.sql --quiet

zip:
	source $(DEPLOY)

app_service:
	source $(APP_SERVICE)

.phony: zip app_service test

#pg_dump -d tailwind --no-owner --no-privileges --no-password --table "mail.contacts" --inserts -f ./dump.sql
#have to find/replace using ^\d*.\t