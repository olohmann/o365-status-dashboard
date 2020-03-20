locals {
  prefix      = lower(var.prefix)
  hash_suffix = lower(substr(sha256(local.prefix), 0, 4))
}

resource "azurerm_resource_group" "rg" {
  name     = "${var.prefix}_${local.hash_suffix}_rg"
  location = var.location
}

resource "azurerm_container_registry" "acr" {
  name                = "${local.prefix}${local.hash_suffix}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Standard"
  admin_enabled       = true
}

resource "azurerm_application_insights" "app_insights" {
  name                = "${local.prefix}-ai"
  resource_group_name = azurerm_resource_group.rg.name
  location            = var.location
  application_type    = "web"

  tags = map(
    "hidden-link:${azurerm_resource_group.rg.id}/providers/Microsoft.Web/sites/${local.prefix}-${local.hash_suffix}",
    "Resource"
  )
}

resource "azurerm_app_service_plan" "sp" {
  name                = "${local.prefix}-asp"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  kind                = "Linux"

  sku {
    tier = var.app_service_plan_tier
    size = var.app_service_plan_size
  }

  reserved = true # Mandatory for Linux plans
}

resource "azurerm_app_service" "app" {
  name                = "${local.prefix}-${local.hash_suffix}"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  app_service_plan_id = azurerm_app_service_plan.sp.id
  https_only          = true

  app_settings = {
    ApplicationInsights__InstrumentationKey = azurerm_application_insights.app_insights.instrumentation_key

    ServiceHealthApiConfiguration__TenantHost             = var.service_health_api_tenant_host
    ServiceHealthApiConfiguration__TenantId               = var.service_health_api_tenant_id
    ServiceHealthApiConfiguration__ClientId               = var.service_health_api_client_id
    ServiceHealthApiConfiguration__ClientSecret           = var.service_health_api_client_secret
    ServiceHealthApiConfiguration__CacheDurationInSeconds = var.service_health_api_cache_duration

    CompanyConfiguration__CompanyName  = var.company_configuration_company_name
    CompanyConfiguration__SupportEmail = var.company_configuration_support_email
    CompanyConfiguration__SupportPhone = var.company_configuration_support_phone

    DOCKER_REGISTRY_SERVER_URL      = azurerm_container_registry.acr.login_server
    DOCKER_REGISTRY_SERVER_USERNAME = azurerm_container_registry.acr.admin_username
    DOCKER_REGISTRY_SERVER_PASSWORD = azurerm_container_registry.acr.admin_password

    WEBSITES_PORT                       = 8080
    WEBSITES_ENABLE_APP_SERVICE_STORAGE = false

  }

  site_config {
    linux_fx_version = "DOCKER|${azurerm_container_registry.acr.login_server}/o365-status-dashboard:1.0.0"
    always_on        = true
    http2_enabled    = true
    ftps_state       = "Disabled"
  }

  auth_settings {
    enabled = var.is_aad_auth_enabled
    unauthenticated_client_action = "RedirectToLoginPage"
    
    active_directory {
      client_id = var.aad_auth_client_id
      
    }
  }

  // Ignore changes to the actual hosted docker image.
  lifecycle {
    ignore_changes = [site_config[0].linux_fx_version]
  }
}
