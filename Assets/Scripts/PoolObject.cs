using UnityEngine;

public class PoolObject : MonoBehaviour
{
    private static int Increasingindex = 1000;
    public static int GenerateInstanceID()
    {
        return Increasingindex++;
    }

    public int PoolObjectInstanceID;

    private GameObjectPool m_Pool;
    public bool isPoolAvailable = true;
    public void SetObjectPool(GameObjectPool pool)
    {
        m_Pool = pool;
    }

    public virtual void PoolRecycle()
    {
        m_Pool.RecycleGameObject(this);
    }
}