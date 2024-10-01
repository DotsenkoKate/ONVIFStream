using CoreWCF;
using Microsoft.AspNetCore.Hosting.Server;
using SharpOnvifCommon;
using SharpOnvifServer;
using SharpOnvifServer.DeviceMgmt;

namespace ONVIFStream
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class DeviceImpl : DeviceBase
    {
        private readonly IServer _server;

        public DeviceImpl(IServer server)
        {
            _server = server;
        }

        public override GetCapabilitiesResponse GetCapabilities(GetCapabilitiesRequest request)
        {
            Console.WriteLine("GetCapabilities");
            return new GetCapabilitiesResponse()
            {
                Capabilities = new Capabilities()
                {
                    Device = new DeviceCapabilities()
                    {
                        XAddr = $"{_server.GetHttpEndpoint()}/onvif/device_service",
                        Network = new NetworkCapabilities1()
                        {
                            IPFilter = true,
                            ZeroConfiguration = true,
                            IPVersion6 = true,
                            DynDNS = true,
                        },
                        System = new SystemCapabilities1()
                        {
                            SystemLogging = true,
                            SupportedVersions = new OnvifVersion[]
                            {
                                new OnvifVersion()
                                {
                                    Major = 16,
                                    Minor = 12
                                }
                            }
                        },
                        IO = new IOCapabilities()
                        {
                            InputConnectors = 0,
                            RelayOutputs = 0
                        },
                        Security = new SecurityCapabilities1()
                        {
                            TLS12 = true,
                        }
                    },
                    Media = new MediaCapabilities()
                    {
                        XAddr = $"{_server.GetHttpEndpoint()}/onvif/media_service",
                        StreamingCapabilities = new RealTimeStreamingCapabilities()
                        {
                            RTP_TCP = true
                        },
                    },
                    Events = new EventCapabilities()
                    {
                        WSPullPointSupport = true,
                        XAddr = $"{_server.GetHttpEndpoint()}/onvif/events_service"
                    }
                }
            };
        }

        public override GetDeviceInformationResponse GetDeviceInformation(GetDeviceInformationRequest request)
        {
            Console.WriteLine("GetDeviceInformation");

            return new GetDeviceInformationResponse()
            {
                FirmwareVersion = "1.0",
                HardwareId = "2qw13ed12",
                Manufacturer = "Digital Region",
                Model = "NanoCamerav1",
                SerialNumber = "001"
            };
        }
        public override DNSInformation GetDNS()

        {
            Console.WriteLine("GetDNS");
            return new DNSInformation()
            {
                FromDHCP = false,
                DNSManual = new IPAddress[]
                {
                    new IPAddress()
                    {
                        IPv4Address = "8.8.8.8"
                    }
                }
            };
        }

        public override GetNetworkInterfacesResponse GetNetworkInterfaces(GetNetworkInterfacesRequest request)
        {
            Console.WriteLine("GetNetworkInterfaces");
            return new GetNetworkInterfacesResponse()
            {
                NetworkInterfaces = new NetworkInterface[]
                {
                    new NetworkInterface()
                    {
                        Enabled = true,
                        Info = new NetworkInterfaceInfo()
                        {
                            Name = "eth0"
                        }
                    },
                }
            };
        }

        public override GetScopesResponse GetScopes(GetScopesRequest request)
        {
            Console.WriteLine("GetScopes");

            return new GetScopesResponse()
            {
                Scopes = new Scope[]
                {
                    new Scope()
                    {
                        ScopeDef = ScopeDefinition.Fixed,
                        ScopeItem = "onvif://www.onvif.org/type/video_encoder"
                    },
                    new Scope()
                    {
                        ScopeDef = ScopeDefinition.Fixed,
                        ScopeItem = "onvif://www.onvif.org/Profile/Streaming"
                    },
                    new Scope()
                    {
                        ScopeDef = ScopeDefinition.Fixed,
                        ScopeItem = "onvif://www.onvif.org/Profile/G"
                    }
                }
            };
        }

        public override GetServicesResponse GetServices(GetServicesRequest request)
        {
            Console.WriteLine("GetServices");

            return new GetServicesResponse()
            {
                Service = new Service[]
                {
                    new Service()
                    {
                        Namespace = OnvifServices.DEVICE_MGMT,
                        XAddr = $"{_server.GetHttpEndpoint()}/onvif/device_service",
                        Version = new OnvifVersion()
                        {
                            Major = 17,
                            Minor = 12
                        },
                    },
                    new Service()
                    {
                        Namespace = OnvifServices.MEDIA,
                        XAddr = $"{_server.GetHttpEndpoint()}/onvif/media_service",
                        Version = new OnvifVersion()
                        {
                            Major = 17,
                            Minor = 12
                        }
                    }
                }
            };
        }
        public override SystemDateTime GetSystemDateAndTime()
        {
            Console.WriteLine("GetSystemDateAndTime");

            var now = System.DateTime.UtcNow;
            return new SystemDateTime()
            {
                UTCDateTime = new SharpOnvifServer.DeviceMgmt.DateTime()
                {
                    Date = new Date()
                    {
                        Day = now.Day,
                        Month = now.Month,
                        Year = now.Year,
                    },
                    Time = new Time()
                    {
                        Hour = now.Hour,
                        Minute = now.Minute,
                        Second = now.Second
                    }
                }
            };
        }
    }
}