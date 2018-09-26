using UnityEngine;

public class GameObjectPoolManager : MonoSingletion<GameObjectPoolManager>
{
    private GameObjectPoolManager()
    {
    }

    public GameObjectPool Pool_BreakBlockPool;
    public PoolObject BreakBlockPrefab;

    public GameObjectPool[] Pool_BlockPool;
    public PoolObject[] BlockPrefab;

    public GameObjectPool Pool_BlockGroupPool;
    public PoolObject BlockGroupPrefab;

    public GameObjectPool Pool_AbandonBlockGroupPool;
    public PoolObject AbandonBlockGroupPrefab;

    void Awake()
    {
        Pool_BreakBlockPool.Initiate((BreakBlockPrefab), 10);
        for (int i = 0; i < Pool_BlockPool.Length; i++)
        {
            Pool_BlockPool[i].Initiate((BlockPrefab[i]), 50);
        }

        Pool_BlockGroupPool.Initiate(BlockGroupPrefab, 30);
        Pool_AbandonBlockGroupPool.Initiate(AbandonBlockGroupPrefab, 30);
    }
}