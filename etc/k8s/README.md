# Kubernetes

This folder contains some configuration to perform some actions with Kubernetes.

## Using the Kubernetes Dashboard

In this section, we are simply explain how to setup and open the Kubernetes Dashboard UI.

> See https://kubernetes.io/docs/tasks/access-application-cluster/web-ui-dashboard/ to learn more about the Kubernetes Dashboard UI.

### Install the Dashboard

Run the following command to install the Kubernetes dashboard to your cluster:

```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/dashboard/v2.7.0/aio/deploy/recommended.yaml
```

### Create User

Run the following command to create a user:

````bash
kubectl apply -f dashboard-adminuser.yaml
````

Run the following command to bind a role for that user:

````bash
kubectl apply -f dashboard-rolebinding.yaml
````

> If you have trouble, check https://github.com/kubernetes/dashboard/blob/master/docs/user/access-control/creating-sample-user.md please.

### Generate a Token

Generate a bearer token that will be used to login to the dashboard:

````bash
kubectl -n kubernetes-dashboard create token admin-user
````

Copy the token, we will use it to login to the application.

### Run Proxy

Running the K8s proxy is needed to access the UI (keep it running):

````bash
kubectl proxy
````

### Open the Dashboard UI

Visit the following page in your browser:

http://localhost:8001/api/v1/namespaces/kubernetes-dashboard/services/https:kubernetes-dashboard:/proxy/#/workloads?namespace=microappstudio01

Use the previously generated bearer token to login.