using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGo.Data;
using JustGo.Helpers;
using JustGo.Interfaces;
using JustGo.Models;
using JustGo.View.Models;
using JustGo.View.Models.Edit;

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

            await AssignProperties(newPlace, viewModel);

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

            await context.LoadNavigationsAsync(context.Entry(place));

            return place;
        }

        public async Task<Place> UpdateAsync(int id, PlaceEditModel editModel)
        {
            var placeToUpdate = context.Places.Find(id);

            if (placeToUpdate == null)
            {
                return null;
            }

            await UpdateProperties(placeToUpdate, editModel);

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

        public async Task AssignProperties(Place place, PlaceViewModel viewModel)
        {
            await Task.Run(() =>
            {
                place.Title = viewModel.Title;
                place.Address = viewModel.Address;
                place.Coordinates = new Coordinates
                {
                    Latitude = viewModel.Coordinates.Latitude,
                    Longitude = viewModel.Coordinates.Longitude
                };
            });
        }

        public async Task UpdateProperties(Place place, PlaceEditModel editModel)
        {
            await Task.Run(() =>
            {
                if (editModel.Title != null)
                {
                    place.Title = editModel.Title;
                }

                if (editModel.Address != null)
                {
                    place.Address = editModel.Address;
                }

                if (editModel.Coordinates != null)
                {
                    place.Coordinates = new Coordinates
                    {
                        Latitude = editModel.Coordinates.Latitude,
                        Longitude = editModel.Coordinates.Longitude
                    };
                }
            });
        }
    }
}