using System.Collections.Generic;

public class BlockGroupGenerator : MonoSingletion<BlockGroupGenerator>
{
    private BlockGroupGenerator()
    {
    }

    void Awake()
    {
        initializeBlockType();
    }

    public class BlockGroupType
    {
        public int[][] BlockRelativePos;
        public int Frequecy;
        public int[] RandomNumRange = new int[2];

        //是否是偶数边长的方形，在旋转上有所要求
        public bool isEvenSizeBoxShape = false;

        public BlockGroupType(int[][] brp, int f, bool isESBS)
        {
            BlockRelativePos = brp;
            Frequecy = f;
            isEvenSizeBoxShape = isESBS;
        }

        public BlockGroupType(int[][] brp, int f)
        {
            BlockRelativePos = brp;
            Frequecy = f;
        }
    }

    //三个数字，第一个数字记录的是颜色(-2代表breaker)，后两个数字是Relative Position
    static BlockGroupType Block_tri_SingleColor = new BlockGroupType(new int[][] {
        new int[]{ 0, 0, 1 },
        new int[]{ 0, 0, 0 },
        new int[] { 0, 1, 0 },
        new int[] { 0, 0, -1 }
    }, 5);
    static BlockGroupType Block_lz = new BlockGroupType(new int[][] {
        new int[]{ 0, 0, 1 },
        new int[]{ 0, 0, 0 },
        new int[] { 1, 1, 0 },
        new int[] { 1, 1, -1 }
    }, 0);
    static BlockGroupType Block_lz_SingleColor = new BlockGroupType(new int[][] {
        new int[]{ 0, 0, 1 },
        new int[]{ 0, 0, 0 },
        new int[] { 0, 1, 0 },
        new int[] { 0, 1, -1 }
    }, 0);
    static BlockGroupType Block_rz = new BlockGroupType(new int[][] {
        new int[]{ 0, 0, 0 },
        new int[]{ 0, 0, -1 },
        new int[] { 1, 1, 0 },
        new int[] { 1, 1, 1 }
    }, 0);
    static BlockGroupType Block_rz_SingleColor = new BlockGroupType(new int[][] {
        new int[]{ 0, 0, 0 },
        new int[]{ 0, 0, -1 },
        new int[] { 0, 1, 0 },
        new int[] { 0, 1, 1 }
    }, 0);
    static BlockGroupType Block_L = new BlockGroupType(new int[][] {
        new int[]{ 0, -1, 0 },
        new int[]{ 0, -1, 1 },
        new int[] { 1, -1, -1 },
        new int[] { 1, 0, -1 },
        new int[] { 1, 1, -1 }
    }, 5);
    static BlockGroupType Block_L_SingleColor = new BlockGroupType(new int[][] {
        new int[]{ 0, -1, 0 },
        new int[]{ 0, -1, 1 },
        new int[] { 1, -1, -1 },
        new int[] { 1, 0, -1 },
        new int[] { 1, 1, -1 }
    }, 10);
    static BlockGroupType Block_l = new BlockGroupType(new int[][] {
        new int[]{ 0, 0, 0 },
        new int[]{ 0, 0, 1 },
        new int[] { 1, 0, -1 },
        new int[] { 1, 1, -1 }
    }, 4);
    static BlockGroupType Block_l_SingleColor = new BlockGroupType(new int[][] {
        new int[]{ 0, 0, 0 },
        new int[]{ 0, 0, 1 },
        new int[] { 0, 0, -1 },
        new int[] { 0, 1, -1 }
    }, 10);
    static BlockGroupType Block_j = new BlockGroupType(new int[][] {
        new int[]{ 0, 0, 0 },
        new int[]{ 0, 0, 1 },
        new int[] { 1, 0, -1 },
        new int[] { 1, -1, -1 }
    }, 4);
    static BlockGroupType Block_j_SingleColor = new BlockGroupType(new int[][] {
        new int[]{ 0, 0, 0 },
        new int[]{ 0, 0, 1 },
        new int[] { 0, 0, -1 },
        new int[] { 0, -1, -1 }
    }, 10);
    static BlockGroupType Block_bar = new BlockGroupType(new int[][] {
        new int[]{ 0, 0, 1 },
        new int[]{ 0, 0, 0 },
        new int[] { 1, 0, -1 },
        new int[] { 1, 0, -2 }
    }, 4);
    static BlockGroupType Block_bar_SingleColor = new BlockGroupType(new int[][] {
        new int[]{ 0, 0, 1 },
        new int[]{ 0, 0, 0 },
        new int[] { 0, 0, -1 },
        new int[] { 0, 0, -2 }
    }, 10);
    static BlockGroupType Block_box22_SingleColor = new BlockGroupType(new int[][] {
        new int[]{ 0, 0, 0 },
        new int[]{ 0, 0, 1 },
        new int[] { 0, 1, 1 },
        new int[] { 0, 1, 0 }
    }, 10, true);
    static BlockGroupType Block_box23 = new BlockGroupType(new int[][] {
        new int[]{ 0, -1, 0 },
        new int[]{ 0, -1, 1 },
        new int[] { 0, 0, 0 },
        new int[] { 0, 0, 1 },
        new int[] { 1, 1, 0 },
        new int[] { 1, 1, 1 }
    }, 7);
    static BlockGroupType Block_box23_SingleColor = new BlockGroupType(new int[][] {
        new int[]{ 0, -1, 0 },
        new int[]{ 0, -1, 1 },
        new int[] { 0, 0, 1 },
        new int[] { 0, 0, 0 },
        new int[] { 0, 1, 0 },
        new int[] { 0, 1, 1 }
    }, 10);
    static BlockGroupType Block_box33_SingleColor = new BlockGroupType(new int[][] {
        new int[]{ 0, -1, -1 },
        new int[]{ 0, -1, 0 },
        new int[]{ 0, -1, 1 },
        new int[] { 0, 0, -1 },
        new int[] { 0, 0, 0 },
        new int[] { 0, 0, 1 },
        new int[] { 0, 1, -1 },
        new int[] { 0, 1, 0 },
        new int[] { 0, 1, 1 },
    }, 10);
    static BlockGroupType Block_box34 = new BlockGroupType(new int[][] {
        new int[]{ 0, -1, -1 },
        new int[]{ 0, -1, 0 },
        new int[]{ 1, -1, 1 },
        new int[]{ 1, -1, 2 },
        new int[] { 0, 0, -1 },
        new int[] { 0, 0, 0 },
        new int[] { 1, 0, 1 },
        new int[] { 1, 0, 2 },
        new int[] { 0, 1, -1 },
        new int[] { 0, 1, 0 },
        new int[] { 1, 1, 1 },
        new int[] { 1, 1, 2 },
    }, 4);
    static BlockGroupType Block_box34_SingleColor = new BlockGroupType(new int[][] {
        new int[]{ 0, -1, -1 },
        new int[]{ 0, -1, 0 },
        new int[]{ 0, -1, 1 },
        new int[]{ 0, -1, 2 },
        new int[] { 0, 0, -1 },
        new int[] { 0, 0, 0 },
        new int[] { 0, 0, 1 },
        new int[] { 0, 0, 2 },
        new int[] { 0, 1, -1 },
        new int[] { 0, 1, 0 },
        new int[] { 0, 1, 1 },
        new int[] { 0, 1, 2 },
    }, 7);
    static BlockGroupType Block_barBreaker = new BlockGroupType(new int[][] {
        new int[]{ -2, 0, -2 },
        new int[]{ 0, 0, -1 },
        new int[]{ 0, 0, 0 },
        new int[] { 0, 0, 1 },
    }, 10);
    static BlockGroupType Block_BlockBreaker1 = new BlockGroupType(new int[][] {
        new int[]{ -2, 0, 0 },
    }, 15);
    static BlockGroupType Block_BlockBreaker2 = new BlockGroupType(new int[][] {
        new int[]{ -2, 0, 0 },
        new int[]{ -2, 0, 1 },
    }, 15);
    static BlockGroupType Block_BlockBreaker3 = new BlockGroupType(new int[][] {
        new int[]{ -2, 0, 1 },
        new int[]{ -2, 0, 0 },
        new int[]{ -2, 0, -1 },
    }, 7);
    static BlockGroupType Block_BlockBreaker22 = new BlockGroupType(new int[][] {
        new int[]{ -2, 0, -1 },
        new int[]{ -2, 0, 0 },
        new int[]{ -2, 1, 0 },
        new int[]{ -2, 1, -1 },
    }, 4, true);

    private static List<BlockGroupType> BlockGroupTypes;
    private int curFrequecySum = 0;

    private void initializeBlockType()
    {
        BlockGroupTypes = new List<BlockGroupType>();
        BlockGroupTypes.Add(Block_tri_SingleColor);
        BlockGroupTypes.Add(Block_lz);
        BlockGroupTypes.Add(Block_lz_SingleColor);
        BlockGroupTypes.Add(Block_rz);
        BlockGroupTypes.Add(Block_rz_SingleColor);
        BlockGroupTypes.Add(Block_l);
        BlockGroupTypes.Add(Block_l_SingleColor);
        BlockGroupTypes.Add(Block_L);
        BlockGroupTypes.Add(Block_L_SingleColor);
        BlockGroupTypes.Add(Block_j);
        BlockGroupTypes.Add(Block_j_SingleColor);
        BlockGroupTypes.Add(Block_bar);
        BlockGroupTypes.Add(Block_bar_SingleColor);
        BlockGroupTypes.Add(Block_box22_SingleColor);
        BlockGroupTypes.Add(Block_box23);
        BlockGroupTypes.Add(Block_box23_SingleColor);
        BlockGroupTypes.Add(Block_box33_SingleColor);
        BlockGroupTypes.Add(Block_box34);
        BlockGroupTypes.Add(Block_box34_SingleColor);
        BlockGroupTypes.Add(Block_barBreaker);
        BlockGroupTypes.Add(Block_BlockBreaker1);
        BlockGroupTypes.Add(Block_BlockBreaker1);
        BlockGroupTypes.Add(Block_BlockBreaker2);
        BlockGroupTypes.Add(Block_BlockBreaker3);
        BlockGroupTypes.Add(Block_BlockBreaker22);

        curFrequecySum = 0;
        foreach (BlockGroupType bgt in BlockGroupTypes)
        {
            bgt.RandomNumRange[0] = curFrequecySum;
            curFrequecySum += bgt.Frequecy;
            bgt.RandomNumRange[1] = curFrequecySum;
        }
    }

    public BlockGroupType getRandomBlockGroupType()
    {
        //随机选择一种方块类型
        int n = UnityEngine.Random.Range(0, BlockGroupGenerator.Instance.curFrequecySum);

        BlockGroupType blockType_selected = BlockGroupTypes[0];
        foreach (BlockGroupType bgt in BlockGroupTypes)
        {
            if (n >= bgt.RandomNumRange[0] && n < bgt.RandomNumRange[1])
            {
                blockType_selected = bgt;
            }
        }
        return blockType_selected;
    }

}

