# Eventula Entrance Client

The Eventula Entrance Client is an additional peace of software for windows and linux that makes the checkin a little bit smoother on events that use [our fork of Eventula Event Manager](https://github.com/Lan2Play/eventula-manager).

## Features

- pin for using the application
- Scanning the QR Code Ticket
- Show if a ticket is already signed in
- Check payment status and mark tickets as paied if nessecary
- Sign in participants
- Keep track of covid tests and timers at the entrance (state is not saved in Eventula itself, but a negative test is nessecary for sign in if the corresponding feature is activated in the settings)
- Keep track of a 2G verification (state is not saved in Eventula itself, but a 2g verification is nessecary for sign in if the corresponding feature is activated in the settings)
- Keep track of onsite signed terms and conditions (state is not saved in Eventula itself, but checked terms are nessecary for sign in if the corresponding feature is activated in the settings)
- Custom background image
- Covid test, 2g verification, terms and conditions button can be enabled/disabled depending on your needs
- Covid test time can be changed (default 15 min)


## Screenshots

![Login Page](https://raw.githubusercontent.com/Lan2Play/eventula_entrance_client/main/Entrance_client_1.png)
![Main Page](https://raw.githubusercontent.com/Lan2Play/eventula_entrance_client/main/Entrance_client_2.png)
![Settings Page](https://raw.githubusercontent.com/Lan2Play/eventula_entrance_client/main/Entrance_client_3.png)

## Usage

- Download the [latest release](https://github.com/Lan2Play/eventula_entrance_client/releases/tag/v1.5.1)
- Login with your webbrowser to your Eventula Manager instance with an admin account and open the token wizzard (for example `https://YOURDOMAIN.TDL/account/tokens/wizzard/start/EventulaEntranceClient`) and create a token
- open the entrance client, unlock with the pin 4637 (currently hardcoded, sorry), go to settings and configure your Eventula api base address (for example `https://YOURDOMAIN.TDL` ), paste the generated Eventula token and save the settings
- you should be able to use the entrance client now, check out the other settings :)

## Development

The entrance client is built with dotnet 7 and electron. You should be able to build and run it just with the current [dotnet sdk](https://dotnet.microsoft.com/en-us/download). 
We recommend to use VSCode or Visual Studio for the development and debugging, both should just work out of the box.

## Future

- We like to add pin management, so there are 2 pins existing that can be changed and also one of them can not view or change the settings