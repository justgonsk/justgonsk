namespace JustGo.Helpers
{
    public interface IConvertibleToViewModel<out TViewModel>
    {
        TViewModel ConvertToViewModel();
    }
}