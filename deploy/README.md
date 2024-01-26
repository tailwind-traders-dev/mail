# Mail Service Deployment

## Mage

Also used by `.github/workflows/deploy-azure-*.yaml` actions workflows.

```
$ mage
Targets:
  deploy:containerApps         deploys the Container App(s) via containerapp.bicep into the provided <resource group> Requires: AZURE_SERVICEBUS_CONNECTION_STRING
  deploy:empty                 empties the <resource group> via empty.bicep
  deploy:group                 creates the <resource group> in <location>
  deploy:rbac                  deploys Role Based Access Control using rbac.bicep with the principalID of the currently signed in user
  deploy:storage               deploys 5 "Storage" services via main.bicep into the provided <resource group>.
  deploy:storageAndPostgres    deploys Storage and Azure Database for Postgres
  deploy:test                  deployment to <name>
```