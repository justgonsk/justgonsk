using System;
using System.Threading.Tasks;
using JustGo.Interfaces;
using JustGo.Models;
using JustGo.View.Models.Edit;

namespace JustGo.Helpers
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
    }
}
