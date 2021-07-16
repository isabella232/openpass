# OpenPass UI

This app gives the ability for our customers to authorize their users.
This app contains 3 projects:
- Main app - ./projects/open-pass
- Widgets - ./projects/widget  
- Widget launcher - ./projects/widget-launcher

Widget - it is a Web Component that is run on customer website. Widget can set/read cookie and launch the window with Main app.
There are few widgets in the app:
* Widget Identification - provides a consistent and unified way to manage advertising user consent and privacy settings inside a website and across a network of websites
* Widget Landing - provides a way to invite user to be authorized on the website

Widget and Main app can communicate through [PostMessages API](https://developer.mozilla.org/en-US/docs/Web/API/Window/postMessage).
Widget launcher - it is a script that insert widget into partner website.

### Widget Identification
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
To add the app to customer website, customer should include the Launcher Script 
`<script src="//openpass.criteo.com/open-pass/widget-app"></script>`and html tag 
`<wdgt-identification></wdgt-identification>` in any place on the page.

`wdgt-identification` tag attributes:
- session = "authenticated" | "unauthenticated";
- variant = "dialog" | "in-site" | "redirect";
- view = "native" | "modal" | "non-skippable-modal";
- provider = "publisher" | "advertiser";

`wdgt-identification` tag methods:
- getUserData() - returns `ifaToken`, `uid2Token` and a flag `isDeclined` that indicates user declined authorization. 
  Can be called as in the example below:
```
<wdgt-identification></wdgt-identification>
<script>
  const userData = document.querySelector('wdgt-identification').getUserData();
</script>
```

`wdgt-identification` tag events:
- "loaded" - the widget emit this event when widget is loaded and the method `.getUserData()` can be called;
- "updated" - the widget emits this event when user finished authorization flow. Also provides [CustomEvent](https://developer.mozilla.org/en-US/docs/Web/API/CustomEvent) that has tokens in the field "detail".

You can subscribe to the event just added an event listener: 
```
<wdgt-identification></wdgt-identification>
<script>
  document.querySelector('wdgt-identification').addEventListener('updated', (customEvent) => {
    const userData = customEvent.detail;
    // Do whatever you need;
  });
</script>
```

The Launcher Script is located in /projects/widget/assets/widget.js. This script only includes the necessary files for Web Component.
There are different scripts for each environment. They are replaced during building.

### Widget Landing
To add the app to customer website, customer should include the Launcher Script
`<script src="//openpass.criteo.com/open-pass/widget-app"></script>`and html tag
`<wdgt-landing></wdgt-landing>` in any place on the page.

`wdgt-landing` tag attributes:
- brandImageUrl - the URL to the website logo;
- brandColor - website theme primary color in HEX.

`wdgt-landing` tag events:
- "requestSignin" - is emitted when user clicked on "Access content and deals" button;
- "requestSignup" - is emitted when user clicked on "Not a member? Sign up" button.

Partners should handle the display of their login form by subscribing to one of the Widget Landing events. 
A few changes are required to the partner's login form:

* Add a checkbox to allow OpenPass to manage and store users data. This checkbox should have the mention "Access exclusive content and deals with OpenPass"
* Once the user has checked the box and logged in through the partner's system (or any SSO),
the partner should provide the returned user PII (email) and call `window.__OpenPass.signUserIn(userEmail)`.
* This redirects the user to the OpenPass website and stores his preferences on the OpenPass system.
The user is then redirected back to the partner's website.
![GIF Demo](./dev/ezgif-3-ecb22e106978.gif)

## Development server
Activate the flag: `chrome://flags/#allow-insecure-localhost`  
Run `npm run start` - it runs both apps in development mode. Widget will serve on port 4300 and Main app on port 4200.
Also, there are available option `:preprod` and `:backendless` to redirect the api calls.

To run the entire application locally, you have to serve your own app/website. Add the script `<script src="http://localhost:4300/assets/widget.js"></script>`
and the html tag `<wdgt-identification session="{session}" view="{view}" variant="{variant}">` to your website.

## Build

Run `npm run build` to build the app. The build artifacts will be stored in the `dist/` directory. Use the `--prod` flag for a production build.
This command also builds the artifacts for preprod.  
Structure of build artifacts:
```
dist
|widget - directory of widget app
  |assets 
    |widget.min.js - widget launcher
  |index.html
  |.....js
|preprod 
  |widget - directory of widget app for preprod environment
    |assets 
      |widget.min.js - widget launcher
    |index.html
    |.....js
|assets - assets of Main app
|index.html - assets of Main app
|...js - assets of Main app
```

The root url for dist folder is `/open-pass`. The server will respond with index.html of Main app.

## Running unit tests

Run `npm run test` to execute the unit tests via [Karma](https://karma-runner.github.io).

## Running end-to-end tests

Run `npm run cypress:run-tests` to execute the end-to-end tests via [Cypress](https://www.cypress.io/).

## Further help

To get more help on the Angular CLI use `ng help` or go check out the [Angular CLI Overview and Command Reference](https://angular.io/cli) page.
