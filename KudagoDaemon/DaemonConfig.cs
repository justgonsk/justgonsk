using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace KudagoDaemon
{
    public class DaemonConfig
    {
        private const string DefaultEventPollUrlPattern =
            "https://kudago.com/public-api/v1.4/events/?location=nsk&expand=dates&" +
            "fields=id,dates,title,short_title,place,description,categories,images,tags,body_text&actual_since={0}&actual_until={1}";
        private const string DefaultEventDetailsUrl = "https://kudago.com/public-api/v1.4/events/";
        private const int DefaultDateTimeRangeLengthInDays = 30;

        //32414 - максимальный ID места на кудаго на момент этого коммита
        private const string DefaultPlaceDetailsUrlPattern =
            "https://kudago.com/public-api/v1.4/places/{0}/?lang=&fields=id,title,address,coords&expand=";

        private const int DefaultTmespan = 600000;

        public string DaemonName { get; set; }
        public string EventPollUrlPattern { get; set; }
        public string EventDetailsUrl { get; set; }
        public int DateTimeRangeLengthInDays { get; set; }

        //32414 - максимальный ID места на кудаго на момент этого коммита
        public string PlaceDetailsUrlPattern { get; set; }

        public int Timespan { get; set; }

        private void UseDefaults()
        {
            EventPollUrlPattern = DefaultEventPollUrlPattern;
            EventDetailsUrl = DefaultEventDetailsUrl;
            DateTimeRangeLengthInDays = DefaultDateTimeRangeLengthInDays;
            PlaceDetailsUrlPattern = DefaultPlaceDetailsUrlPattern;
            Timespan = DefaultTmespan;
        }

        public void ReadConfigFromFile(string filename)
        {
            try
            {
                using (FileStream lStream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    XElement daemonSettingsRoot = XElement.Load(lStream);
                    EventPollUrlPattern = daemonSettingsRoot.Element("urls").Element("EventPollUrlPattern").Value;
                    EventDetailsUrl = daemonSettingsRoot.Element("urls").Element("EventDetailsUrl").Value;
                    PlaceDetailsUrlPattern = daemonSettingsRoot.Element("urls").Element("PlaceDetailsUrlPattern").Value;
                    DateTimeRangeLengthInDays = int.Parse(daemonSettingsRoot.Element("DateTimeRangeLengthInDays").Value);
                    Timespan = int.Parse(daemonSettingsRoot.Element("Timespan").Value);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception " + e.ToString() + " when read config file");
                UseDefaults();
            }
        }
    }
}
