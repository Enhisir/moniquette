export type NetworkConnection = {
  name: string;
  description: string;
  interfaceType: string;
  ipAddressString?: string | null;
  macAddressString?: string | null;
  domainNameServices: string[];
  gateways: string[];
};