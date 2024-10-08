﻿using SharpOnvifServer.Media;

namespace ONVIFStream.Config.Models
{
    public class Components
    {
        public class GetProfilesResponse
        {
            public Profile[] Profiles { get; set; }
        }

        public class Profile
        {
            public string Name { get; set; }
            public string token { get; set; }
            public VideoEncoderConfiguration VideoEncoderConfiguration { get; set; }
            public VideoSourceConfiguration VideoSourceConfiguration { get; set; }
        }

        public class VideoEncoderConfiguration
        {
            public string Encoding { get; set; }
            public VideoResolution Resolution { get; set; }
            public VideoRateControl RateControl { get; set; }
            public int Quality { get; set; }
        }

        public class VideoSourceConfiguration
        {
            public string token { get; set; }
            public string SourceToken { get; set; }
            public IntRectangle Bounds { get; set; }
        }

        public class VideoResolution
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public class VideoRateControl
        {
            public int FrameRateLimit { get; set; }
            public int EncodingInterval { get; set; }
        }

        public class IntRectangle
        {
            public int height { get; set; }
            public int width { get; set; }
        }

    }
}
