# deploy.ps1

$ErrorActionPreference = "Stop"

Write-Host "================================="
Write-Host "Building Docker Image"
Write-Host "================================="

docker build . -t fcg-userapi:latest

Write-Host "================================="
Write-Host "Namespace"
Write-Host "================================="

kubectl apply -f .\k8s\fgc-namespace.yml

Write-Host "================================="
Write-Host "Secrets"
Write-Host "================================="

kubectl apply -f .\k8s\secrets\sqlserver-pwd-secret.yml
kubectl apply -f .\k8s\secrets\userapi-constr-secret.yml
kubectl apply -f .\k8s\secrets\userapi-jwt-key-secret.yml
kubectl apply -f .\k8s\secrets\userapi-mq-uri-secret.yml
kubectl apply -f .\k8s\secrets\userapi-root-pwd-secret.yml

Write-Host "================================="
Write-Host "ConfigMaps"
Write-Host "================================="

kubectl apply -f .\k8s\config-maps\userapi-config-map.yml

Write-Host "================================="
Write-Host "Persistent Volumes"
Write-Host "================================="

kubectl apply -f .\k8s\volumes\userdb-pvc.yml

Write-Host "================================="
Write-Host "Services"
Write-Host "================================="

kubectl apply -f .\k8s\services\sqlserver-service.yml
kubectl apply -f .\k8s\services\userapi-service.yml

Write-Host "================================="
Write-Host "Deployments"
Write-Host "================================="

kubectl apply -f .\k8s\deployments\sqlserver-deployment.yml
kubectl apply -f .\k8s\deployments\userapi-deployment.yml

Write-Host "================================="
Write-Host "Status"
Write-Host "================================="

kubectl get deployments -n fgc
kubectl get pods -n fgc
kubectl get services -n fgc

Write-Host "Deploy concluído."