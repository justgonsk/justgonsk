namespace JustGoModels.Interfaces
{
    public interface IConvertibleToViewModel<TViewModel>
    {
        TViewModel ToViewModel();
    }
}