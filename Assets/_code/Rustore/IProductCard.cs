namespace MegaGame
{
    public interface IProductCard<T>
    {
        T GetData();
        void SetData(T value);
    }
}
