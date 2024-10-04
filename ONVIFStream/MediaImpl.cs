using CoreWCF;
using Microsoft.AspNetCore.Hosting.Server;
using SharpOnvifServer.Media;
using Newtonsoft.Json;
using Settings = ONVIFStream.Config.Models.Components.Settings;

namespace ONVIFStream
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class MediaImpl : MediaBase
    {
        private readonly IServer _server;
        private Settings? _settings;

        public MediaImpl(IServer server)
        {
            _server = server;

            _settings = JsonConvert.DeserializeObject<Settings>(ReadSettingsFromJson());

            if (_settings == null) throw new Exception("Json deserializing was failed.");
            else if (_settings.Profiles.Length == 0) throw new Exception("No profiles.");
            else if (_settings.VideoEncoderConfigurationOptions is null) throw new Exception("VideoEncoderConfigurationOptions is null.");
        }

        public override GetProfilesResponse GetProfiles(GetProfilesRequest request)
        {
            var response = new GetProfilesResponse()
            {
                Profiles = _settings!.Profiles
            };
            return response;
        }

        public override Profile GetProfile(string ProfileToken)
        {
            var profiles = _settings!.Profiles.ToList();

            var profile = profiles.FirstOrDefault(p => p.token == ProfileToken);

            if (profile == null) Console.WriteLine($"Profile with token == {ProfileToken} was not found.");

            return profile;
        }

        public override MediaUri GetSnapshotUri(string ProfileToken)
        {
            return new MediaUri() { Uri = _settings!.Links.Snapshot };
        }
        
        public override MediaUri GetStreamUri(StreamSetup StreamSetup, string ProfileToken)
        {
            if (StreamSetup == null) throw new ArgumentNullException("Stream setup is null.");

            string link;

            switch (StreamSetup.Transport.Protocol) 
            {
                case TransportProtocol.RTSP:
                    {
                        link = _settings!.Links.StreamingRTSP;
                        break;
                    }
                case TransportProtocol.HTTP:
                    {
                        link = _settings!.Links.StreamingMJPEG;
                        break;
                    }
                default:
                    {
                        link = string.Empty; 
                        break;
                    }
            }

            return new MediaUri() { Uri = link };
        }

        public override VideoSourceConfiguration GetVideoSourceConfiguration(string ConfigurationToken)
        {
            var videoSourceConfigurations = new List<VideoSourceConfiguration>();

            foreach (var profile in _settings!.Profiles)
            {
                videoSourceConfigurations.Add(new VideoSourceConfiguration()
                {
                    token = profile.VideoSourceConfiguration.token,
                    SourceToken = profile.VideoSourceConfiguration.SourceToken,
                    Bounds = new IntRectangle()
                    {
                        width = profile.VideoSourceConfiguration.Bounds.width,
                        height = profile.VideoSourceConfiguration.Bounds.height
                    }
                });
            }

            var configuration = videoSourceConfigurations.FirstOrDefault(c => c.token == ConfigurationToken);

            if (configuration == null) Console.WriteLine($"VideoSourceConfiguration with token == {ConfigurationToken} was not found.");

            return configuration;
        }

        public override VideoEncoderConfigurationOptions GetVideoEncoderConfigurationOptions(string ConfigurationToken, string ProfileToken)
        {
            return _settings!.VideoEncoderConfigurationOptions;
        }

        public override GetVideoSourcesResponse GetVideoSources(GetVideoSourcesRequest request)
        {
            var videoSources = new List<VideoSource>();

            foreach (var video in _settings!.Profiles)
            {
                videoSources.Add(new VideoSource()
                {
                    token = video.VideoSourceConfiguration.SourceToken,
                    Resolution = new VideoResolution()
                    {                        
                        Width = video.VideoSourceConfiguration.Bounds.width,
                        Height = video.VideoSourceConfiguration.Bounds.height
                    }
                });
            }

            return new GetVideoSourcesResponse()
            {
                VideoSources = videoSources.ToArray()
            };
        }

        private string ReadSettingsFromJson()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            string relativeFilePath = Path.Combine(basePath, "Config", "media_settings.json");

            if (!File.Exists(relativeFilePath))
            {
                throw new FileNotFoundException($"Файл не найден: {relativeFilePath}");
            }

            string json = File.ReadAllText(relativeFilePath);

            return json;
        }
    }

}