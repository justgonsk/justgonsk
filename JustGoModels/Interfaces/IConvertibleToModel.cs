namespace JustGoModels.Interfaces
{
    public interface IConvertibleToModel<out TModel>
    {
        TModel ToModel();
    }
}