#!/bin/sh
set -eu
SCRIPT_DIR="$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)"
if [ "$(id -u)" != "0" ]; then
  echo "Run as root on OpenWrt" >&2
  exit 1
fi
install -m 0755 "$SCRIPT_DIR/router/edu-whitelist" /usr/bin/edu-whitelist
/usr/bin/edu-whitelist install
