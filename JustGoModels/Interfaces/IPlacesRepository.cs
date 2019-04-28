using JustGoModels.Models;
using JustGoModels.Models.Edit;
using JustGoModels.Models.View;

namespace JustGoModels.Interfaces
{
    public interface IPlacesRepository : IRepository<Place, PlaceViewModel, PlaceEditModel>
    {
        Poll<PlaceViewModel> GetPlacePoll(int? offset = 0, int? count = 100);
    }
}