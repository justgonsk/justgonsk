using JustGo.Helpers;
using JustGo.Models;
using JustGo.View.Models;

namespace JustGo.Interfaces
{
    public interface IPlacesRepository : IRepository<Place, PlaceViewModel>
    {
        Poll<PlaceViewModel> GetPlacePoll(int? offset = 0, int? count = 100);
    }
}