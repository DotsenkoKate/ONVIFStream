using CoreWCF;
using Microsoft.AspNetCore.Hosting.Server;
using SharpOnvifServer.Media;
using Newtonsoft.Json;
using ONVIFStream.Config.Models;
using SharpOnvifServer.DeviceMgmt;
using IntRange = SharpOnvifServer.Media.IntRange;
using VideoResolution = SharpOnvifServer.Media.VideoResolution;

namespace ONVIFStream
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class MediaImpl : MediaBase
    {
        private readonly IServer _server;

        public MediaImpl(IServer server)
        {
            _server = server;
        }
        public override GetProfilesResponse GetProfiles(GetProfilesRequest request)
        {

            GetProfilesResponse res = JsonConvert.DeserializeObject<GetProfilesResponse>(ReadJsonFromRelativePath());
            var test = new GetProfilesResponse()
            {
                Profiles = res.Profiles
            };
            return test;
        }
        public override Profile GetProfile(string ProfileToken)
        {

            GetProfilesResponse res = JsonConvert.DeserializeObject<GetProfilesResponse>(ReadJsonFromRelativePath());
            var profiles = new List<Profile> {};
            foreach (var item in res.Profiles)
            {
                profiles.Add(item);
            }
            var profile = profiles.FirstOrDefault(p => p.token == ProfileToken);
            return profile;
        }

        public override MediaUri GetSnapshotUri(string ProfileToken)
        {

            Components.GetProfilesResponse res = JsonConvert.DeserializeObject<Components.GetProfilesResponse>(ReadJsonFromRelativePath());

            var profiles = new List<Components.Profile> { };
            foreach (var item in res.Profiles)
            {
                profiles.Add(item);
            }

            var profile = profiles.FirstOrDefault(p => p.token == ProfileToken);
            if (profile.VideoEncoderConfiguration.Encoding == "H264")
            {
                return new MediaUri()
                {
                    //Uri = $"{_server.GetHttpEndpoint()}/preview"
                    Uri = "http://localhost:7238/cameras/snapshot/1/pars"
                };
            }
            else if (profile.VideoEncoderConfiguration.Encoding == "JPEG")
            {
                return new MediaUri()
                {
                    //Uri = $"{_server.GetHttpEndpoint()}/preview"
                    Uri = "http://localhost:7238/cameras/snapshot/1/pars"
                };
            }
            else
                return new MediaUri()
                {
                    //Uri = $"{_server.GetHttpEndpoint()}/preview"
                    Uri = "http://localhost:7238/cameras/snapshot/1/pars"
                };

        }

            /*return new MediaUri()
            {
                //Uri = $"{_server.GetHttpEndpoint()}/preview"
                Uri = "http://localhost:7238/cameras/snapshot/1/pars"
            };
            }*/
        
        public override MediaUri GetStreamUri(StreamSetup StreamSetup, string ProfileToken)
        {

            Components.GetProfilesResponse res = JsonConvert.DeserializeObject<Components.GetProfilesResponse>(ReadJsonFromRelativePath());
            
            var profiles = new List<Components.Profile> { };
            foreach (var item in res.Profiles)
            {
                profiles.Add(item);
            }

            var profile = profiles.FirstOrDefault(p => p.token == ProfileToken);
            if (profile.VideoEncoderConfiguration.Encoding == "H264")
            {
                return new MediaUri()
                {
                    //Uri = $"{_server.GetHttpEndpoint()}/preview"
                    Uri = "http://10.10.2.204:5000/api/v1/complex/cameras/1/rtsp"
                };
            }
            else if (profile.VideoEncoderConfiguration.Encoding == "JPEG")
            {
                return new MediaUri()
                {
                    //Uri = $"{_server.GetHttpEndpoint()}/preview"
                    Uri = "http://10.10.2.204:5000/api/v1/complex/cameras/1/mjpeg"
                };
            }
            else 
                return new MediaUri()
            {
                //Uri = $"{_server.GetHttpEndpoint()}/preview"
                Uri = "http://10.10.2.204:5000/api/v1/complex/cameras/1/mp4"
            };

        }
        public override VideoSourceConfiguration GetVideoSourceConfiguration(string ConfigurationToken)
        {

            Components.GetProfilesResponse res = JsonConvert.DeserializeObject<Components.GetProfilesResponse>(ReadJsonFromRelativePath());

            var videoSourceConfigurations = new List<VideoSourceConfiguration>();

            // ���������� ������ ������� �� ������������������ ������
            foreach (var profile in res.Profiles)
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

            // ����� ������������ �� ������
            var configuration = videoSourceConfigurations.FirstOrDefault(c => c.token == ConfigurationToken);
            return configuration;
        }
        public override VideoEncoderConfigurationOptions GetVideoEncoderConfigurationOptions(string ConfigurationToken, string ProfileToken)
        {
            /*string filePath = "C://Users//Docenko//source//repos//ONVIFStream//ONVIFStream//Config//VideoEncoderConfigurationOptions.json";
            string json = File.ReadAllText(filePath);
            VideoEncoderConfigurationOptions res = JsonConvert.DeserializeObject<VideoEncoderConfigurationOptions>(json);
            return res;*/

              return new VideoEncoderConfigurationOptions()
         {

            QualityRange = new IntRange() // ���������� �������� ��������
             {
                 Min = 0,
                 Max = 10 // ������ ��������� �������� �� 0 �� 10
             },

             JPEG = new JpegOptions() // ����� ��� JPEG
             {
                 ResolutionsAvailable = new VideoResolution[]
                 {
                     new VideoResolution() { Width = 640, Height = 360 },
                     new VideoResolution() { Width = 1280, Height = 720 },
                     new VideoResolution() { Width = 1920, Height = 1080 }
                 },
                 FrameRateRange = new IntRange() // �������� ������� ������
                 {
                     Min = 1,
                     Max = 30
                 },
                 EncodingIntervalRange = new IntRange() // ��������� ����������� (��������, �������� �����)
                 {
                     Min = 1,
                     Max = 10
                 }
             },
             MPEG4 = new Mpeg4Options()
             {
                 ResolutionsAvailable = new VideoResolution[]
                         {
                             new VideoResolution() { Width = 640, Height = 360 },
                             new VideoResolution() { Width = 1280, Height = 720 },
                             new VideoResolution() { Width = 1920, Height = 1080 }
                         },
                 GovLengthRange = new IntRange() // �������� ����� GOV
                 {
                     Min = 1,
                     Max = 60
                 },
                 FrameRateRange = new IntRange() // �������� ������� ������
                 {
                     Min = 1,
                     Max = 30
                 },
                 EncodingIntervalRange = new IntRange() // �������� �����������
                 {
                     Min = 1,
                     Max = 4
                 }
             },
             H264 = new H264Options()
             {
                 ResolutionsAvailable = new VideoResolution[]
                 {
                     new VideoResolution() { Width = 640, Height = 360 },
                     new VideoResolution() { Width = 1280, Height = 720 },
                     new VideoResolution() { Width = 1920, Height = 1080 }
                 },
                 GovLengthRange = new IntRange() // �������� ����� GOV
                 {
                     Min = 1,
                     Max = 60
                 },
                 FrameRateRange = new IntRange() // �������� ������� ������
                 {
                     Min = 1,
                     Max = 30
                 },
                 EncodingIntervalRange = new IntRange() // �������� �����������
                 {
                     Min = 1,
                     Max = 4
                 },
                 H264ProfilesSupported = new H264Profile[]
                 {
                     H264Profile.Baseline,
                     H264Profile.Main,
                     H264Profile.High
                 }
             },
             GuaranteedFrameRateSupported = true,
             GuaranteedFrameRateSupportedSpecified = true,
         };


        }
        public override GetVideoSourcesResponse GetVideoSources(GetVideoSourcesRequest request)
        {

            Components.GetProfilesResponse res = JsonConvert.DeserializeObject<Components.GetProfilesResponse>(ReadJsonFromRelativePath());

            var videoSources = new List<SharpOnvifServer.Media.VideoSource>();

            // ���������� ������ ������� �� ������������������ ������
            foreach (var video in res.Profiles)
            {
                videoSources.Add(new SharpOnvifServer.Media.VideoSource()
                {
                    token = video.VideoSourceConfiguration.SourceToken,
                    Resolution = new SharpOnvifServer.Media.VideoResolution()
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
        public string ReadJsonFromRelativePath()
        {
            // �������� ����������, ��� ��������� ����������� ����
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // ���� � ����� ������������ ���������� ������
            string relativeFilePath = Path.Combine(basePath, "Config", "Profiles.json");

            // ���������, ���������� �� ����
            if (!File.Exists(relativeFilePath))
            {
                throw new FileNotFoundException($"���� �� ������: {relativeFilePath}");
            }

            // ������ ���������� ����� JSON
            string json = File.ReadAllText(relativeFilePath);

            return json;
        }
    }

}