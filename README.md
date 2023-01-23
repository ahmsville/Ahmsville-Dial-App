# Ahmsville-Dial-App
Companion software for the Ahmsville Dial.

**Note:** The Ahmsville Dial software is in constant development, and as such, the **Release** installer gets updated quite frequently, I recommend checking back with this repository from time to time to see if there is a new release, or a reupload of the latest release. (Until auto update notification is implement)

New release installers will usually contain new features, bug fixes and optimizations, updated library and code for the Dials, or feature request from users.

# Software Setup

Step1 - Download and Install [Arduino IDE.](https://www.arduino.cc/en/software)

Step2 - Download and install the Release installer file for the [Ahmsville Dial App.](https://github.com/ahmsville/Ahmsville-Dial-App/releases)

## Setting up Arduino IDE for the Ahmsville Dial board

Step1 – Open up the Arduino ide and go to Tools >> Board >> Boards Manager.

![ASSEMBLY FINAL_x264_Moment](https://user-images.githubusercontent.com/44074914/143771229-740d84f1-e3ef-41bc-afd0-634cb5792c26.jpg)


Step2 – Search for SAMD and install the Arduino SAMD Boards package.

![ASSEMBLY FINAL_x264_Moment(2)](https://user-images.githubusercontent.com/44074914/143771279-1d1fef11-5224-48b8-b490-f2a8c8d5da2e.jpg)


## First time programming

**Launch Ahmsville Dial – this will launch the app in the background, you can then open the app from the notification area.**

![app instruction1](https://user-images.githubusercontent.com/44074914/143771324-55a406cb-7073-47c2-8895-31746129b198.PNG)![app instruction2](https://user-images.githubusercontent.com/44074914/143771335-a6d25e43-966b-40f7-8e0b-e542a1d56a04.PNG)


**Open the Manual operation tab.**

![app instruction3](https://user-images.githubusercontent.com/44074914/143771360-a2762e13-b87d-42c6-87a1-5bb4f20362bd.PNG)![app instruction4](https://user-images.githubusercontent.com/44074914/143771381-1d9b4143-0315-45e5-becb-2a9b3fbd738f.PNG)


**Select your dial variant and click on update library.**

![app instruction5](https://user-images.githubusercontent.com/44074914/143771393-40d77400-b197-4b6d-834e-3b6f166b53c2.PNG)


_This will automatically install all the required Arduino libraries, but you can also get the libraries from the links below._

![app instruction6](https://user-images.githubusercontent.com/44074914/143771430-b64d0a7d-f80a-461c-9c9e-17e45790701b.PNG)
![app instruction7](https://user-images.githubusercontent.com/44074914/143771457-e5897786-2bfe-4d1c-bb00-9cf615a5a3dd.PNG)

Ahmsville Dial v2 - [https://github.com/ahmsville/Ahmsville-Dial-V2](https://github.com/ahmsville/Ahmsville-Dial-V2)

Magnetic Rotary Encoder - [https://github.com/ahmsville/Magnetic\_rotary\_encoding](https://github.com/ahmsville/Magnetic_rotary_encoding)

Capacitive touch – [https://github.com/PaulStoffregen/CapacitiveSensor](https://github.com/PaulStoffregen/CapacitiveSensor)

Adv – Capacitive touch – [https://github.com/ahmsville/Advanced\_capacitive\_touch\_detection](https://github.com/ahmsville/Advanced_capacitive_touch_detection)

MacroKey – [https://github.com/ahmsville/MacroKey](https://github.com/ahmsville/MacroKey)

SpaceNavigator - [https://github.com/ahmsville/SpaceNavigator](https://github.com/ahmsville/SpaceNavigator)

FastLED - [https://github.com/FastLED/FastLED](https://github.com/FastLED/FastLED)

RF24- [https://github.com/nRF24/RF24](https://github.com/nRF24/RF24)

ADCRead - [https://github.com/ahmsville/ADCRead](https://github.com/ahmsville/ADCRead)

**Now click on ReProgram Dial, which will open the Arduino sketch for your chosen dial.**

![app instruction8](https://user-images.githubusercontent.com/44074914/143771483-c54c7c0e-0fce-4b6a-ad21-b5e12f9152eb.PNG)


**Select board as the Arduino Zero (Native USB Port) and select the COM-Port for the dial.**

![image](https://user-images.githubusercontent.com/44074914/143771592-5028a5b9-6f29-436a-8482-f1b8e1b29c1d.png)

**The final step is to upload the sketch to your dial.**

![app instruction9](https://user-images.githubusercontent.com/44074914/143771611-3fabacf3-4e86-4d0e-863d-9f2e8d4c5cfa.PNG)


**Once the upload is complete, you dial should turn on and automatically connect to the App.**

![app instruction10](https://user-images.githubusercontent.com/44074914/143771619-8dd0aefc-3962-4a65-8840-4be5de374568.PNG)

# SpaceNav Support

For now the Ahmsville Dial SpaceNav has dedicated addin for three modelling software’s: Fusion 360, Solidworks, and Blender.

Different addin/addon have been developed for the mentioned software’s, that allows the Dial to control the modelling environment in these software’s, **the dial does not do this by manipulating your keyboard or mouse,** instead it uses the addin code running alongside the modelling software, so it does not impede your use of the keyboard, mouse or any other control devices. 

## **Setting up the Dial for Use in Solidworks**

* Step 1 – You obviously need to have solidworks installed on your PC.
* Step 2 – You also need to have a working Ahmsville Dial V2 - SpaceNav or Absolute variant.
* Step 3 – install the latest release of the Ahmsville Dial companion app.
* Step 4 – Make sure you have a configuration for Solidworks named “SOLIDWORKS”, by default you will have this configuration in the app as I always bundle my personal dial’s configurations with the app.
* Step 5 – Open Solidworks and you should be able to manipulate the model using the SpaceNav with no fuss, to see a full list of what you can do with the dial in Solidworks, click on MK5 on the Absolute Variant and Longpress the capacitive touch on the SpaceNav Variant, you can also watch the video for a more detailed demonstration.

## **Setting up the Dial for Use in Fusion 360**

* Step 1 – You obviously need to have Fusion 360 installed on your PC.
* Step 2 – You also need to have a working Ahmsville Dial V2 - SpaceNav or Absolute variant.
* Step 3 – install the latest release of the Ahmsville Dial companion app.
* Step 4 – Make sure you have a configuration for Fusion 360 named “Fusion_360”, by default you will have this configuration in the app as I always bundle my personal dial’s configurations with the app.
* Step 5 – Open Fusion 360 and you should be able to manipulate the model using the SpaceNav with no fuss, to see a full list of what you can do with the dial in Fusion 360, click on MK5 on the Absolute Variant and Longpress the capacitive touch on the SpaceNav Variant, you can also watch the video for a more detailed demonstration.

## **Setting up the Dial for Use in Blender**

* Step 1 – You obviously need to have Blender installed on your PC.
* Step 2 – You also need to have a working Ahmsville Dial V2 - SpaceNav or Absolute variant.
* Step 3 – install the latest release of the Ahmsville Dial companion app.
* Step 4 – locate the python version bundled with your blender, it’s usually located in (C:\Program Files\Blender Foundation\Blender 2.93\2.93\python\bin).
* Step 5 – open command prompt and cd into that folder with a command similar to this;
  * _**cd C:\Program Files\Blender Foundation\Blender 2.93\2.93\python\bin**_.  Once you’re in, enter the following commands one after the other:

  * _**python.exe -m ensurepip**_
  * _**python.exe -m pip install pywin32**_
  * _**python.exe -m pip install pynput**_

 this will install the python modules required by the Ahmsville Dial addon.
* Step 6 – Open Blender and navigate to Edit >> Preferences >> Add-ons and then click on install, Locate the Blender addon bundled with the Ahmsville dial companion app which is located @ (C:\Program Files\Ahmsville Labs\Ahmsville Dial\BLENDER Addon), select the file named _**ahmsville_dial.py**_ and make sure to enable it as shown in the image.
![Captuggre](https://user-images.githubusercontent.com/44074914/214071323-328cffda-489b-4377-95ea-3b0e4def86ef.PNG)

* Step 7 – Make sure you have a configuration for Blender named “Blender”, by default you will have this configuration in the app as I always bundle my personal dial’s configurations with the app.
* Step 8 – Open Blender and you should be able to manipulate the model using the SpaceNav after pressing ctrl + w, unlike solidworks and fusion 360 the Ahmsville dial blender addon does not start with blender automatically, hence the convenient shortcut. To see a full list of what you can do with the dial in Blender, click on MK5 on the Absolute Variant and Longpress the capacitive touch on the SpaceNav Variant, you can also watch the video for a more detailed demonstration.

SpaceNav Setup Video - https://youtu.be/7mp8e4i2rcc





