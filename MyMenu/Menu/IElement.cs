namespace MyMenu
{
    public interface IElement
    {
        void Execute();
        string GetView();
        bool isValid();
    }
}