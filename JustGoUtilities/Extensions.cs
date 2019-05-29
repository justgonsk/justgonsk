﻿using System;
using System.Collections.Generic;
using JustGoModels.Interfaces;
using JustGoModels.Models.View;
using JustGoModels.Policies;
using Microsoft.AspNetCore.Mvc;

namespace JustGoUtilities
{
    public static class Extensions
    {
        /// <summary>
        /// Получает из хранилища все объекты, на ходу превращая их в view-модели
        /// </summary>
        /// <returns>Перечисление всех объектов в виде view-моделей</returns>
        public static IEnumerable<TViewModel> AsViewModels<TModel, TViewModel>(this IEnumerable<TModel> modelSequence)
            where TModel : IConvertibleToViewModel<TViewModel>
        {
            foreach (var model in modelSequence)
            {
                yield return model.ToViewModel();
            }
        }

        /// <summary>
        /// Создаёт объект <see cref="Poll{T}"/> по последовательности view-моделек
        /// </summary>
        /// <typeparam name="TViewModel">Тип DTO</typeparam>
        /// <param name="viewModelSequence"></param>
        /// <returns></returns>
        public static Poll<TViewModel> ToPoll<TViewModel>(this IEnumerable<TViewModel> viewModelSequence)
        {
            return new Poll<TViewModel>(viewModelSequence);
        }

        public static Poll<TViewModel> ToPoll<TModel, TViewModel>(this IEnumerable<TModel> modelSequence)
            where TModel : IConvertibleToViewModel<TViewModel>
        {
            return modelSequence.AsViewModels<TModel, TViewModel>().ToPoll();
        }

        public static long ToUnixTimeSeconds(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }

        public static bool IsAdmin(this Controller controller) => controller.User.IsInRole(nameof(Admins));
    }
}