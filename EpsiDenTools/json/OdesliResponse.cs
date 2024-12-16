using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpsiDenTools.json
{

    public class OdesliResponse
    {
        public string entityUniqueId { get; set; }
        public string userCountry { get; set; }
        public string pageUrl { get; set; }
        public dynamic entitiesByUniqueId { get; set; }
        public Linksbyplatform linksByPlatform { get; set; }
    }

    public class Linksbyplatform
    {
        public Amazonmusic amazonMusic { get; set; }
        public Amazonstore amazonStore { get; set; }
        public Audiomack audiomack { get; set; }
        public Anghami anghami { get; set; }
        public Boomplay boomplay { get; set; }
        public Deezer deezer { get; set; }
        public Applemusic appleMusic { get; set; }
        public Itunes itunes { get; set; }
        public Napster napster { get; set; }
        public Pandora pandora { get; set; }
        public Soundcloud soundcloud { get; set; }
        public Tidal tidal { get; set; }
        public Yandex yandex { get; set; }
        public Youtube youtube { get; set; }
        public Youtubemusic youtubeMusic { get; set; }
        public Spotify spotify { get; set; }
    }

    public class Amazonmusic
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Amazonstore
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Audiomack
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Anghami
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Boomplay
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Deezer
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Applemusic
    {
        public string country { get; set; }
        public string url { get; set; }
        public string nativeAppUriMobile { get; set; }
        public string nativeAppUriDesktop { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Itunes
    {
        public string country { get; set; }
        public string url { get; set; }
        public string nativeAppUriMobile { get; set; }
        public string nativeAppUriDesktop { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Napster
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Pandora
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Soundcloud
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Tidal
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Yandex
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Youtube
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Youtubemusic
    {
        public string country { get; set; }
        public string url { get; set; }
        public string entityUniqueId { get; set; }
    }

    public class Spotify
    {
        public string country { get; set; }
        public string url { get; set; }
        public string nativeAppUriDesktop { get; set; }
        public string entityUniqueId { get; set; }
    }

}
