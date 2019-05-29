using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace JustGoModels.Models.View
{
    /// <summary>
    /// DTO-шка для передачи списков чего-либо (событий, мест и т.д...)
    /// </summary>
    /// <typeparam name="T">Класс, элементы которого образуют список</typeparam>
    public class Poll<T>
    {
        [JsonIgnore]
        public string Next { get; }

        [JsonIgnore]
        public string Previous { get; }

        public Poll(IEnumerable<T> results)
        {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            Results = results.ToList();
        }

        public List<T> Results { get; }
        public long Count => Results.Count;

        public void AddRange(Poll<T> poll)
        {
            if (poll == null)
                throw new ArgumentNullException(nameof(poll));

            Results.AddRange(poll.Results);
        }

        // FilterBy был перемещён в сам класс фильтра
    }
}