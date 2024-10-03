using CoreWCF;
using Microsoft.AspNetCore.Hosting.Server;
using SharpOnvifServer.Media;
using Newtonsoft.Json;
using ONVIFStream.Config.Models;
using SharpOnvifServer.DeviceMgmt;
using IntRange = SharpOnvifServer.Media.IntRange;
using VideoResolution = SharpOnvifServer.Media.VideoResolution;
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
            /*string filePath = "C://Users//Docenko//source//repos//ONVIFStream//ONVIFStream//Config//VideoEncoderConfigurationOptions.json";
            string json = File.ReadAllText(filePath);
            VideoEncoderConfigurationOptions res = JsonConvert.DeserializeObject<VideoEncoderConfigurationOptions>(json);
            return res;*/
            return new VideoEncoderConfigurationOptions()
            {
                QualityRange = new IntRange() // Установите диапазон качества
                {
                    Min = 0,
                    Max = 10 // Пример диапазона качества от 0 до 10
                },
                JPEG = new JpegOptions() // Опции для JPEG
                {
                    ResolutionsAvailable = new VideoResolution[]
                    {
                        new VideoResolution() { Width = 640, Height = 360 },
                        new VideoResolution() { Width = 1280, Height = 720 },
                        new VideoResolution() { Width = 1920, Height = 1080 }
                    },
                    FrameRateRange = new IntRange() // Диапазон частоты кадров
                    {
                        Min = 1,
                        Max = 30
                    },
                    EncodingIntervalRange = new IntRange() // Интервалы кодирования (например, ключевые кадры)
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
                    GovLengthRange = new IntRange() // Диапазон длины GOV
                    {
                        Min = 1,
                        Max = 60
                    },
                    FrameRateRange = new IntRange() // Диапазон частоты кадров
                    {
                        Min = 1,
                        Max = 30
                    },
                    EncodingIntervalRange = new IntRange() // Интервал кодирования
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
                    GovLengthRange = new IntRange() // Диапазон длины GOV
                    {
                        Min = 1,
                        Max = 60
                    },
                    FrameRateRange = new IntRange() // Диапазон частоты кадров
                    {
                        Min = 1,
                        Max = 30
                    },
                    EncodingIntervalRange = new IntRange() // Интервал кодирования
                    {
                        Min = 1,
                        Max = 4
                    },
                    H264ProfilesSupported =
                    [
                        H264Profile.Baseline,
                        H264Profile.Main,
                        H264Profile.High
                    ]
                },
                GuaranteedFrameRateSupported = true,
                GuaranteedFrameRateSupportedSpecified = true,
            };
        }

        public override GetVideoSourcesResponse GetVideoSources(GetVideoSourcesRequest request)
        {
            var videoSources = new List<SharpOnvifServer.Media.VideoSource>();

            foreach (var video in _settings!.Profiles)
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

        private string ReadSettingsFromJson()
        {
            // Получаем директорию, где находится исполняемый файл
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // Путь к файлу относительно директории сборки
            string relativeFilePath = Path.Combine(basePath, "Config", "settings.json");

            // Проверяем, существует ли файл
            if (!File.Exists(relativeFilePath))
            {
                throw new FileNotFoundException($"Файл не найден: {relativeFilePath}");
            }

            // Читаем содержимое файла JSON
            string json = File.ReadAllText(relativeFilePath);

            return json;
        }
    }

}