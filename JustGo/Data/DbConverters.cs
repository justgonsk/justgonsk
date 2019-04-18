using System;
using System.Collections.Generic;
using System.Linq;
using JustGo.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JustGo.Data
{
    public static class DbConverters
    {
        /// <summary>
        /// Транслирует список URL картинок в строку, разделённую '|' для хранения в нашей базе
        /// При извлечении из базы воссоздаёт список моделей со ссылками
        /// </summary>
        ///
        /// <remarks>
        /// Отрефакторить этот говнокод не представляется возможным, потому что ValueConverter
        /// работает не с просто лямбдами, а с деревьями выражений
        /// </remarks>
        public static ValueConverter<ICollection<ImageModel>, string> ImagesConverter { get; }
            = new ValueConverter<ICollection<ImageModel>, string>
        (
            list => string.Join('|', list.Select(image => image.Image)),
            wholeString => wholeString.Split('|', StringSplitOptions.None).Select(url => new ImageModel
            {
                Image = url
            }).ToList()
        );
    }
}