namespace Domain.Port
{
    public interface IDependencies
    {
        T Instance<T>() where T : class;
    }
}
