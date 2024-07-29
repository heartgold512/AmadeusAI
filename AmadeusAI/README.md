### Looking for maintainers

If anyone is interested in fixing some bugs or even extending AmadeusAI - contact me ill be very happy for improving upon this project together


(This application displays Amadeus at the bottom right/left of your primary screen, just above the taskbar if you have it there. she vanishes when you hover over her with your cursor, so she never interferes with any application that her window may cover (unless you change the code)

# Download/Using MonikAI

To give Monika a window to *your* desktop as well, visit the [Tab] (https://github.com/) and download the latest AmadeusAI.exe under //

Note: This program only works on Microsoft Windows, I have tested it on Windows 10.
I wouldnt mind working on some linux version though in the future as well as making the website and upgrading things in general
# Browser Extensions

By default, Amadeus will only react to applications being started and entertain you with some idle chatter. If you want her to react to you browsing the web, you need to install the correct extension for your browser:

* Firefox: [Mozilla Addons](https://addons.mozilla.org/en-US/firefox/addon/addonname/)
* Chrome: //we will see

# Contributing

Ill gladly take constributions so contact me let me know

### Bugs reporting

If you find a bug or want to request a feature, create an Issue right here on GitHub so I can see it 


### Behaviours

To add different things Amadeus can react to, you have to add behaviours. So far, Amadeus can react to applications being launched and web pages being loaded (by URL). To implement your own, add a new class in the `Behaviours` folder and make it implement IBehaviour. It will automatically be loaded. To make monika say something, simply use `window.Say(...)` in Update or Init.

### General improvements

Always appreciated, although I can't give you a tutorial on this, you'll have to try and understand the code yourself or use some form of tool such as bing, gpt etc to gide you through

**NOTE**: If you develop in Visual Studio, you have to run VS as administrator, as AmadeusAI WILL only LAUNCH when it itself is launched with admin credentials

# Art
The art assets have not been created by me and i have used stable diffusion as well as gimp from one origional image provided by this user:
for the images they have provided i am very thankful for the birth of this project

