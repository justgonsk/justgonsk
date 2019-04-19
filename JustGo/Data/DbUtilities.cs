using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace JustGo.Data
{
    public static class DbUtilities
    {
        /// <summary>
        /// Транслирует список URL картинок в строку, разделённую '|' для хранения в нашей базе
        /// При извлечении из базы воссоздаёт список моделей картинок со ссылками
        /// </summary>
        ///
        /// <remarks>
        /// Отрефакторить этот говнокод не представляется возможным, потому что ValueConverter
        /// работает не с просто лямбдами, а с деревьями выражений
        /// </remarks>
        public static ValueConverter<ICollection<ImageModel>, string> ImagesLinksConverter { get; }
            = new ValueConverter<ICollection<ImageModel>, string>(
            list => string.Join('|', list.Select(image => image.Image)),
            wholeString => wholeString.Split('|', StringSplitOptions.None).Select(url => new ImageModel
            {
                Image = url
            }).ToList()
        );

        /// <summary>
        /// Загружает из базы все навигационные свойства сущности
        /// </summary>
        /// <typeparam name="T">Класс, сопоставленный с таблицей в БД</typeparam>
        /// <param name="context">Объект контекста</param>
        /// <param name="entry">Объект сущности</param>
        /// <returns></returns>
        public static async Task LoadNavigationProperties<T>(this DbContext context, T entry) where T : class
        {
            foreach (var navigation in context.Entry(entry).Navigations)
            {
                await navigation.LoadAsync();
            }
        }
    }
}