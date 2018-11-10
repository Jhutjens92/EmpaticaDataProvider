# Empatica Data Provider
The EmpaticaDataProvider is an application designed to automatically connect to a specific datastream for an Empatica E4 watch. 
It get's all the data by connecting to the Empatica BLE server, sets up a BLE connection with the Empatica E4 and connects the device.

-- WARNING --
Currently the app is still under development and the only datastream currently available is the "acc" datastream.
 

### Prerequisites
- Download the Learning Hub: https://github.com/janschneiderou/LearningHub
- Installed the Empatica BLE Server: http://developer.empatica.com/windows-ble-server.html
- Empatica E4 watch

## Getting Started
The datastream is currently limited only to the acc datastream (Accelerometer). This is due to constrains on the Learning Hub side.
The EmpaticaDataProvider will automatically start the Empatica BLE Server with the parameters which need to be provided.

You can test the EmpaticaDataProvider without using the Learning Hub by just starting the executable and press "Start Recording"
Currently it does not provide any logging when you run it seperatly. It only shows the received values in the textboxes.

If you want to use it with the Learning Hub combined then make you sure have the Learning Hub set up accordingly. 
A complete how to guide can be found here: https://docs.google.com/document/d/1FbTd6wjqa9P_6O51gjZRU2ubiCA94nMZr001NkgBZ5s/edit#

## Startup Parameters (Learning Hub/CMD)
-sip is used to define the server IP on which you want the Empatica BLE Server to run.
-sp is used to define the server port on which you want the Empatica BLE Server to run.
-ak	is used to provide the Api Key which is need to start the application with a watch. Please check the Empatica website on how to receive your API-key for your registered watch.

The default values for the Server IP and Port are 127.0.0.1:5555.
the API-Key is required as a parameter, else the application won't work.

## Startup Parameters Examples
-sip 127.0.0.1 
-sp 5555
-ak a389709e458C4155985821a5ac90c893 

## Authors
* **Jordi Hutjens** -(https://github.com/jhutjens92)