using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JustGo.Models;
using JustGo.View.Models;

namespace JustGo.Interfaces
{
    /// <summary>
    /// Интерфейс для хранилища объектов бизнес-логики
    /// </summary>
    /// <typeparam name="TModel">Data-модель, она же просто модель, она же модель бизнес-логики</typeparam>
    /// <typeparam name="TViewModel">View-модель, она же DTO</typeparam>
    /// <typeparam name="TEditModel">Модель изменений для присваивания новых значений view-моделям.
    /// Если какое-то свойство равно null, то НЕ МЕНЯЕМ это свойство у view-модели.</typeparam>
    public interface IRepository<TModel, in TViewModel, in TEditModel>
        where TModel : IConvertibleToViewModel<TViewModel>
    {
        /// <summary>
        /// Добавляет в хранилище новый объект по данным view-модели
        /// </summary>
        /// <param name="viewModel">Данные для создания нового объекта</param>
        /// <returns>Модель бизнес-логики добавленного объекта</returns>
        Task<TModel> AddAsync(TViewModel viewModel);

        /// <summary>
        /// Получает из хранилища объект с указанным <paramref name="id"/>
        /// </summary>
        /// <param name="id">id объекта для поиска</param>
        /// <returns>Модель бизнес-логики объекта с ключом <paramref name="id"/></returns>
        Task<TModel> FindAsync(int id);

        /// <summary>
        /// Получает из хранилища все объекты
        /// </summary>
        /// <returns>Перечисление всех объектов</returns>
        IEnumerable<TModel> EnumerateAll();

        /// <summary>
        /// Находит в хранилище объект с указанным <paramref name="id"/> и обновляет его в соответствии с содержимым <paramref name="editModel"/>
        /// </summary>
        /// <param name="id">id объекта для обновления</param>
        /// <param name="editModel"></param>
        /// <returns>Модель бизнес-логики обновлённого объекта с ключом <paramref name="id"/></returns>
        Task<TModel> UpdateAsync(int id, TEditModel editModel);

        /// <summary>
        /// Удаляет из хранилища объект по указанному <paramref name="id"/>
        /// </summary>
        /// <param name="id">id объекта для удаления</param>
        /// <returns>Удалённый объект</returns>
        Task<TModel> DeleteAsync(int id);

        /// <summary>
        /// Обновляет модель значениями из view-модели
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        Task AssignProperties(TModel model, TViewModel viewModel);

        /// <summary>
        /// Обновляет модель значениями из edit-модели. Если какое-то свойство равно null,
        /// это значит что его не меняем.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="editModel"></param>
        /// <returns></returns>
        Task UpdateProperties(TModel model, TEditModel editModel);
    }
}