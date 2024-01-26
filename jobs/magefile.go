//go:build mage
// +build mage

package main

import (
	"fmt"
	"jobs/senders"
	"time"

	"github.com/magefile/mage/mg"
	"github.com/magefile/mage/sh"
)

type Test mg.Namespace

// Hello is our default mage target which we also call
// by default within our Docker container
func (Test) Hello() error {
	now := time.Now().UTC().Format(time.RFC3339)
	fmt.Printf("hello %s\n", now)
	return nil
}

// Goodbye is an alternative mage target we can call
func (Test) Goodbye() error {
	now := time.Now().UTC().Format(time.RFC3339)
	fmt.Printf("goodbye %s\n", now)
	return nil
}

type Email mg.Namespace

// SendOne sends one test email to <to> via Azure Communications Services
func (Email) SendOne(to string) error {
	s, err := senders.NewAzureContainerServicesFromEnv()
	if err != nil {
		return err
	}
	return s.SendOne(to)
}

// GetResult gets the result of <id> from Azure Communication Services
func (Email) GetResult(operationID string) error {
	s, err := senders.NewAzureContainerServicesFromEnv()
	if err != nil {
		return err
	}
	return s.GetResult(operationID)
}

type Docker mg.Namespace

// Build builds the container image, "jobs", with --no-cache
// and Dockerfile which builds a static binary and
// multi-stage builds to utilize a distroless image
func (Docker) Build() error {
	cmd1 := []string{
		"docker",
		"build",
		"--no-cache",
		"-t",
		"jobs",
		".",
	}
	return sh.RunV(cmd1[0], cmd1[1:]...)
}

// BuildDev builds the container image, "jobs", with --no-cache
// and dev.Dockerfile which uses the golang:latest image,
// installs mage and vim, for more interactive development
func (Docker) BuildDev() error {
	cmd1 := []string{
		"docker",
		"build",
		"--no-cache",
		"-f",
		"dev.Dockerfile",
		"-t",
		"jobs",
		".",
	}
	return sh.RunV(cmd1[0], cmd1[1:]...)
}

// Run runs the jobs container with the mage target
func (Docker) Run(target string) error {
	cmd1 := []string{
		"docker",
		"run",
		"-it",
		"jobs",
		"mage",
		target,
	}
	return sh.RunV(cmd1[0], cmd1[1:]...)
}
