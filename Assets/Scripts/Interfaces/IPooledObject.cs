namespace RunnerGame.Interfaces
{
    
    public interface IPooledObject
    {
        string PoolTag { get; }
        void OnSpawn();
        void OnDespawn();
    }
}
