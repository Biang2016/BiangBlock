using UnityEngine;
using System.Collections.Generic;

public class BlockGroup : PoolObject
{
    public override void PoolRecycle()
    {
        base.PoolRecycle();
        bList = null;
        downTicker = 0;
        isBox = false;
        isPiece = false;
        isBroken = false;
    }

    public List<Block> bList;
    private float downTime;

    void Awake()
    {
    }

    private void InitDownTime()
    {
        if (!isPiece)
            downTime = GameManager.Instance.DownTime;
        else if (!GameManager.Instance.IsPieceAbandon)
            downTime = GameManager.Instance.PieceDownTime;
        else
            downTime = GameManager.Instance.AbandonPieceDownTime;
    }

    //每隔一定时间自动下落一格
    float downTicker = 0;

    void Update()
    {
        if (bList != null)
        {
            if (downTicker < downTime)
            {
                downTicker += Time.deltaTime;
            }
            else
            {
                down(true);
            }
        }
    }

    public void Initialize()
    {
        bList = RandomBlockGroup();
        InitDownTime();
    }

    public void Initialize(List<Block> input_bList)
    {
        bList = input_bList;
        InitDownTime();
    }

    public bool isBox = false;

    //随机生成一个方块组
    public List<Block> RandomBlockGroup()
    {
        BlockGroupGenerator.BlockGroupType blockType_selected = BlockGroupGenerator.Instance.getRandomBlockGroupType();

        //对于2X2正方形，由于其旋转时有特殊性，采用一个布尔值记录
        if (blockType_selected.isEvenSizeBoxShape)
            isBox = true;
        else
            isBox = false;

        //每个方块组中，由两种颜色构成
        int[] use_colors = new int[2];
        for (int i = 0; i < use_colors.Length; i++)
        {
            use_colors[i] = Random.Range(0, GameManager.Instance.ColorNum);
        }

        List<Block> res = new List<Block>();
        foreach (int[] block in blockType_selected.BlockRelativePos)
        {
            Block newBlock;
            if (block[0] == -2)
            {
                newBlock = GameObjectPoolManager.Instance.Pool_BreakBlockPool.AllocateGameObject<Block>(transform);
                newBlock.InitiateBlockByRelativePosition(block[0], block);
                res.Add(newBlock);
            }
            else
            {
                newBlock = GameObjectPoolManager.Instance.Pool_BlockPool[use_colors[block[0]]].AllocateGameObject<Block>(transform);
                newBlock.InitiateBlockByRelativePosition(use_colors[block[0]], block);
                res.Add(newBlock);
            }
        }

        return res;
    }

    #region Block motions

    public void down(bool needTranslate) //needTranslate表示，是否是有Addline引起的模拟down
    {
        if (bList == null) return;

        if (needTranslate) Debug.Log("SelfDown");
        else Debug.Log("AddLineDown");
        if (needTranslate) downTicker = 0;
        bool canMove = true;
        bool isReachBreaker = false;
        bool isBreakerReach = false;
        List<int[]> breakersGridPos = new List<int[]>();
        List<Block> ourBrokenBlock = new List<Block>();
        List<int[]> breakGridsPos = new List<int[]>();
        List<Block> ourBreakers = new List<Block>();

        bool needMoveUp = false;
        foreach (Block bi in bList)
        {
            if (bi.gridPosition[1] >= GameManager.Instance.Height)
                continue;

            if (!needTranslate)
            {
                if (BlocksManager.Instance.Grid[bi.gridPosition[0], bi.gridPosition[1]] != -1) //如果是Addline引起的模拟down，需要判定本体是否已经陷入上升的基础中
                {
                    needMoveUp = true;
                    break;
                }
            }
        }

        if (needMoveUp) //如果是Addline引起的模拟down，需要判定本体是否已经陷入上升的基础中，如果是，则要向上移动一格
        {
            gameObject.transform.Translate(BlocksManager.Instance.InitSize * Vector3.up);
            foreach (Block bi in bList)
            {
                bi.gridPosition[1]++;
            }
        }

        foreach (Block bi in bList)
        {
            if (bi.gridPosition[1] <= 0)
            {
                canMove = false;
                break;
            }

            if (bi.gridPosition[1] >= GameManager.Instance.Height)
                continue;

            //下两个位子的预测
            if (bi.gridPosition[1] <= 1)
            {
                BlocksManager.Instance.resetKeyPressTime();
            }
            else
            {
                int nextPos_2 = BlocksManager.Instance.Grid[bi.gridPosition[0], bi.gridPosition[1] - 2]; //下两个位子的情况
                if (nextPos_2 != -1)
                {
                    BlocksManager.Instance.resetKeyPressTime();
                }
            }

            //下个位子的预测
            int nextPos = BlocksManager.Instance.Grid[bi.gridPosition[0], bi.gridPosition[1] - 1]; //下个位子的情况

            if (nextPos == -2)
            {
                //下个位子遇到Breaker
                isReachBreaker = true;
                breakersGridPos.Add(new int[] {bi.gridPosition[0], bi.gridPosition[1] - 1});
                ourBrokenBlock.Add(bi);
            }

            if (nextPos != -1 && bi.isBreaker)
            {
                //如果下个位置非空且此方块为Breaker，则记录，待定
                isBreakerReach = true;
                breakGridsPos.Add(new int[] {bi.gridPosition[0], bi.gridPosition[1] - 1});
                ourBreakers.Add(bi);
            }

            if (nextPos != -1 && nextPos != -2 && !bi.isBreaker)
            {
                //如果自带非Breaker块的下一个位置非Breaker也非空，则无法移动
                canMove = false;
                break;
            }
        }

        if (bList != null)
        {
            if (isReachBreaker && canMove)
            {
                //如果碰到Breaker
                if (ourBrokenBlock.Count > 0)
                    reachBreaker(breakersGridPos, ourBrokenBlock);
            }
        }
        else
        {
            BlockGroupOver();
            return;
        }

        if (bList != null)
        {
            if (isBreakerReach && canMove)
            {
                //自带的Breaker碰到东西，且整体还能向下移动
                if (ourBreakers.Count > 0)
                    BreakerReach(ourBreakers, breakGridsPos);
            }
        }
        else
        {
            BlockGroupOver();
            return;
        }

        if (!isBroken)
        {
            if (bList != null && bList.Count > 0 && canMove)
            {
                if (needTranslate)
                {
                    gameObject.transform.Translate(BlocksManager.Instance.InitSize * Vector3.down);
                    foreach (Block bi in bList)
                    {
                        bi.gridPosition[1]--;
                    }
                }
            }
            else
            {
                reachBottom();
            }
        }
    }

    public void left_move()
    {
        bool canMove = true;
        bool isReachBreaker = false;
        bool isBreakerReach = false;
        List<int[]> breakersGridPos = new List<int[]>();
        List<Block> ourBrokenBlock = new List<Block>();
        List<int[]> breakGridsPos = new List<int[]>();
        List<Block> ourBreakers = new List<Block>();

        foreach (Block bi in bList)
        {
            if (bi.gridPosition[0] - 1 < 0)
            {
                canMove = false;
                break;
            }

            if (bi.gridPosition[1] >= GameManager.Instance.Height)
                continue;


            //下两个位子的预测
            if (bi.gridPosition[0] - 2 < 0)
            {
                BlocksManager.Instance.resetKeyPressTime();
            }
            else
            {
                int nextPos_2 = BlocksManager.Instance.Grid[bi.gridPosition[0] - 2, bi.gridPosition[1]]; //下两个位子的情况
                if (nextPos_2 != -1)
                {
                    BlocksManager.Instance.resetKeyPressTime();
                }
            }

            //下个位子的预测
            int nextPos = BlocksManager.Instance.Grid[bi.gridPosition[0] - 1, bi.gridPosition[1]]; //下个位子的情况

            if (nextPos == -2)
            {
                //下个位子遇到Breaker
                isReachBreaker = true;
                breakersGridPos.Add(new int[] {bi.gridPosition[0] - 1, bi.gridPosition[1]});
                ourBrokenBlock.Add(bi);
            }

            if (nextPos != -1 && bi.isBreaker)
            {
                //如果下个位置非空且此方块为Breaker，则记录，待定
                isBreakerReach = true;
                breakGridsPos.Add(new int[] {bi.gridPosition[0] - 1, bi.gridPosition[1]});
                ourBreakers.Add(bi);
            }

            if (nextPos != -1 && nextPos != -2 && !bi.isBreaker)
            {
                //如果自带非Breaker块的下一个位置非Breaker也非空，则无法移动
                canMove = false;
                break;
            }
        }

        if (bList != null)
        {
            if (isReachBreaker)
            {
                //如果碰到Breaker
                if (ourBrokenBlock.Count > 0)
                    reachBreaker(breakersGridPos, ourBrokenBlock);
            }
        }
        else
        {
            BlockGroupOver();
            return;
        }

        if (bList != null)
        {
            if (isBreakerReach && canMove)
            {
                //自带的Breaker碰到东西，且整体还能向左移动
                if (ourBreakers.Count > 0)
                    BreakerReach(ourBreakers, breakGridsPos);
            }
        }
        else
        {
            BlockGroupOver();
            return;
        }

        if (!isBroken)
        {
            if (bList != null)
            {
                if (bList.Count > 0 && canMove)
                {
                    gameObject.transform.Translate(BlocksManager.Instance.InitSize * Vector3.left);
                    foreach (Block bi in bList)
                    {
                        bi.gridPosition[0]--;
                    }
                }
            }
            else
            {
                BlockGroupOver();
                return;
            }
        }
    }

    public void right_move()
    {
        bool canMove = true;
        bool isReachBreaker = false;
        bool isBreakerReach = false;
        List<int[]> breakersGridPos = new List<int[]>();
        List<Block> ourBrokenBlock = new List<Block>();
        List<int[]> breakGridsPos = new List<int[]>();
        List<Block> ourBreakers = new List<Block>();

        foreach (Block bi in bList)
        {
            if (bi.gridPosition[0] + 1 >= GameManager.Instance.Width)
            {
                canMove = false;
                break;
            }

            if (bi.gridPosition[1] >= GameManager.Instance.Height)
                continue;

            //下两个位子的预测
            if (bi.gridPosition[0] + 2 >= GameManager.Instance.Width)
            {
                BlocksManager.Instance.resetKeyPressTime();
            }
            else
            {
                int nextPos_2 = BlocksManager.Instance.Grid[bi.gridPosition[0] + 2, bi.gridPosition[1]]; //下两个位子的情况
                if (nextPos_2 != -1)
                {
                    BlocksManager.Instance.resetKeyPressTime();
                }
            }

            //下个位子的预测
            int nextPos = BlocksManager.Instance.Grid[bi.gridPosition[0] + 1, bi.gridPosition[1]]; //下个位子的情况

            if (nextPos == -2)
            {
                //下个位子遇到Breaker
                isReachBreaker = true;
                breakersGridPos.Add(new int[] {bi.gridPosition[0] + 1, bi.gridPosition[1]});
                ourBrokenBlock.Add(bi);
            }

            if (nextPos != -1 && bi.isBreaker)
            {
                //如果下个位置非空且此方块为Breaker，则记录，待定
                isBreakerReach = true;
                breakGridsPos.Add(new int[] {bi.gridPosition[0] + 1, bi.gridPosition[1]});
                ourBreakers.Add(bi);
            }

            if (nextPos != -1 && nextPos != -2 && !bi.isBreaker)
            {
                //如果自带非Breaker块的下一个位置非Breaker也非空，则无法移动
                canMove = false;
                break;
            }
        }

        if (bList != null)
        {
            if (isReachBreaker)
            {
                //如果碰到Breaker
                if (ourBrokenBlock.Count > 0)
                    reachBreaker(breakersGridPos, ourBrokenBlock);
            }
        }
        else
        {
            BlockGroupOver();
            return;
        }

        if (bList != null)
        {
            if (isBreakerReach && canMove)
            {
                //自带的Breaker碰到东西，且整体还能向左移动
                if (ourBreakers.Count > 0)
                    BreakerReach(ourBreakers, breakGridsPos);
            }
        }
        else
        {
            BlockGroupOver();
            return;
        }

        if (!isBroken)
        {
            if (bList != null)
            {
                if (bList.Count > 0 && canMove)
                {
                    gameObject.transform.Translate(BlocksManager.Instance.InitSize * Vector3.right);
                    foreach (Block bi in bList)
                    {
                        bi.gridPosition[0]++;
                    }
                }
            }
            else
            {
                BlockGroupOver();
                return;
            }
        }
    }

    public void rotate()
    {
        if (isBox)
        {
            rotateBox();
            return;
        }

        bool canRotate = true;
        foreach (Block bi in bList)
        {
            bi.gridPosition_RotatePrepare[0] = (int) (-bi.relativePosition[1] + gameObject.transform.position.x + GameManager.Instance.Width / 2);
            bi.gridPosition_RotatePrepare[1] = (int) (bi.relativePosition[0] + gameObject.transform.position.y + GameManager.Instance.Height / 2);
            if (bi.gridPosition_RotatePrepare[0] >= GameManager.Instance.Width || bi.gridPosition_RotatePrepare[0] < 0 || bi.gridPosition_RotatePrepare[1] < 0 || bi.gridPosition_RotatePrepare[1] >= GameManager.Instance.Height)
            {
                canRotate = false;
                break;
            }

            if (BlocksManager.Instance.Grid[bi.gridPosition_RotatePrepare[0], bi.gridPosition_RotatePrepare[1]] != -1)
            {
                canRotate = false;
                break;
            }
        }

        if (canRotate)
        {
            foreach (Block bi in bList)
            {
                bi.gameObject.transform.Translate(BlocksManager.Instance.InitSize * Vector3.right * (bi.gridPosition_RotatePrepare[0] - bi.gridPosition[0]) + Vector3.up * (bi.gridPosition_RotatePrepare[1] - bi.gridPosition[1]));

                bi.gridPosition[0] = bi.gridPosition_RotatePrepare[0];
                bi.gridPosition[1] = bi.gridPosition_RotatePrepare[1];
                int temp = bi.relativePosition[0];
                bi.relativePosition[0] = -bi.relativePosition[1];
                bi.relativePosition[1] = temp;
            }
        }
    }

    private void rotateBox()
    {
        float cenX = 0;
        float cenY = 0;
        foreach (Block bi in bList)
        {
            cenX += bi.gridPosition[0];
            cenY += bi.gridPosition[1];
        }

        cenX /= bList.Count;
        cenY /= bList.Count;
        foreach (Block bi in bList)
        {
            bi.gridPosition_RotatePrepare[0] = (int) (cenY + cenX - bi.gridPosition[1]);
            bi.gridPosition_RotatePrepare[1] = (int) (cenY - cenX + bi.gridPosition[0]);
        }

        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).Translate(BlocksManager.Instance.InitSize * Vector3.right * (bList[i].gridPosition_RotatePrepare[0] - bList[i].gridPosition[0]) + Vector3.up * (bList[i].gridPosition_RotatePrepare[1] - bList[i].gridPosition[1]));
        }

        foreach (Block bi in bList)
        {
            bi.gridPosition[0] = bi.gridPosition_RotatePrepare[0];
            bi.gridPosition[1] = bi.gridPosition_RotatePrepare[1];
        }
    }

    //方块到底后，触发一系列事件
    private void reachBottom()
    {
        Debug.Log("ReachBottom");
        if (bList != null)
            foreach (Block bi in bList)
            {
                if (bi.gridPosition[1] >= GameManager.Instance.Height)
                {
                    Debug.Log("Game Over");
                    GameManager.Instance.GameOver();
                    return;
                }

                if (bi.isBreaker)
                {
                    BlocksManager.Instance.Grid[bi.gridPosition[0], bi.gridPosition[1]] = -2;
                }
                else
                {
                    BlocksManager.Instance.Grid[bi.gridPosition[0], bi.gridPosition[1]] = bi.ColorIndex;
                }

                BlocksManager.Instance.Blocks[bi.gridPosition[0], bi.gridPosition[1]] = bi;
                bi.gameObject.transform.parent = BlocksManager.Instance.gameObject.transform;
            }

        BlockGroupOver();
    }

    private void BlockGroupOver()
    {
        BlocksManager.Instance.RefreshGrid();
        if (isPiece)
        {
            BlocksManager.Instance.currentBlockGroupPieces.Remove(this);
        }

        if (BlocksManager.Instance.currentBlockGroup == this)
        {
            BlocksManager.Instance.currentBlockGroup = null;
            BlocksManager.Instance.resetKeyPressTime();
            BlocksManager.Instance.resetKeyPressBeginTime();
        }

        if (BlocksManager.Instance.currentBlockGroupPieces.Count == 0 && BlocksManager.Instance.currentBlockGroup == null)
        {
            BlocksManager.Instance.creatNewBlockGroup();
        }

        PoolRecycle();
    }

    //方块触及Breaker后的事件
    private void reachBreaker(List<int[]> breakersGridPos, List<Block> ourBrokenBlock)
    {
        foreach (int[] breakerGridPos in breakersGridPos)
        {
            if (BlocksManager.Instance.Blocks[breakerGridPos[0], breakerGridPos[1]] != null)
                BlocksManager.Instance.Blocks[breakerGridPos[0], breakerGridPos[1]].removeBlock();
        }

        foreach (Block ob in ourBrokenBlock)
        {
            if (bList != null) bList.Remove(ob);
            ob.removeBlock();
        }

        tryBreakIntoPieces(true);
        BlocksManager.Instance.TryBreakIntoPieces(GameManager.Instance.IsPieceAbandon);
    }

    //自带Breaker触及方块后的事件
    private void BreakerReach(List<Block> ourBreakers, List<int[]> breakGridsPos)
    {
        int countBreakScore = 0;
        foreach (Block ourBreaker in ourBreakers)
        {
            if (bList != null) bList.Remove(ourBreaker);
            ourBreaker.removeBlock();
        }

        foreach (int[] breakGridPos in breakGridsPos)
        {
            Block breakMegaBlock = BlocksManager.Instance.IsInBoxes(breakGridPos);
            if (breakMegaBlock != null)
            {
                countBreakScore += breakMegaBlock.BlockInfo[3] * breakMegaBlock.BlockInfo[4];
                BlocksManager.Instance.RemoveMegaBlockAndChildren(breakMegaBlock);
            }
            else if (BlocksManager.Instance.Blocks[breakGridPos[0], breakGridPos[1]] != null)
            {
                countBreakScore += 1;
                BlocksManager.Instance.Blocks[breakGridPos[0], breakGridPos[1]].removeBlock();
            }
        }

        if (countBreakScore > 0)
            GameManager.Instance.GetBreakScore(countBreakScore);

        tryBreakIntoPieces(GameManager.Instance.IsPieceAbandon);
        BlocksManager.Instance.TryBreakIntoPieces(GameManager.Instance.IsPieceAbandon);
    }

    //是否为大块分裂成的小块
    public bool isPiece = false;

    //该大块是否已经分裂
    public bool isBroken = false;

    private void tryBreakIntoPieces(bool isPieceAbandon)
    {
        if (bList.Count == 0)
            return;
        int[] unionFind = new int[bList.Count];
        for (int i = 0; i < unionFind.Length; i++)
        {
            unionFind[i] = i;
        }

        for (int i = 0; i < bList.Count; i++)
        {
            for (int j = i + 1; j < bList.Count; j++)
            {
                if (bList[i].gridPosition[0] == bList[j].gridPosition[0])
                {
                    if (bList[i].gridPosition[1] + 1 == bList[j].gridPosition[1] || bList[i].gridPosition[1] - 1 == bList[j].gridPosition[1])
                    {
                        int rootI = i;
                        while (rootI != unionFind[rootI])
                        {
                            rootI = unionFind[rootI];
                        }

                        int rootJ = j;
                        while (rootJ != unionFind[rootJ])
                        {
                            rootJ = unionFind[rootJ];
                        }

                        if (rootI != rootJ)
                            unionFind[rootJ] = rootI;
                    }
                }
                else if (bList[i].gridPosition[1] == bList[j].gridPosition[1])
                {
                    if (bList[i].gridPosition[0] + 1 == bList[j].gridPosition[0] || bList[i].gridPosition[0] - 1 == bList[j].gridPosition[0])
                    {
                        int rootI = i;
                        while (rootI != unionFind[rootI])
                        {
                            rootI = unionFind[rootI];
                        }

                        int rootJ = j;
                        while (rootJ != unionFind[rootJ])
                        {
                            rootJ = unionFind[rootJ];
                        }

                        if (rootI != rootJ)
                            unionFind[rootJ] = rootI;
                    }
                }
            }
        }

        for (int i = 0; i < unionFind.Length; i++)
        {
            while (unionFind[i] != unionFind[unionFind[i]])
            {
                unionFind[i] = unionFind[unionFind[i]];
            }
        }

        Dictionary<int, List<Block>> dic = new Dictionary<int, List<Block>>();
        for (int i = 0; i < unionFind.Length; i++)
        {
            if (!dic.ContainsKey(unionFind[i]))
            {
                dic.Add(unionFind[i], new List<Block>());
            }

            dic[unionFind[i]].Add(bList[i]);
        }

        if (dic.Count == 1)
            return;
        foreach (List<Block> lb in dic.Values)
        {
            BlockGroup newBlockGroup = GameObjectPoolManager.Instance.Pool_BlockGroupPool.AllocateGameObject<BlockGroup>(transform.parent);
            newBlockGroup.transform.rotation = transform.parent.rotation;
            newBlockGroup.transform.position = transform.position;
            foreach (Block b in lb)
            {
                b.transform.parent = newBlockGroup.transform;
            }

            newBlockGroup.Initialize(lb);
            newBlockGroup.isPiece = true;
            BlocksManager.Instance.currentBlockGroupPieces.Add(newBlockGroup);
        }

        bList.Clear();

        isBroken = true;
        if (isPiece)
            BlocksManager.Instance.currentBlockGroupPieces.Remove(this);
        PoolRecycle();
        BlocksManager.Instance.resetKeyPressTime();
        BlocksManager.Instance.resetKeyPressBeginTime();
    }

    #endregion
}