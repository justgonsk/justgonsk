namespace JustGoUtilities
{
    public static class Constants
    {
        public const string EventPollUrl =
            "https://kudago.com/public-api/v1.4/events/?location=nsk&expand=dates&fields=id,dates,title,short_title,place,description,categories,images,tags&actual_since=1554508800";

        public const string EventDetailsUrl = "https://kudago.com/public-api/v1.4/events/";

        //32414 - максимальный ID места на кудаго на момент этого коммита
        public const string PlaceDetailsUrlPattern =
            "https://kudago.com/public-api/v1.4/places/{0}/?lang=&fields=id,title,address,coords&expand=";

        public const int EventsPollDaemonTimespan = 600000;

        public const string CategoriesKey = "categories";
    }
}