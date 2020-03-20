variable "prefix" {
  type = string
}

variable "location" {
  type = string
}

variable "app_service_plan_tier" {
  type = string
}

variable "app_service_plan_size" {
  type = string
}

/* Service Health API Configuration */
variable "service_health_api_tenant_host" {
  type = string
}

variable "service_health_api_tenant_id" {
  type = string
}

variable "service_health_api_client_id" {
  type = string
}

variable "service_health_api_client_secret" {
  type = string
}

variable "service_health_api_cache_duration" {
  type = string
}

/* Company Configuration */
variable "company_configuration_company_name" {
  type = string
}

variable "company_configuration_support_email" {
  type = string
}

variable "company_configuration_support_phone" {
  type = string
}

/* AAD Auth Configuration */
variable "is_aad_auth_enabled" {
  type    = bool
  default = false
}

variable "aad_auth_client_id" {
  type = string
}