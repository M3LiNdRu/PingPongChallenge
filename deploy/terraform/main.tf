# Configure the Azure provider
terraform {
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = ">= 2.26"
    }
  }
}

provider "azurerm" {
  features {},
  subscription_id = "e567fbd3-7d60-4cc1-a17e-ec49ffcb6e5c"
}

resource "azurerm_resource_group" "rg" {
    name     = "ping-pong-challenge-rg"
    location = "westeurope"

    tags = {
        Environment = "pingpongchallenge"
        Team = "4Arreplegats"
    }
}
