using CoreWCF;
using Microsoft.AspNetCore.Hosting.Server;
using Newtonsoft.Json;
using SharpOnvifCommon;
using SharpOnvifServer;
using SharpOnvifServer.DeviceMgmt;
using Settings = ONVIFStream.Config.Models.Components.DeviceSettings;

namespace ONVIFStream
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class DeviceImpl : DeviceBase
    {
        private readonly IServer _server;
        private Settings? _settings;

        public DeviceImpl(IServer server)
        {
            _server = server;

            _settings = _settings = JsonConvert.DeserializeObject<Settings>(JSONReader.ReadJsonFile("device_settings.json"));

            if (_settings == null) throw new Exception("Can not deserialize device settings.");
        }

        public override GetCapabilitiesResponse GetCapabilities(GetCapabilitiesRequest request)
        {
#if DEBUG
            Console.WriteLine("GetCapabilities");
#endif
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
                                    Major = 17,
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
                            //???
                            RTP_TCP = true,
                            RTP_RTSP_TCP = true,
                            RTPMulticast = true,
                            RTP_RTSP_TCPSpecified = true,
                            RTPMulticastSpecified = true,
                            RTP_TCPSpecified = true
                            //
                        },
                    }                    
                }
            };
        }

        public override GetDeviceInformationResponse GetDeviceInformation(GetDeviceInformationRequest request)
        {
#if DEBUG
            Console.WriteLine("GetDeviceInformation");
#endif
            return _settings!.DeviceInformation;
        }

        public override DNSInformation GetDNS()
        {
#if DEBUG
            Console.WriteLine("GetDNS");
#endif
            return new DNSInformation()
            {
                FromDHCP = false,
                DNSManual = new IPAddress[]
                {
                    new IPAddress()
                    {
                        IPv4Address = "8.8.8.8" //?
                    }
                }
            };
        }

        public override GetNetworkInterfacesResponse GetNetworkInterfaces(GetNetworkInterfacesRequest request)
        {
#if DEBUG
            Console.WriteLine("GetNetworkInterfaces");
#endif
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
                    }
                }
            };
        }

        public override GetScopesResponse GetScopes(GetScopesRequest request)
        {
#if DEBUG
            Console.WriteLine("GetScopes");
#endif
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
                    },
                    new Scope()
                    {
                        ScopeDef = ScopeDefinition.Fixed,
                        ScopeItem = "onvif://www.onvif.org/Profile/S"
                    }
                }
            };
        }

        public override GetServicesResponse GetServices(GetServicesRequest request)
        {
#if DEBUG
            Console.WriteLine("GetServices");
#endif
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
#if DEBUG
            Console.WriteLine("GetSystemDateAndTime");
#endif
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