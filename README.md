# Ahmsville-Dial-App
Companion software for the Ahmsville Dial.

# Software Setup

Step1 - Download and Install [Arduino IDE.](https://www.arduino.cc/en/software)

Step2 - Download and install the Release installer file for the [Ahmsville Dial App.](https://github.com/ahmsville/Ahmsville-Dial-App)

## Setting up Arduino IDE for the Ahmsville Dial board

Step1 – Open up the Arduino ide and go to Tools \&gt;\&gt; Board \&gt;\&gt; Boards Manager.

![](RackMultipart20211128-4-3zxmhk_html_13da1a10646df483.jpg)

Step2 – Search for SAMD and install the Arduino SAMD Boards package. ![](RackMultipart20211128-4-3zxmhk_html_813fb0b59af30dc5.jpg)

## First time programming

**Launch Ahmsville Dial – this will launch the app in the background, you can then open the app from the notification area.**

![](RackMultipart20211128-4-3zxmhk_html_55980e5ccdd0ca8a.png) ![](RackMultipart20211128-4-3zxmhk_html_3d4899f86a746d4d.png)

**Open the Manual operation tab.**

![](RackMultipart20211128-4-3zxmhk_html_9d9c8ddfa65b40ba.png)

![](RackMultipart20211128-4-3zxmhk_html_4d90c059daf9196e.png)

**Select your dial variant and click on update library.**

![](RackMultipart20211128-4-3zxmhk_html_49574e5bd679e557.png)

_This will automatically install all the required Arduino libraries, but you can also get the libraries from the links below._

![](RackMultipart20211128-4-3zxmhk_html_1a04e7393cd032bd.png)

![](RackMultipart20211128-4-3zxmhk_html_c4dd6811c3226a06.png)

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

![](RackMultipart20211128-4-3zxmhk_html_a36a18c523f569fc.png)

![](RackMultipart20211128-4-3zxmhk_html_3deeca3c89f24e48.png)

**Select board as the Arduino Zero (Native USB Port) and select the COM-Port for the dial.**

![Shape1](RackMultipart20211128-4-3zxmhk_html_1a9b3b0b70d72c9d.gif) ![](RackMultipart20211128-4-3zxmhk_html_cb076188476a55eb.png)

**The final step is to upload the sketch to your dial.**![](RackMultipart20211128-4-3zxmhk_html_a76256a1fa72481d.png)

**Once the upload is complete, you dial should turn on and automatically connect to the App.**

![](RackMultipart20211128-4-3zxmhk_html_609663811c8a320c.png)
