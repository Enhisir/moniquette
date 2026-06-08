# OpenWRT educational whitelist

This package installs a domain/IP whitelist for OpenWRT routers with firewall4/nftables.
It is intended for an isolated educational network where student devices may access only approved external resources.

## Install

Copy the directory to the router, then run:

```sh
cd /tmp/Moniquette.Router
sh install.sh
```

## Configure

Whitelist files on the router:

```sh
/etc/edu-whitelist/domains.txt
/etc/edu-whitelist/ips.txt
/etc/edu-whitelist/config
```

Typical config:

```sh
LAN_IF="br-lan"
WAN_IF="wan"
ENABLE_IPV6="0"
AUTO_UPDATE_HOUR="6"
REMOTE_DOMAINS_URL=""
```

## Commands

```sh
edu-whitelist add-domain example.edu
edu-whitelist remove-domain example.edu
edu-whitelist add-ip 203.0.113.10
edu-whitelist update
edu-whitelist status
edu-whitelist stop
edu-whitelist cron 6
edu-whitelist uninstall
```

## How it works

The script resolves domains from `/etc/edu-whitelist/domains.txt`, combines them with static IP/CIDR values from `/etc/edu-whitelist/ips.txt`, generates an nftables table named `inet edu_whitelist`, and attaches a forward hook with priority `-150`. Only traffic from `LAN_IF` to `WAN_IF` is restricted. Local LAN traffic and router input traffic are not changed.

By default IPv6 forwarding from LAN to WAN is dropped because otherwise students may bypass an IPv4-only whitelist. Enable IPv6 only after adding IPv6 addresses and testing the target network.