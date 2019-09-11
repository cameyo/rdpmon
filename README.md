# RdpMon - Server-side RDP monitoring tool

## Overview
A monitoring tool for RDS servers that shows real-time and past RDP connections along with source IP, success and failure counts, logins, active and past session, executed processes and more.

## RDP security and brute-force attacks
RDP is a fantastic technology, yet it brings some security challenges along, especially when it comes to cloud machines that are directly connected to the Internet. From our own observations once a cloud-connected machine with port 3389 is discovered by several bots, it undergoes **brute-force attacks** that amount to 100K - 200K password attempts per week. And in most cases it is difficult to even know about it. Also, security vulnerabilities that are discovered from time to time such as [BlueKeep](https://en.wikipedia.org/wiki/BlueKeep) can make this even more challenging.
Most RDP tools are designed to manage the Windows aspect of it such as users, quotas etc. But there is very little when it comes to cloud-oriented RDP security and management. RdpMon addresses the need of cloud-oriented RDP monitoring.

## Usage
The first time you run RdpMon, it installs itself as a service named "RDP Monitor". The service part constantly logs in the background RDP activity targeted at the machine it runs on, even when you are logged off. The GUI part lets you view the logged activity as well as real-time connection and session events as they occur. Both parts are contained within the same executable: RdpMon.exe.

### Connections
Under the Connections tab you can see RDP connections and connection attempts, grouped by IPs. IPs are marked by different colors: green=legitimate connections, red=high-intensity failed connections (likely brute-force attacks), yellow=low-intensity failed connections.
![RdpMon connections](https://files.cameyo.com/resources/rdpmon-connects-1.png)

At the bottom, a status bar shows the overall counts:

![RdpMon status](https://files.cameyo.com/resources/rdpmon-connects-statusbar.png)

### Sessions
Under the Sessions tab you can view both past and current RDP sessions. Clicking on a session in this list displays the processes that were / are used during this session. Live sessions are marked by a green bullet. Right-clicking on a live session allows shadowing it (=viewing the session in real time).
![RdpMon sessions](https://files.cameyo.com/resources/rdpmon-sessions-1.png)

This project uses LiteDB for data storage.

## License

[MIT](http://opensource.org/licenses/MIT)

Copyright (c) 2019 - Cameyo Inc, by Eyal Dotan

