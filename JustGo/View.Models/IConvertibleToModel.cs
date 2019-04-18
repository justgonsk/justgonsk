namespace JustGo.View.Models
{
    public interface IConvertibleToModel<out TModel>
    {
        TModel ToModel();
    }
}