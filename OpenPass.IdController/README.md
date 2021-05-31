# OpenPass back-end

The back-end server provides a bunch of endpoints the front-end will interact with. It is basically in charge of:
* Generating the pseudonymous identifier (IFA token) in case of the unauthenticated workflow.
* Generating the UID2 (UID2 token) calling the UID2 service in case of the authenticated workflow.
* Apply the users preferences (for instance opt-out) in case the users interact with the transparency and consent portal.

## Launch Application

Prerequisites:
* Have dotnet installed
* Have nuget.org registered as a source. If not:
  `dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org`

Clone project and go to a folder `OpenPass.IdController`.

From command line, run the following command: `dotnet run`.

You can specify the configuration you want to rely on using the option --configuration (for instance: Development).

In browser, go to the [Swagger API](http://localhost:1234/swagger/index.html).

## API overview

Application is running on port 1234. To have a look at the API through Swagger: http://localhost:1234/swagger/index.html.

For your interest here the list of available API endpoints:
- AuthenticatedController - endpoints related to the workflow based on emails:
  - GenerateOtp - generate one time password and send email with verification code.
  - ValidateOtp - validate verification code sent by email and generated UID2 based on email. Response the UID2 token + the IFA token (stored in 1P cookie).
  - GenerateEmailToken - generate UID2 based on email retrieved from any kind of authentication service. Response the UID2 token + the IFA token (stored in 1P cookie).
- UnAuthenticatedController - endspoints related to the workflow based on central 1P cookie:
  - CreateIfa - get or create token for anonymous user. Response the IFA token.
- OpenPassController - endpoints related to the rendering of front-end files:
  - Widget - return path for widget/assets/widget.min.js
  - Index - return path for dist/index.html
- PortalController - endpoints related to the consent and transparency portal:
  - OptOut - clean all 1P cookies if exists (UID2 + IFA tokens) and create a dedicated opt-out cookies.
  - OptIn - remove opt-out cookie.
- EventController - endpoints used for monitoring purposes
  - SaveEvent - endpoint for monitoring purposes (only used for PoC measurement).

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
  - Endpoint - UID2 service endpoint
  - ApiKey - UID2 service API key

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
