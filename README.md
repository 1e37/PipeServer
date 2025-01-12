# PipeServer
 --unmaintained--<br>
This is a Multi-Process PipeServer. It is possible to spawn n times a pipes, base on the amount of configs avaliable in the List _configs.
The first pipe is reserved as main communication pipe to any frontend application or bot.
Based on StreamReader and StreamWriter we are able to let different programs communicate, which support the WIN32 API.
The pipe0_bridge_win32api.py is a demo client, which connects to the pipe0. Its ment to send commands to the PipeServer.
The PipeServer is then passing our arguments and start a worker which will run a scheduled Job until exit.

The worker will authenticate to TG services and based on the runmode, it will eather dump informations from groups, store it then it in a csv, or it can also use the csv to send messages to the groups or channels.
