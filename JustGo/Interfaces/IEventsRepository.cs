using System.Threading.Tasks;
using JustGo.Helpers;
using JustGo.Models;
using JustGo.View.Models;

namespace JustGo.Interfaces
{
    public interface IEventsRepository : IRepository<Event, EventViewModel>
    {
        /// <summary>
        /// Возвращает объект <see cref="Poll{T}"/> с view-моделями событий
        /// </summary>
        /// <param name="filter">Фильтр для отсеивания событий</param>
        /// <param name="offset">Сколько пропустить в последовательности после отсеивания, по умолчанию <value>0</value> </param>
        /// <param name="count">Сколько взять в последовательности после пропуска, по умолчанию <value>100</value></param>
        /// <returns></returns>
        Poll<EventViewModel> GetEventPoll(EventsFilter filter = null, int? offset = 0, int? count = 100);
    }
}