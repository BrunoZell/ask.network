# Platform Services

This lists all deployable services that execute within a platform instance. All these services are schedules via Kubernetes objects like _Pod_, _Deployment_, or _StatefulSet_ and may be paired with a custom Kubernetes _Controller_ that manages them.

Admins and users of the platform interact with a platform instance via its management API. Conceptually this is very similar to how Kubernetes clusters are controlled via the Kube API.

## Management API [1]

Publically hosted REST API to control platform state.

## Context Daemon [1]

Eagerly builds the latest platform context based on all (whitelisted) `IObserver`-Instaces currently running.

## Live Strategy Daemon [1]

Runs the specified strategy in the form of `Sdk.Decide` for live execution.

## Observer Daemon [n] + Controller

Runs arbitrary `IObserver`-Instances.

## Broker Daemon [n] + Controller

Runs arbitrary `IBroker`-Instances.
