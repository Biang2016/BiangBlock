using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BlocksManager : MonoSingletion<BlocksManager>
{
    public GameObject BlockGroupPrefab;
    public GameObject AbandonBlockGroupPrefab;

    public Vector3 StartPosition;
    public BlockGroup currentBlockGroup;
    public List<BlockGroup> currentBlockGroupPieces = new List<BlockGroup>();
    public List<Block> MegaBlocks = new List<Block>();

    //Block size, position
    public float InitSize = 1;
    public float InitBorder = 0.1f;
    public float InitThick = 0.3f;
    public float InitMegaThick = 0.4f;
    public float InitZ = -0.2f;

    //MegaBlocks disappear conditions
    public int maxSelfDisappearMegaBlockWidth = 7;
    public int maxSelfDisappearMegaBlockHeight = 7;
    public int maxSelfDisappearMegaBlockEdge = 10;


    void Awake()
    {
        InitSize = (float)19 / GameManager.Instance.Width;
    }

    void Start()
    {
    }

    public void StartGame()
    {
        Grid = new int[GameManager.Instance.Width, GameManager.Instance.Height];
        for (int i = 0; i < GameManager.Instance.Width; i++)
        {
            for (int j = 0; j < GameManager.Instance.Height; j++)
            {
                Grid[i, j] = -1;
            }
        }
        Blocks = new Block[GameManager.Instance.Width, GameManager.Instance.Height];
        MegaBlock_Pointers = new Block[GameManager.Instance.Width, GameManager.Instance.Height];
        StartPosition = new Vector3(0f, GameManager.Instance.Height / 2 + 3, InitZ);
        creatNewBlockGroup();
    }

    public void ResetGame()
    {
        foreach (Block b in Blocks)
        {
            b.PoolRecycle();
        }
        foreach (Block mb in MegaBlocks)
        {
            mb.PoolRecycle();
        }

        Grid = new int[GameManager.Instance.Width, GameManager.Instance.Height];
        for (int i = 0; i < GameManager.Instance.Width; i++)
        {
            for (int j = 0; j < GameManager.Instance.Height; j++)
            {
                Grid[i, j] = -1;
            }
        }
        Blocks = new Block[GameManager.Instance.Width, GameManager.Instance.Height];
        MegaBlock_Pointers = new Block[GameManager.Instance.Width, GameManager.Instance.Height];
        StartPosition = new Vector3(0f, GameManager.Instance.Height / 2 + 3, InitZ);
        creatNewBlockGroup();
    }

    void Update()
    {
        if (GameManager.Instance.GameState != GameManager.GameStates.Playing) return;
        updateKeyControl();
    }

    public void creatNewBlockGroup()
    {
        currentBlockGroup = GameObjectPoolManager.Instance.Pool_BlockGroupPool.AllocateGameObject<BlockGroup>(transform);
        currentBlockGroup.transform.position = StartPosition;
        currentBlockGroup.transform.rotation = transform.rotation;
        currentBlockGroup.Initialize();
    }


    //Key Control
    public float KeyPressTimeThreshold = 0.25f;
    public float KeyClickTimeThreshold = 0.15f;
    public float DownAccelerate = 1f;
    public float HorizontalAccelerate = 1f;

    private float downKeyTick = 0f;
    private float leftKeyTick = 0f;
    private float rightKeyTick = 0f;
    private float downKeyPressTime = 0f;
    private float leftKeyPressTime = 0f;
    private float rightKeyPressTime = 0f;

    public void resetKeyPressTime()
    {
        downKeyPressTime = 0;
        leftKeyPressTime = 0;
        rightKeyPressTime = 0;
    }

    public void resetKeyPressBeginTime()
    {
        downKeyTick = 0;
        leftKeyTick = 0;
        rightKeyTick = 0;
    }


    private void updateKeyControl()
    {
        if (currentBlockGroup != null)
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                downKeyPressTime += Time.deltaTime;
                downKeyTick += Time.deltaTime;
                if (downKeyPressTime >= KeyPressTimeThreshold)
                {
                    float downInterval = KeyPressTimeThreshold / (downKeyPressTime / KeyPressTimeThreshold);
                    if (downKeyTick >= downInterval / DownAccelerate)
                    {
                        currentBlockGroup.down();
                        downKeyTick = 0;
                    }
                }
            }
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (downKeyTick < KeyClickTimeThreshold)
                {
                    currentBlockGroup.down();
                }
                resetKeyPressTime();
                resetKeyPressBeginTime();
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                leftKeyPressTime += Time.deltaTime;
                leftKeyTick += Time.deltaTime;
                if (leftKeyPressTime >= KeyPressTimeThreshold)
                {

                }
                if (leftKeyPressTime >= KeyPressTimeThreshold)
                {
                    float leftInterval = KeyPressTimeThreshold / (leftKeyPressTime / KeyPressTimeThreshold);
                    if (leftKeyTick >= leftInterval / HorizontalAccelerate)
                    {
                        currentBlockGroup.left_move();
                        leftKeyTick = 0;
                    }
                }
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                if (leftKeyTick < KeyClickTimeThreshold)
                {
                    currentBlockGroup.left_move();
                }
                resetKeyPressTime();
                resetKeyPressBeginTime();
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                rightKeyPressTime += Time.deltaTime;
                rightKeyTick += Time.deltaTime;
                if (rightKeyPressTime >= KeyPressTimeThreshold)
                {
                    float rightInterval = KeyPressTimeThreshold / (rightKeyPressTime / KeyPressTimeThreshold);
                    if (rightKeyTick >= rightInterval / HorizontalAccelerate)
                    {
                        currentBlockGroup.right_move();
                        rightKeyTick = 0;
                    }
                }
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                if (rightKeyTick < KeyClickTimeThreshold)
                {
                    currentBlockGroup.right_move();
                }
                resetKeyPressTime();
                resetKeyPressBeginTime();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentBlockGroup.rotate();
            }
        }
    }

    #region Grid

    /*
	 * -2 - Breaker
	 * -1 - Empty
	 * 0 - Block_0
	 * 1 - Block_1
	 * 2 - Block_2
	 * 3 - Block_3
	*/
    public int[,] Grid;

    //每一格方块对应的脚本
    public Block[,] Blocks;

    public Block[,] MegaBlock_Pointers;

    public void refreshGrid()
    {
        if (tryBreakIntoPieces(GameManager.Instance.IsPieceAbandon))
        {
            return;
        }

        while (MegaBlocks.Count > 0)
        {
            Block mb = MegaBlocks[0];
            MegaBlocks.RemoveAt(0);
            mb.PoolRecycle();
        }

        List<int[]> temp = findAllBoxes(Grid, GameManager.Instance.ColorNum);
        int biggestMegaArea = 1;
        int currentBiggestRow = 1;
        int currentBiggestCol = 1;
        foreach (int[] tmp in temp)
        {
            createMegaBlock(tmp[0], tmp[1], tmp[2], tmp[3], tmp[4]);
            if (tmp[3] * tmp[4] > biggestMegaArea)
            {
                biggestMegaArea = tmp[3] * tmp[4];
                currentBiggestRow = tmp[4];
                currentBiggestCol = tmp[3];
            }
        }
        GameManager.Instance.SetMaxBlockSize(new int[] { currentBiggestRow, currentBiggestCol });

        bool isMegaBlockDisapear = false;
        int countEliminateScore = 0;
        for (int i = 0; i < MegaBlocks.Count; i++)
        {
            if (MegaBlocks[i].BlockInfo[3] >= maxSelfDisappearMegaBlockEdge || MegaBlocks[i].BlockInfo[4] >= maxSelfDisappearMegaBlockEdge || (MegaBlocks[i].BlockInfo[3] >= maxSelfDisappearMegaBlockWidth && MegaBlocks[i].BlockInfo[4] >= maxSelfDisappearMegaBlockHeight))
            {
                countEliminateScore += MegaBlocks[i].BlockInfo[3] * MegaBlocks[i].BlockInfo[4] * 2;
                RemoveMegaBlockAndChildren(MegaBlocks[i]);
                isMegaBlockDisapear = true;
                i--;
            }
        }

        if (countEliminateScore > 0)
            GameManager.Instance.GetEliminateScore(countEliminateScore);

        if (isMegaBlockDisapear)
            refreshGrid();

    }

    /// <summary>
    /// 尝试找到所有方块中的零散块，并生成BlockGroupPiece，为后续的自由落下做准备
    /// </summary>
    /// <returns><c>true</c>, if break into pieces was tryed, <c>false</c> otherwise.</returns>
    /// <param name="isPieceAbandon">是否抛弃落下的方块<c>true</c> isPieceAbandon.</param>
    public bool tryBreakIntoPieces(bool isPieceAbandon)
    {
        for (int i = 0; i < GameManager.Instance.Width; i++)
        {
            for (int j = 0; j < GameManager.Instance.Height; j++)
            {
                if (Blocks[i, j] == null)
                    continue;
                if (i > 0)
                    Blocks[i, j].leftBlock = Blocks[i - 1, j];
                if (i < GameManager.Instance.Width - 1)
                    Blocks[i, j].rightBlock = Blocks[i + 1, j];
                if (j > 0)
                    Blocks[i, j].beneathBlock = Blocks[i, j - 1];
                if (j < GameManager.Instance.Height - 1)
                    Blocks[i, j].aboveBlock = Blocks[i, j + 1];
                Blocks[i, j].isSignedRootedInGround = false;
                Blocks[i, j].isSignedFly = false;
            }
        }

        for (int i = 0; i < GameManager.Instance.Width; i++)
        {
            int j = 0;//对于在地面的那一排，他们所在的连通区被认为是接地的
            if (Blocks[i, j] != null && !Blocks[i, j].isSignedRootedInGround)
            {
                Blocks[i, j].SignRootedInGround();//递归标记连通区
            }
        }

        List<List<Block>> blockPiecesLL = new List<List<Block>>();
        foreach (Block b in Blocks)
        {
            if (b != null && !b.isSignedRootedInGround && !b.isSignedFly)
            {
                List<Block> blockPiecesL = b.SignFly();//递归标记连通区
                blockPiecesLL.Add(blockPiecesL);
            }
        }

        int countFallBlockNums = 0;
        foreach (List<Block> blockPiecesL in blockPiecesLL)
        {
            countFallBlockNums += blockPiecesL.Count;
            PoolObject newBlockGroup;
            if (!isPieceAbandon)
            {
                newBlockGroup = GameObjectPoolManager.Instance.Pool_BlockGroupPool.AllocateGameObject<BlockGroup>(transform);
            }
            else
            {
                newBlockGroup = GameObjectPoolManager.Instance.Pool_AbandonBlockGroupPool.AllocateGameObject<AbandonBlockGroup>(transform);
            }
            foreach (Block b in blockPiecesL)
            {
                b.transform.parent = newBlockGroup.transform;
                if (MegaBlock_Pointers[b.gridPosition[0], b.gridPosition[1]] != null)
                    MegaBlock_Pointers[b.gridPosition[0], b.gridPosition[1]].transform.parent = b.transform;
                Blocks[b.gridPosition[0], b.gridPosition[1]] = null;
                Grid[b.gridPosition[0], b.gridPosition[1]] = -1;
            }
            if (!isPieceAbandon)
            {
                BlockGroup bg = (BlockGroup)newBlockGroup;
                bg.Initialize(blockPiecesL);
                bg.isPiece = true;
                currentBlockGroupPieces.Add(bg);
            }
            else
            {
                ((AbandonBlockGroup)newBlockGroup).Initialize(blockPiecesL);
                ((AbandonBlockGroup)newBlockGroup).BeginDrop();
            }

        }

        if (countFallBlockNums > 0)
            GameManager.Instance.GetFallScore(countFallBlockNums);

        return blockPiecesLL.Count > 0;
    }

    //判断breaker是否撞上box
    public Block isInBoxes(int[] gridPosition)
    {
        foreach (Block megaBlock in MegaBlocks)
        {
            if (gridPosition[0] >= megaBlock.BlockInfo[1] && gridPosition[0] <= megaBlock.BlockInfo[1] + megaBlock.BlockInfo[3] - 1 && gridPosition[1] >= megaBlock.BlockInfo[2] && gridPosition[1] <= megaBlock.BlockInfo[2] + megaBlock.BlockInfo[4] - 1)
                return megaBlock;
        }
        return null;
    }

    public void RemoveMegaBlockAndChildren(Block megaBlock)
    {
        if (MegaBlocks.Contains(megaBlock))
        {
            for (int i = megaBlock.BlockInfo[1]; i <= megaBlock.BlockInfo[1] + megaBlock.BlockInfo[3] - 1; i++)
            {
                for (int j = megaBlock.BlockInfo[2]; j <= megaBlock.BlockInfo[2] + megaBlock.BlockInfo[4] - 1; j++)
                {
                    if (Blocks[i, j] != null)
                        Blocks[i, j].removeBlock();
                    MegaBlock_Pointers[i, j] = null;
                }
            }
            MegaBlocks.Remove(megaBlock);
            megaBlock.PoolRecycle();
        }
    }

    private List<int[]> findAllBoxes(int[,] grid, int colorNum)
    {
        if (grid == null || grid.GetLength(0) == 0 || grid.GetLength(1) == 0)
            return null;
        List<int[]> res = new List<int[]>();

        int[,,] dp = new int[grid.GetLength(0) + 1, grid.GetLength(1) + 1, colorNum];

        int[] max = new int[colorNum];
        for (int i = 1; i <= grid.GetLength(0); i++)
        {
            for (int j = 1; j <= grid.GetLength(1); j++)
            {
                for (int c = 0; c < colorNum; c++)
                {
                    if (grid[i - 1, j - 1] == c)
                    {
                        dp[i, j, c] = Mathf.Min(Mathf.Min(dp[i - 1, j - 1, c], dp[i, j - 1, c]), dp[i - 1, j, c]) + 1;
                        max[c] = Mathf.Max(max[c], dp[i, j, c]);
                    }
                }
            }
        }

        for (int c = 0; c < colorNum; c++)
        {
            if (max[c] > 1)
            {
                int count = 0;
                int countMax_Row = 0;
                int[] rightBottom_Row = new int[2];
                for (int i = 0; i < dp.GetLength(0); i++)
                {
                    for (int j = 0; j < dp.GetLength(1); j++)
                    {
                        if (dp[i, j, c] == max[c])
                        {
                            count++;
                            if (count > countMax_Row)
                            {
                                countMax_Row = count;
                                rightBottom_Row[0] = i - 1;
                                rightBottom_Row[1] = j - 1;
                            }
                        }
                        else
                        {
                            count = 0;
                        }
                    }
                    count = 0;
                }

                int countMax_Col = 0;
                int[] rightBottom_Col = new int[2];
                for (int j = 0; j < dp.GetLength(1); j++)
                {
                    for (int i = 0; i < dp.GetLength(0); i++)
                    {
                        if (dp[i, j, c] == max[c])
                        {
                            count++;
                            if (count > countMax_Col)
                            {
                                countMax_Col = count;
                                rightBottom_Col[0] = i - 1;
                                rightBottom_Col[1] = j - 1;
                            }
                        }
                        else
                        {
                            count = 0;
                        }
                    }
                    count = 0;
                }

                if (countMax_Row > countMax_Col)
                {
                    res.Add(new int[] {c, rightBottom_Row [0] - max [c] + 1, rightBottom_Row [1] - (countMax_Row + max [c] - 1) + 1,
                        max [c], countMax_Row + max [c] - 1
                    });
                }
                else
                {
                    res.Add(new int[] {c, rightBottom_Col [0] - (countMax_Col + max [c] - 1) + 1, rightBottom_Col [1] -
                        max [c] + 1, countMax_Col + max [c] - 1, max [c]
                    });
                }
            }
        }

        if (res.Count != 0)
        {
            int[,] resultGrid = new int[grid.GetLength(0), grid.GetLength(1)];
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    resultGrid[i, j] = grid[i, j];
                }
            }
            foreach (int[] megaBlock in res)
            {
                for (int i = megaBlock[1]; i <= megaBlock[1] + megaBlock[3] - 1; i++)
                {
                    for (int j = megaBlock[2]; j <= megaBlock[2] + megaBlock[4] - 1; j++)
                    {
                        resultGrid[i, j] = -1;
                    }
                }
            }
            List<int[]> newRes = findAllBoxes(resultGrid, colorNum);
            foreach (int[] megaBlock in newRes)
            {
                res.Add(megaBlock);
            }
        }

        return res;
    }

    private void createMegaBlock(int colorIndex, int startCol, int startRow, int Cols, int Rows)
    {
        Block newMegaBlock = GameObjectPoolManager.Instance.Pool_BlockPool[colorIndex].AllocateGameObject<Block>(transform);
        MegaBlocks.Add(newMegaBlock);
        newMegaBlock.isMegaBlock = true;
        newMegaBlock.BlockInfo = new int[] { colorIndex, startCol, startRow, Cols, Rows };
        float cenX = (startCol + Cols / 2f - GameManager.Instance.Width / 2f) * InitSize;
        float cenY = (startRow + Rows / 2f - GameManager.Instance.Height / 2f) * InitSize;
        float sizeX = Cols * InitSize - InitBorder;
        float sizeY = Rows * InitSize - InitBorder;
        newMegaBlock.transform.position = new Vector3(cenX, cenY, InitZ);
        newMegaBlock.transform.localScale = new Vector3(sizeX, sizeY, InitMegaThick);

        //记录小Block对应的MegaBlock
        for (int i = newMegaBlock.BlockInfo[1]; i <= newMegaBlock.BlockInfo[1] + newMegaBlock.BlockInfo[3] - 1; i++)
        {
            for (int j = newMegaBlock.BlockInfo[2]; j <= newMegaBlock.BlockInfo[2] + newMegaBlock.BlockInfo[4] - 1; j++)
            {
                MegaBlock_Pointers[i, j] = newMegaBlock;
            }
        }

        GameManager.Instance.SetMaxBlockSize(new int[] { Cols, Rows });
    }

    #endregion
}

//头脑风暴：
/*
 * 落下的方块，结成大冰块后会发射子弹、射线之类的东西，击落上方的冰块，并引导碎片飞到需要的地方
 * 或者，大冰块不会攻击，小方块会攻击，然后小方块会逐渐击毁大方块（掩体），导致无法继续增加方块
 * 下方块的时候要躲子弹
 * 悬空部分的方块直接化为奖励或者分数
 * 
 * 冰块变大的时候内部的效果一定要越来越酷
 * 
 * 坦克大战思路，给坦克的9个格子打各种装甲，不同装甲有不同功能，比如反射光束，那么一个坦克打出光束，团队协作，反射到敌人身上
 * 
 * 
 * 
 * 编程心得之：玩家手感
 * 在做俄罗斯方块的时候，虽然长按下键快速下落方块看似很简单的一个操作，然而蕴涵挺多小技巧。比如说玩家长按之后应该是一个逐渐加速的过程，
 * 不应该直接落到底部，而且就算落到了底部，也不应判定为结束下落，应该留给玩家零点几秒的时间来决定是否向左或者向右，甚至旋转。
*/
