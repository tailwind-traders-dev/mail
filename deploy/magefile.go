//go:build mage
// +build mage

package main

import (
	"errors"
	"fmt"
	"os"
	"time"

	"github.com/magefile/mage/mg"
	"github.com/magefile/mage/sh"
)

type Deploy mg.Namespace

// Test deployment to <name>
func (Deploy) Test(name string) error {
	fmt.Printf("Testing deployment to: %s\n", name)
	return nil
}

// ContainerApps deploys the Container App(s) via containerapp.bicep
// into the provided <resource group>
// Requires: AZURE_SERVICEBUS_CONNECTION_STRING
func (Deploy) ContainerApps(resourceGroup string) error {
	serviceBusConnection := os.Getenv("AZURE_SERVICEBUS_CONNECTION_STRING")
	if serviceBusConnection == "" {
		return errors.New("AZURE_SERVICEBUS_CONNECTION_STRING environment variable not found")
	}
	databaseURL := os.Getenv("APP_DATABASE_URL")
	if databaseURL == "" {
		return errors.New("APP_DATABASE_URL environment variable not found")
	}
	cmd1 := []string{
		"az",
		"deployment",
		"group",
		"create",
		"--resource-group",
		resourceGroup,
		"--template-file",
		"azure-container-apps/containerapp.bicep",
		"--parameters",
		"service_bus_connection=" + serviceBusConnection,
		"app_database_url=" + databaseURL,
	}
	return sh.RunV(cmd1[0], cmd1[1:]...)
}

// Data deploys 5 "Data" services via main.bicep into the
// provided <resource group>. The services are:
// Container Registry, Blob Storage, Service Bus, Key Vault and Postgres
func (Deploy) Data(resourceGroup string) error {
	cmd1 := []string{
		"az",
		"deployment",
		"group",
		"create",
		"--resource-group",
		resourceGroup,
		"--template-file",
		"azure-data/main.bicep",
		"--parameters",
		"deployPostgres=false",
	}
	return sh.RunV(cmd1[0], cmd1[1:]...)
}

// DataAndPostgres deploys Data and Azure Database for Postgres
func (Deploy) DataAndPostgres(resourceGroup string) error {
	cmd1 := []string{
		"az",
		"deployment",
		"group",
		"create",
		"--resource-group",
		resourceGroup,
		"--template-file",
		"azure-data/main.bicep",
		"--parameters",
		"deployPostgres=true",
	}
	return sh.RunV(cmd1[0], cmd1[1:]...)
}

// RBAC deploys Role Based Access Control using rbac.bicep
// with the principalID of the currently signed in user
func (Deploy) RBAC(resourceGroup string) error {
	cmd1 := []string{
		"az",
		"ad",
		"signed-in-user",
		"show",
		"--query",
		"id",
		"--out",
		"tsv",
	}
	principalID, err := sh.Output(cmd1[0], cmd1[1:]...)
	if err != nil {
		return err
	}

	cmd2 := []string{
		"az",
		"deployment",
		"group",
		"create",
		"--resource-group",
		resourceGroup,
		"--template-file",
		"azure-data/rbac.bicep",
		"--parameters",
		"principalID=" + principalID,
	}
	return sh.RunV(cmd2[0], cmd2[1:]...)
}

// Empty empties the <resource group> via empty.bicep
func (Deploy) Empty(resourceGroup string) error {
	fmt.Printf("Emptying Resource Group (%s) in 10 seconds.\n", resourceGroup)
	time.Sleep(time.Second * 10)

	cmd1 := []string{
		"az",
		"deployment",
		"group",
		"create",
		"--resource-group",
		resourceGroup,
		"--mode",
		"Complete",
		"--template-file",
		"azure-container-apps/empty.bicep",
	}
	return sh.RunV(cmd1[0], cmd1[1:]...)
}

// Group creates the <resource group> in <location>
func (Deploy) Group(name, location string) error {
	cmd1 := []string{
		"az",
		"group",
		"create",
		"--name",
		name,
		"--location",
		location,
	}
	return sh.RunV(cmd1[0], cmd1[1:]...)
}
