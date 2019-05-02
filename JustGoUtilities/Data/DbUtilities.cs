using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using JustGoModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JustGoUtilities.Data
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

        public static ValueConverter<T, string> JsonConverter<T>()
        {
            return new ValueConverter<T, string>(
                list => JsonConvert.SerializeObject(list),
                jsonString => JsonConvert.DeserializeObject<T>(jsonString)
            );
        }

        /// <summary>
        /// Загружает из базы все навигационные свойства сущности рекурсивно
        /// </summary>
        /// <typeparam name="TEntityEntry">Класс, сопоставленный с таблицей в БД</typeparam>
        /// <param name="context">Объект контекста</param>
        /// <param name="entry">Объект сущности</param>
        /// <param name="recursionDepth"></param>
        /// <returns></returns>
        public static async Task LoadNavigationsAsync<TEntityEntry>(this DbContext context,
            TEntityEntry entry, int recursionDepth = 0) where TEntityEntry : EntityEntry
        {
            foreach (var navigation in entry.Navigations.Where(nav => !nav.IsLoaded))
            {
                await navigation.LoadAsync();
                if (recursionDepth > 0)
                {
                    var entities = navigation.CurrentValue as IEnumerable;
                    foreach (var recursiveEntity in entities ?? new[] { navigation.CurrentValue })
                    {
                        await context.LoadNavigationsAsync(context.Entry(recursiveEntity), recursionDepth - 1);
                    }
                }
            }
        }

        public static void IgnoreBaseProperties<T>(this EntityTypeBuilder<T> builder) where T : class
        {
            var baseProperties = typeof(T).BaseType.GetProperties().Select(p => p.Name);
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance)
                .Select(p => p.Name);

            foreach (var baseProperty in baseProperties.Except(properties))
            {
                builder.Ignore(baseProperty);
            }
        }
    }
}