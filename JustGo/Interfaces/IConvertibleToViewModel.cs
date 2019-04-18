namespace JustGo.Interfaces
{
    public interface IConvertibleToViewModel<out TViewModel>
    {
        TViewModel ConvertToViewModel();
    }
}