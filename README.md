# I.DOT SiLA2 Sample Repository

## Purpose
This repository contains Dispendix client samples to help I.DOT SiLA 2 API users developed to create a new custom SiLA 2 client.

## General Remarks
The connection to the I.DOT SiLA 2 server instrument should be implemented following the SiLA implementation guidelines. **The I.DOT SiLA 2 server only supports client-initiated connection method**. This means that the client should initiate the connection to the I.DOT SiLA 2 server. 
Any scheduler software, PMS (process management software), or LIMS (laboratory information management system) that complies with SiLA 2 standards can connect with the I.DOT SiLA 2 server. SiLA 2 runs over HTTP/2 connection. Therefore, this connection should use an internet or local DHCP connection with a router. The user has to connect  a client software that follows the SiLA 2 client implementations. As of 2021/02/17, the SiLA 2 consortium provides client implementations and SDKs in some popular programming languages such as:

•	Java

•	C#

•	Python

A SiLA 2 client can connect by entering an IP address or with SiLA server discovery. Server discovery is the easiest way for a client to access the I.DOT instrument. The server discovery feature uses multicast DNS messaging (mDNS) with the service name **_sila._tcp**.

## SiLA 2 Library
For more information about SiLA 2 library, please go to the [SiLA 2 official page](https://gitlab.com/SiLA2).
