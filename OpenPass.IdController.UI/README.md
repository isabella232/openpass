# OpenPass UI

This app gives the ability for our customers to authorize their users.
This app contains 3 projects:

- Main app - ./projects/open-pass
- Widget - ./projects/widget
- Widget launcher - ./projects/widget-launcher

Widget - it is a Web Component that is run on customer website. Widget can set/read cookie and launch the window with Main app.
Widget and Main app can communicate through [PostMessages API](https://developer.mozilla.org/en-US/docs/Web/API/Window/postMessage).
Widget launcher - it is a script that insert widget into partner website.

```
Customer website
+-----------------+               new browser window
|  <launcher/>    |               +-------------+
|      |          |               |             |
|   (inserts)     |               |             |
|      |          | post-messages |   Main app  |  rest
|  [<Widget/> ]<----------------->|             |<------->[Criteo API]
|                 |               +-------------+
+-----------------+
```

To add the app to customer website, customer should include script
`<script src="//openpass.criteo.com/open-pass/widget-app"></script>`and html tag
`<wdgt-identification></wdgt-identification>` in any place on the page.
`wdgt-identification` tag has next attributes:

- session = "authenticated" | "unauthenticated";
- variant = "dialog" | "in-site";
- view = "native" | "modal";

The script is located in /projects/widget/assets/widget.js. This script only includes the necessary files for Web Component.
There are different scripts for each environment. They are replaced during building.

This project was generated with [Angular CLI](https://github.com/angular/angular-cli) version 11.0.1.

## Development server

Activate the flag: `chrome://flags/#allow-insecure-localhost`
Run `npm install` - it install all required packages.
Run `npm run start` - it runs both apps in development mode. Widget will serve on port 4300 and Main app on port 4200.
The Url should be https://localhost:4200/api/open-pass
Also, there are available option `:backendless` to redirect the api calls.

To run the entire application locally, you have to serve your own app/website. Add the script `<script src="http://localhost:4300/assets/widget.js"></script>`
and the html tag `<wdgt-identification session="{session}" view="{view}" variant="{variant}">` to your website.

## Code scaffolding

Run `ng generate component component-name` to generate a new component. You can also use `ng generate directive|pipe|service|class|guard|interface|enum|module`.

## Build

Run `npm run build` to build the app. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.
Structure of build artifacts:

```
dist
|widget - directory of widget app
  |assets
    |widget.min.js - widget launcher
  |index.html
  |.....js
|assets - assets of Main app
|index.html - assets of Main app
|...js - assets of Main app
```

The root url for dist folder is `/open-pass`. The server will respond with index.html of Main app.

## TODO

[] implement loading iframe instead of Web Component

## Running unit tests

Run `npm run test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `ng e2e` to execute the end-to-end tests via [Protractor](http://www.protractortest.org/).

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.io/cli) page.
