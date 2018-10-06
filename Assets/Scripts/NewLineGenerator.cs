using UnityEngine;
using System.Collections;

public class NewLineGenerator
{
    private static int[,] newGrid;
    private static int currentCopyLineIndex = 0;

    public static int F22;
    public static int F33;
    public static int F44;

    public static void Initialize()
    {
        F22 = GameManager.Instance.Frequency22;
        F33 = GameManager.Instance.Frequency33;
        F44 = GameManager.Instance.Frequency44;
    }

    public static int[] GetNewLine()
    {
        if (newGrid == null)
        {
            GenerateNewGrid();
        }
        if (currentCopyLineIndex >= newGrid.GetLength(1))
        {
            currentCopyLineIndex = 0;
            GenerateNewGrid();
        }
        int[] newline = new int[GameManager.Instance.Width];
        for (int i = 0; i < newline.Length; i++)
        {
            newline[i] = newGrid[i, currentCopyLineIndex];
        }
        currentCopyLineIndex++;
        return newline;
    }

    public static void IncreaseHardLevel(int num)//生成方块的杂乱等级
    {
        F22 += num * 1;
        F33 += num * 2;
        F44 += num * 3;
        GenerateNewGrid();
    }

    public static void GenerateNewGrid()
    {
        newGrid = new int[GameManager.Instance.Width, GameManager.Instance.Height];

        // For 1x1 block
        for (int i = 0; i < GameManager.Instance.Width; i++)
        {
            for (int j = 0; j < GameManager.Instance.Height; j++)
            {
                int index = Random.Range(0, BlocksManager.Instance.LeftColors.Count);
                int color = BlocksManager.Instance.LeftColors[index];
                newGrid[i, j] = BlocksManager.Instance.LeftColors[index];
            }
        }

        // For 2x2 block
        for (int i = 0; i < GameManager.Instance.Width - 1; i += Random.Range(F22, F22 + 3))
        {
            for (int j = 0; j < GameManager.Instance.Height - 1; j += Random.Range(F22, F22 + 3))
            {
                int index = Random.Range(0, BlocksManager.Instance.LeftColors.Count);
                int color = BlocksManager.Instance.LeftColors[index];
                newGrid[i, j] = color;
                newGrid[i + 1, j] = color;
                newGrid[i, j + 1] = color;
                newGrid[i + 1, j + 1] = color;
            }
        }

        // For 3x3 block
        for (int i = 0; i < GameManager.Instance.Width - 2; i += Random.Range(F33, F33 + 5))
        {
            for (int j = 0; j < GameManager.Instance.Height - 2; j += Random.Range(F33, F33 + 5))
            {
                int index = Random.Range(0, BlocksManager.Instance.LeftColors.Count);
                int color = BlocksManager.Instance.LeftColors[index];
                newGrid[i, j] = color;
                newGrid[i + 1, j] = color;
                newGrid[i + 2, j] = color;
                newGrid[i, j + 1] = color;
                newGrid[i + 1, j + 1] = color;
                newGrid[i + 2, j + 1] = color;
                newGrid[i, j + 2] = color;
                newGrid[i + 1, j + 2] = color;
                newGrid[i + 2, j + 2] = color;
            }
        }

        // For 4x4 block
        for (int i = 0; i < GameManager.Instance.Width - 3; i += Random.Range(F44, F44 + 6))
        {
            for (int j = 0; j < GameManager.Instance.Height - 3; j += Random.Range(F44, F44 + 6))
            {
                int index = Random.Range(0, BlocksManager.Instance.LeftColors.Count);
                int color = BlocksManager.Instance.LeftColors[index];
                newGrid[i, j] = color;
                newGrid[i + 1, j] = color;
                newGrid[i + 2, j] = color;
                newGrid[i + 3, j] = color;
                newGrid[i, j + 1] = color;
                newGrid[i + 1, j + 1] = color;
                newGrid[i + 2, j + 1] = color;
                newGrid[i + 3, j + 1] = color;
                newGrid[i, j + 2] = color;
                newGrid[i + 1, j + 2] = color;
                newGrid[i + 2, j + 2] = color;
                newGrid[i + 3, j + 2] = color;
                newGrid[i, j + 3] = color;
                newGrid[i + 1, j + 3] = color;
                newGrid[i + 2, j + 3] = color;
                newGrid[i + 3, j + 3] = color;
            }
        }
    }
}
