# I.DOT SiLA2 Sample Repository

## Purpose

This repository contains Dispendix client samples to help I.DOT SiLA 2 API users create their own SiLA 2 clients.

## General Remarks

The connection to the I.DOT SiLA 2 server instrument should be implemented following the SiLA implementation guidelines. **The I.DOT SiLA 2 server only supports client-initiated connection methods**.
Any scheduler software, PMS (process management software), or LIMS (laboratory information management system) that complies with SiLA 2 standards can connect with the I.DOT SiLA 2 server.

SiLA 2 runs over HTTP/2. Therefore, an internet connection or local DHCP with a router is required. The connecting client software also needs to follow the SiLA 2 specification. As of 2022/09/27, the SiLA 2 consortium provides reference implementations and SDKs in these programming languages:

- C#
- Java
- Javascript
- Python
- C++

A SiLA 2 client can connect either by providing an IP address or with SiLA server discovery. Server discovery is the easiest way for a client to access the I.DOT instrument. The server discovery feature uses multicast DNS messaging (mDNS) with the service name **\_sila.\_tcp**.

## SiLA 2 Library

For more information about SiLA 2 library, please go to the [SiLA 2 official page](https://gitlab.com/SiLA2).
