using System.Threading.Tasks;
using JustGoModels.Interfaces;
using JustGoModels.Models;
using JustGoModels.Models.Edit;
using JustGoModels.Models.View;
using static JustGoUtilities.Utilities;

namespace KudagoDaemon
{
    public static class PlaceManager
    {
        public async static Task<Place> AddPlaceToDatabase(IPlacesRepository placesRepository, PlaceEditModel place)
        {
            var placeFromDatabase = await placesRepository.FindAsync(place.Id.Value);

            if (placeFromDatabase == null)
            {
                return await placesRepository.AddAsync(place);
            }

            return placeFromDatabase;
        }

        public async static Task<Poll<EventViewModel>> GetPlacesFromUrl(string url, string placeDetailsUrl)
        {
            var parsedPoll = await ParseResponseFromUrl(url);

            var pollInOurFormat = await ConvertToOurApiFormat(parsedPoll, placeDetailsUrl);

            var poll = pollInOurFormat.ToObject<Poll<EventViewModel>>(SnakeCaseSerializer);

            return poll;
        }
    }
}