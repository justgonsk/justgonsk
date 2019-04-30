namespace JustGoModels.Interfaces
{
    public interface IConvertibleToViewModel<out TViewModel>
    {
        TViewModel ToViewModel();
    }
}