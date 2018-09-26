using UnityEngine;
using System.Collections.Generic;

public class Block : PoolObject
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
        isMegaBlock = false;
        BlockInfo = null;
    }

    public int ColorIndex;

    public int[] relativePosition = new int[2];
    public int[] gridPosition = new int[2];
    public int[] gridPosition_RotatePrepare = new int[2];

    public bool isBreaker = false;
    public GameObject dieEffect;

    public bool isMegaBlock = false;
    public int[] BlockInfo;//如果是MegaBlock的时候使用

    public void initiateBlock(int bc, int[] rp)
    {
        if (bc == -2)
            isBreaker = true;
        else
            ColorIndex = bc;

        relativePosition[0] = rp[1];
        relativePosition[1] = rp[2];

        gameObject.transform.localScale = new Vector3(BlocksManager.Instance.InitSize - BlocksManager.Instance.InitBorder, BlocksManager.Instance.InitSize - BlocksManager.Instance.InitBorder, BlocksManager.Instance.InitThick);
        gameObject.transform.position = new Vector3(relativePosition[0] + gameObject.transform.parent.position.x, relativePosition[1] + gameObject.transform.parent.position.y, BlocksManager.Instance.InitZ);
        gridPosition[0] = (int)(relativePosition[0] + gameObject.transform.parent.position.x + GameManager.Instance.Width / 2);
        gridPosition[1] = (int)(relativePosition[1] + gameObject.transform.parent.position.y + GameManager.Instance.Height / 2);
    }

    public void removeBlock()
    {
        BlocksManager.Instance.Grid[gridPosition[0], gridPosition[1]] = -1;
        BlocksManager.Instance.Blocks[gridPosition[0], gridPosition[1]] = null;

        if (dieEffect != null)
        {
            Vector3 pos = gameObject.transform.position;
            Vector3 thisPos = new Vector3(pos.x, pos.y, pos.z + 0.3f);
            Instantiate(dieEffect, thisPos, transform.rotation, BlocksManager.Instance.transform);
        }
        PoolRecycle();
    }

    //用于统计连通区
    public Block leftBlock;
    public Block rightBlock;
    public Block aboveBlock;
    public Block beneathBlock;
    public bool isSignedRootedInGround = false;
    public bool isSignedFly = false;

    public void SignRootedInGround()
    {
        isSignedRootedInGround = true;
        if (leftBlock != null && !leftBlock.isSignedRootedInGround)
            leftBlock.SignRootedInGround();
        if (rightBlock != null && !rightBlock.isSignedRootedInGround)
            rightBlock.SignRootedInGround();
        if (aboveBlock != null && !aboveBlock.isSignedRootedInGround)
            aboveBlock.SignRootedInGround();
        if (beneathBlock != null && !beneathBlock.isSignedRootedInGround)
            beneathBlock.SignRootedInGround();
    }

    //检查是否是碎片状态
    public List<Block> SignFly()
    {
        List<Block> res = new List<Block>();
        res.Add(this);

        isSignedFly = true;

        if (leftBlock != null && !leftBlock.isSignedFly)
        {
            List<Block> res_l = leftBlock.SignFly();
            foreach (Block b in res_l)
                res.Add(b);
        }
        if (rightBlock != null && !rightBlock.isSignedFly)
        {
            List<Block> res_r = rightBlock.SignFly();
            foreach (Block b in res_r)
                res.Add(b);
        }
        if (aboveBlock != null && !aboveBlock.isSignedFly)
        {
            List<Block> res_a = aboveBlock.SignFly();
            foreach (Block b in res_a)
                res.Add(b);
        }
        if (beneathBlock != null && !beneathBlock.isSignedFly)
        {
            List<Block> res_b = beneathBlock.SignFly();
            foreach (Block b in res_b)
                res.Add(b);
        }

        return res;
    }

}

