#!/bin/bash
# Azure Infrastructure Provisioning Script for OleAI
# Note: Since the user provisioned resources manually via portal, this is just for reference/future automation.

set -e

# Load initial variables
RESOURCE_GROUP="rg-oleai-prod"
LOCATION="westeurope"
SQL_SERVER="oleai-sql-server"
SQL_DB="OleAIDb"
ACR_NAME="oleaiacr"
VM_NAME="oleai-vm"

echo "Creating Resource Group..."
az group create --name $RESOURCE_GROUP --location $LOCATION

echo "Creating Container Registry..."
az acr create --resource-group $RESOURCE_GROUP --name $ACR_NAME --sku Basic --admin-enabled true

echo "Creating Azure SQL Server..."
# Note: You should pass the password securely in production
az sql server create --name $SQL_SERVER --resource-group $RESOURCE_GROUP --location $LOCATION --admin-user "oleaiadmin" --admin-password "YourStrongPassword123!"

echo "Creating Azure SQL Database..."
az sql db create --resource-group $RESOURCE_GROUP --server $SQL_SERVER --name $SQL_DB --service-objective Basic

echo "Creating Virtual Machine..."
az vm create \
  --resource-group $RESOURCE_GROUP \
  --name $VM_NAME \
  --image Ubuntu2204 \
  --size Standard_B4ms \
  --admin-username azureuser \
  --generate-ssh-keys \
  --public-ip-sku Standard \
  --custom-data ./cloud-init.yml

echo "Opening Ports..."
az vm open-port --resource-group $RESOURCE_GROUP --name $VM_NAME --port 80,443

echo "Provisioning complete!"
