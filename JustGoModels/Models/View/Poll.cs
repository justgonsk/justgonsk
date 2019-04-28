using System;
using System.Collections.Generic;
using System.Linq;

namespace JustGoModels.Models.View
{
    /// <summary>
    /// DTO-шка для передачи списков чего-либо (событий, мест и т.д...)
    /// </summary>
    /// <typeparam name="T">Класс, элементы которого образуют список</typeparam>
    public class Poll<T>
    {
        public Poll(IEnumerable<T> results)
        {
            if (results == null)
                throw new ArgumentNullException(nameof(results));

            Results = results.ToList();
            Count = Results.Count;
        }

        public long Count { get; set; }
        public List<T> Results { get; set; }

        // FilterBy был перемещён в сам класс фильтра
    }
}