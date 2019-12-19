# Startup Bash Script

The following command line commands are only performed once for each added device:
```bash
sudo apt-get install bluez
bluetoothctl
agent on
scan on
scan off #when device is found turn off scan
pair {macaddr} #mac address of found device
trust {macaddr} #mac address of found device
ctrl-c #exit bluetootctl
```

Create sh file for the first device:
```bash
#!/bin/bash
rfcomm connect hci0 {macaddr} #Connect to rfcomm0
```

Create sh files for all following devices (And increase channel number for each device)
```bash
#!/bin/bash
rfcomm connect 1 {macaddr} #Connect to rfcomm1
```


Open crontab: 
```bash
sudo crontab -e
```
and insert the following lines at the end of the document:

```bash
#Bluetooth demon takes approx 20 seconds to start
@reboot sleep 30 && /usr/bin/sudo {Path_to_device_1_sh_file}
#Await first connect before connecting second device
@reboot sleep 40 && /usr/bin/sudo {Path_to_device_2_sh_file}
#Increase sleep for each added device
```
