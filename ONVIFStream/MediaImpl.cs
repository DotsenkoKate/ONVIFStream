using CoreWCF;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.Logging;
using SharpOnvifServer;
using SharpOnvifServer.Media;

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
            return new GetProfilesResponse()
            {
                Profiles = new Profile[]
                {
                    new Profile()
                    {
                        Name = "mainStream",
                        token = "Profile_1",
                        VideoEncoderConfiguration = new VideoEncoderConfiguration()
                        {
                            Encoding = VideoEncoding.JPEG,
                            Resolution = new VideoResolution()
                            {
                                Width = 640,
                                Height = 360
                            },
                            RateControl = new VideoRateControl()
                            {
                                FrameRateLimit = 30,
                                EncodingInterval = 1
                            },
                            Quality = 5
                        },
                        VideoSourceConfiguration = new VideoSourceConfiguration()
                        {
                            token = "Config_1",
                            SourceToken = "VideoSourceToken",
                            Bounds = new IntRectangle()
                            {
                                height = 640,
                                width = 360
                            }
                        }
                    }
                }
            };
        }
        public override Profile GetProfile(string ProfileToken)
        {
            try
            {

                if (string.IsNullOrEmpty(ProfileToken))
                {
                    throw new ArgumentException("ProfileToken is null or empty.");
                }

                var profiles = new List<Profile>
                {
                     new Profile()
                     {
                         Name = "mainStream",
                         token = "Profile_1",
                         VideoEncoderConfiguration = new VideoEncoderConfiguration()
                         {
                             Encoding = VideoEncoding.JPEG,
                             Resolution = new VideoResolution()
                             {
                                 Width = 640,
                                 Height = 360
                             },
                             RateControl = new VideoRateControl()
                             {
                                 FrameRateLimit = 30,
                                 EncodingInterval = 1
                             },
                             Quality = 5
                         },
                         VideoSourceConfiguration = new VideoSourceConfiguration()
                         {
                             token = "Config_1",
                             SourceToken = "VideoSourceToken",
                             Bounds = new IntRectangle()
                             {
                                 height = 640,
                                 width = 360
                             }
                         }
                     },
                };
                var profile = profiles.FirstOrDefault(p => p.token == ProfileToken);

                if (profile == null)
                {
                    throw new Exception($"Profile with token {ProfileToken} not found.");
                }

                return profile;
            }
            catch (Exception ex)
            {
                throw new Exception($"Profile error! Info: {ex}"); ;
            }
        }
        public override MediaUri GetSnapshotUri(string ProfileToken)
        {
            Console.WriteLine("GetSnapshotUri");

            return new MediaUri()
            {
                //Uri = $"{_server.GetHttpEndpoint()}/preview"
                Uri = "http://localhost:7238/cameras/snapshot/1/pars"
            };
        }
        public override MediaUri GetStreamUri(StreamSetup StreamSetup, string ProfileToken)
        {
            
            return new MediaUri()
            {
                //Uri = $"{_server.GetHttpEndpoint()}/preview"
                Uri = "http://10.10.2.204:5000/api/v1/complex/cameras/1/mjpeg"

            };
        }
        public override VideoSourceConfiguration GetVideoSourceConfiguration(string ConfigurationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(ConfigurationToken))
                {
                    throw new ArgumentException("ConfigurationToken is null or empty.");
                }

                // Конфигурация видеопотока
                var videoSourceConfigurations = new List<VideoSourceConfiguration>
                {
                    new VideoSourceConfiguration()
                    {
                        Name = "MainStreamConfig",
                        token = "Config_1",
                        SourceToken = "VideoSourceToken",
                        Bounds = new IntRectangle()
                        {
                            x = 0,
                            y = 0,
                            width = 640,
                            height = 360
                        }
                    },
                };

                // Поиск конфигурации по токену
                var configuration = videoSourceConfigurations.FirstOrDefault(c => c.token == ConfigurationToken);

                if (configuration == null)
                {
                    throw new Exception($"VideoSourceConfiguration with token {ConfigurationToken} not found.");
                }
                return configuration;
            }
            catch (Exception ex)
            {
                throw new Exception($"VideoSourceConfiguration error. Info: {ex}");
            }
        }
        public override VideoEncoderConfigurationOptions GetVideoEncoderConfigurationOptions(string ConfigurationToken, string ProfileToken)
        {

            // Создаем и возвращаем опции конфигурации кодировщика видео
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
            return new GetVideoSourcesResponse()
            {
                VideoSources = new VideoSource[]
                {
                    new VideoSource()
                    {
                        token = "VideoSourceToken",
                        Resolution = new SharpOnvifServer.Media.VideoResolution()
                        {
                            Width = 640,
                            Height = 360
                        }
                    }
                }
            };
        }
    }
}