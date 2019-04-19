using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGo.Data;
using JustGo.Helpers;
using JustGo.Interfaces;
using JustGo.Models;
using JustGo.View.Models;

namespace JustGo.Repositories
{
    internal class DbPlacesRepository : IPlacesRepository
    {
        private readonly MainContext context;

        public DbPlacesRepository(MainContext context)
        {
            this.context = context;
        }

        public IEnumerable<Place> EnumerateAll() => context.Places;

        public Poll<PlaceViewModel> GetPlacePoll(int? offset = 0, int? count = 100)
        {
            var wholeSequence = EnumerateAll();

            var limitedSequence = wholeSequence
                .Skip(offset ?? 0).Take(count ?? 100)
                .AsViewModels<Place, PlaceViewModel>();

            return limitedSequence.ToPoll();
        }

        public async Task<Place> AddAsync(PlaceViewModel viewModel)
        {
            var newPlace = new Place();

            AssignProperties(newPlace, viewModel);

            context.Places.Add(newPlace);

            await context.SaveChangesAsync();

            return newPlace;
        }

        public async Task<Place> FindAsync(int id)
        {
            var place = await context.Places.FindAsync(id);

            if (place == null)
            {
                return null;
            }

            await context.LoadNavigationProperties(context.Entry(place));

            return place;
        }

        public async Task<Place> UpdateAsync(int id, PlaceViewModel viewModel)
        {
            var placeToUpdate = context.Places.Find(id);

            if (placeToUpdate == null)
            {
                return null;
            }

            AssignProperties(placeToUpdate, viewModel);

            await context.SaveChangesAsync();

            return placeToUpdate;
        }

        public async Task<Place> DeleteAsync(int id)
        {
            var placeToRemove = context.Places.Find(id);

            if (placeToRemove == null)
            {
                return null;
            }

            context.Places.Remove(placeToRemove);

            await context.SaveChangesAsync();

            return placeToRemove;
        }

        private void AssignProperties(Place place, PlaceViewModel viewModel)
        {
            place.Title = viewModel.Title;
            place.Address = viewModel.Address;
            place.Coordinates = new Coordinates
            {
                Latitude = viewModel.Coordinates.Latitude,
                Longitude = viewModel.Coordinates.Longitude
            };
        }
    }
}