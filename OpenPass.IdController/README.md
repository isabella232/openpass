# OpenPass Back-end

Backend-up provide endpoints without any authentication to working with.

## Swagger API

Application is running on port 1234. To launch Swagger API, go to http://localhost:1234/swagger/index.html url.

Controllers:

- AuthenticatedController - provides endpoints to generate, validate and login via SSO logic:
  - GenerateOtp - generate one time password and send email with verification code
  - ValidateOtp - validate verification code sent by email and send two tokens with response
  - GenerateEmailToken - SSO login for Facebook and Google and send two tokens with response
- EventController - provides endpoint to save events from UI
  - SaveEvent - endpoint for tracking purposes
- OpenPassController - provides endpoints to get physical path's to files
  - Widget - return path for widget/assets/widget.min.js
  - Index - return path for dist/index.html
- PortalController - provides Optin and Optout mechanism
  - OptOut - remove Ifa and Uid2 token. Set Optout cookie
  - OptIn - remove Optout cookie
- UnAuthenticatedController - provides endpoints for authenticated flow
  - CreateIfa - get or create token for anonymous user

## Configuration

AppSettings:

- Smtp settings:

  - Host: host of smtp server
  - Port: port of smtp server
  - EnableSsl: enable SSL
  - DisplayName: display name sent email from
  - Address: email address sent email from
  - UserName: username for sending email
  - Password: password for sending email

- Uid2Configuration - configuration for external integration for Authenticated flow
  - Endpoint - uid2 service endpoint
  - ApiKey - uid2 service API key

- MetricsOptions - settings for default metrics

  - DefaultContextLabel - name for context
  - Enabled - enable or disable metrics

- MetricEndpointsOptions - settings to view metrics
  - MetricsEndpointEnabled - enable or disable option to view metrics by /metrics url
  - MetricsTextEndpointEnabled - enable or disable to view metrics by /metrics-text url
  - EnvironmentInfoEndpointEnabled - enable or disabe to view metrics by /env url

## Metrics

We used App Metrics for gathering metrics. [App Metrics](https://www.app-metrics.io/) is an open-source and cross-platform .NET library used to record metrics within an application.
In Open-pass application we defined basic [custom metrics types](https://www.app-metrics.io/getting-started/metric-types/) and used only [Counters](https://www.app-metrics.io/getting-started/metric-types/counters/) types for tracking metrics.

App Metrics reporters allows defined metrics to be flushed for reporting and visualization. The following [list](https://www.app-metrics.io/reporting/reporters/) of the reporters currently available.

## Launch Application

Prerequisites:

- Add nuget.org as a source from command line:
  `dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org`

Clone project and go to a folder `OpenPass.IdController`. From command line, run the following command: `dotnet run`. Optional options: --configuration Development. In browser, go to the [Swagger API](http://localhost:1234/swagger/index.html).
