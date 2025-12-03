namespace TradeSync.Desktop.Views
{
    public interface ISaveable
    {
        bool HasUnsavedChanges { get; }
        Task SaveAsync();
        void DiscardChanges(); // Скасувати зміни
    }
}