using System.Threading.Tasks;

namespace JustGo.Interfaces
{
    public interface IConvertibleToViewModel<TViewModel>
    {
        TViewModel ToViewModel();
    }
}