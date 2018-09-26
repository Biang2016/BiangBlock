using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AbandonBlockGroup : PoolObject
{
    public override void PoolRecycle()
    {
        foreach (Block b in bList)
        {
            b.PoolRecycle();
        }
        base.PoolRecycle();
        Anim.SetTrigger("Reset");
    }

    public Animator Anim;
    public List<Block> bList;

    public void Initialize(List<Block> input_bList)
    {
        bList = input_bList;
        foreach (Block b in bList)
        {
            if (b.isMegaBlock)
            {
                BlocksManager.Instance.RemoveMegaBlockAndChildren(b);
            }
        }
    }

    public void BeginDrop()
    {
        StartCoroutine(Co_BeginDrop());
    }

    IEnumerator Co_BeginDrop()
    {
        Anim.SetTrigger("JumpDown");
        yield return new WaitForSeconds(1);
        PoolRecycle();
    }
}
