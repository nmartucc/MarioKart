Mario Kart Setup

Description of scripts:

NetMan1.cs - Network manager that handles connection between Unity instances on PC and Project Tango. IP Address variable should be set to the IP Address of the Project Tango. Should be attached to the Transport game object.

PortMan.cs - Manages the serial port with Arduino. Make sure this script and Arduino are using the same port/speed. Should be attached to the Transport game object.

KartController.cs - Handles collisions with item boxes and off road colliders. Controls item usage and uses PortMan to send the maximum allowed speed to Arduino. Should be attached to the kart game object.

OVRPlayerController2.cs - Manages the OVRCameraRig for Oculus Rift. Should be attached to the OVRPlayerController game object.

MYOVRDebugInfo.cs - Receives sensor data from Arduino using Portman and displays a warning HUD if data is below threshold. Should be attached to the OVRPlayerController game object.

Instructions for running Mario Kart:
1. Make sure all batteries are charged and that the Oculus Rift, Arduino, and Network Adapter are all plugged in and functioning.

2. Create a wireless hotspot for the Project Tango to connect to by running Command Prompt as Administrator and typing: netsh wlan start hostednetwork
If the hostednetwork does not exist, first create one and then start it: 
netsh wlan set hostednetwork mode=allow ssid=[Hotspotname] key=[password] (Ex: Hotspotname = hotspot, key = 11111111)

3. Connect the Project Tango to the hotspot using static IP addresses for the PC and the tablet. Run TangoMario on the tablet.

4. Open the MarioTrack scene in Unity and run.