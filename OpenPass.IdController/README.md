# OpenPass back-end

The back-end server provides a bunch of endpoints the front-end will interact with. It is basically in charge of:
* Generating the pseudonymous identifier (IFA token) in the unauthenticated workflow.
* Generating the UID2 (UID2 token) by calling the UID2 service in the authenticated workflow.
* Manage user preferences (e.g opt-out) trough transparency and consent portal.

## Requirements

* Dotnet Core SDK 2.1
* Dotnet Runtime 2.1

## Run Application

Run the following command in the current directory:

```
dotnet run
```

This should build then run the OpenPass backend. You can specify environment configurations with the `--configuration` option (e.g: `--configuration Development`).

In browser, go to the [Swagger API](http://localhost:1234/swagger/index.html).

**Note**:

The backend serves assets built in the [front-end](https://github.com/criteo/openpass/blob/main/OpenPass.IdController.UI/README.md) project. In a production environment, don't forget to first build the frontend and copy the `dist` folder locally:

```
cd ../OpenPass.IdController.UI/ && npm install && npm run build
cp -r ../OpenPass.IdController.UI/dist/ dist/
```

## API overview

Application is running on port 1234. A Swagger API is available: http://localhost:1234/swagger/index.html.

For your interest here the list of available API endpoints:
- **AuthenticatedController**: endpoints related to the workflow based on emails:
  - **GenerateOtp**: generate one time password and send email with verification code.
  - **ValidateOtp**: validate verification code sent by email and generated UID2 based on email. Response the UID2 token + the IFA token (stored in 1P cookie).
  - **GenerateEmailToken**: generate UID2 based on email retrieved from any kind of authentication service. Response the UID2 token + the IFA token (stored in 1P cookie).

- **UnAuthenticatedController**: endspoints related to the workflow based on central 1P cookie:
  - **CreateIfa**: get or create token for anonymous user. Response the IFA token.

- **OpenPassController**: endpoints related to the rendering of front-end files:
  - **Widget**: return path for widget/assets/widget.min.js
  - **Index**: return path for dist/index.html

- **PortalController**: endpoints related to the consent and transparency portal:
  - **OptOut**: clean all 1P cookies if exists (UID2 + IFA tokens) and create a dedicated opt-out cookies.
  - **OptIn**: remove opt-out cookie.

- **EventController**: endpoints used for monitoring purposes
  - **SaveEvent**: endpoint for monitoring purposes (only used for PoC measurement).

## Configuration

AppSettings:

- **Smtp settings**:
  - **Host**: host of smtp server
  - **Port**: port of smtp server
  - **EnableSsl**: enable SSL
  - **DisplayName**: displayed name of the sender
  - **Address**: email address of the sender
  - **UserName**: username for sending email
  - **Password**: password for sending email

- **Uid2Configuration**: configuration for external UID2 integration for Authenticated flow
  - **Endpoint**: UID2 service endpoint
  - **ApiKey**: UID2 service API key

- **MetricsOptions**: settings for default metrics
  - **DefaultContextLabel**: name for context
  - **Enabled**: enable or disable metrics

- **MetricEndpointsOptions**: settings to view metrics
  - **MetricsEndpointEnabled**: enable or disable option to view metrics from /metrics url
  - **MetricsTextEndpointEnabled**: enable or disable option to view metrics from /metrics-text url
  - **EnvironmentInfoEndpointEnabled**: enable or disabe option to view metrics from /env url

## Metrics

We use App Metrics to gather metrics. [App Metrics](https://www.app-metrics.io/) is an open-source and cross-platform .NET library used to record metrics within an application.
In Open-pass application we define basic [custom metrics types](https://www.app-metrics.io/getting-started/metric-types/) and use only [Counters](https://www.app-metrics.io/getting-started/metric-types/counters/) types for tracking metrics.

App Metrics reporters allows defined metrics to be flushed for reporting and visualization. The following [list](https://www.app-metrics.io/reporting/reporters/) of the reporters currently available.
