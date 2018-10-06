using UnityEngine;

public class GameObjectPoolManager : MonoSingletion<GameObjectPoolManager>
{
    private GameObjectPoolManager()
    {
    }

    public GameObjectPool Pool_BreakBlockPool;
    [SerializeField] private PoolObject BreakBlockPrefab;

    public GameObjectPool[] Pool_BlockPool;
    [SerializeField] private PoolObject[] BlockPrefab;

    public GameObjectPool Pool_BlockGroupPool;
    [SerializeField] private PoolObject BlockGroupPrefab;

    public GameObjectPool Pool_AbandonBlockGroupPool;
    [SerializeField] private PoolObject AbandonBlockGroupPrefab;

    public GameObjectPool Pool_GridLinePool;
    [SerializeField] private PoolObject GridLinePrefab;

    void Awake()
    {
        Pool_BreakBlockPool.Initiate((BreakBlockPrefab), 10);
        for (int i = 0; i < Pool_BlockPool.Length; i++)
        {
            Pool_BlockPool[i].Initiate((BlockPrefab[i]), 50);
        }

        Pool_BlockGroupPool.Initiate(BlockGroupPrefab, 30);
        Pool_AbandonBlockGroupPool.Initiate(AbandonBlockGroupPrefab, 30);
        Pool_GridLinePool.Initiate(GridLinePrefab, 50);
    }
}