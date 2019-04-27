using JustGo.Helpers;
using JustGo.Models;
using JustGo.View.Models;
using JustGo.View.Models.Edit;

namespace JustGo.Interfaces
{
    public interface IPlacesRepository : IRepository<Place, PlaceViewModel, PlaceEditModel>
    {
        Poll<PlaceViewModel> GetPlacePoll(int? offset = 0, int? count = 100);
    }
}