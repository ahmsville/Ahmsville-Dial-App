# Ahmsville-Dial-App
Companion software for the Ahmsville Dial.

# Software Setup

Step1 - Download and Install [Arduino IDE.](https://www.arduino.cc/en/software)

Step2 - Download and install the Release installer file for the [Ahmsville Dial App.](https://github.com/ahmsville/Ahmsville-Dial-App)

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


