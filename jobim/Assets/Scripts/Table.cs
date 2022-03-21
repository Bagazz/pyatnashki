using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Table : MonoBehaviour
{
    private const int SIZE = 4;

    public event System.Action OnMoveComplete;

    [SerializeField]
    private Cell cellPrefab;

    private Cell[,] table;

    private void Clear()
    {
        var cells  = FindObjectsOfType<Cell>();

        foreach (var cell in cells)
            Destroy(cell.gameObject);

        table = new Cell[SIZE, SIZE];
    }

    private int[,] GenerateTable()
    {
        int[,] table = new int[SIZE, SIZE];

        do
        {
            List<int> numbers = Enumerable.Range(1, 15).ToList();

            for(int i = 0; i < SIZE; i++)
            {
                for (int j = 0; j < SIZE; j++)
                {
                    if (j == SIZE - 1 && i == SIZE - 1) continue;

                    int index = Random.Range(0, numbers.Count);
                    table[j, i] = numbers[index];
                    numbers.RemoveAt(index);
                }
            }
        }
        while (isSolvable(table.Cast<int>().ToArray()));

        return table;
    }

    private bool isSolvable(int[] table)
    {
        int countInversions = 0;

        for (int i = 0; i< table.Length; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (table[j] > table[i]) countInversions++;
                    
            }
        }

        return countInversions % 2 == 0;
    }

    public void Generate()
    {
        Clear();

        int[,] table = GenerateTable();

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                if (table[j, i] == 0) break;

                var cell = Instantiate(cellPrefab, transform);
                cell.transform.position = new Vector3(-j, 0f, i);
                cell.Number = table[j, i];
                this.table[j, i] = cell;
            }
        }
    }

    public void FakeGenerate()
    {
        

        int[,] table = new int[SIZE, SIZE];


        List<int> numbers = Enumerable.Range(1, 15).ToList();

        int index = 0;

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                if (j == SIZE - 1 && i == SIZE - 1) continue;

              
                table[j, i] = numbers[index];
            
                index++;
            }
        }

        Clear();


        table[SIZE - 1, SIZE - 1] = table[SIZE - 2, SIZE - 1];
        table[SIZE - 2, SIZE - 1] = 0;

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                if (table[j, i] == 0) continue;

                var cell = Instantiate(cellPrefab, transform);
                cell.transform.position = new Vector3(-j, 0f, i);
                cell.Number = table[j, i];
                this.table[j, i] = cell;
            }
        }


    }


    public bool TryMove(Cell cell)
    {
        int x = -Mathf.RoundToInt(cell.transform.position.x);
        int y = Mathf.RoundToInt(cell.transform.position.z);

        List<Vector2Int> dxdy = new List<Vector2Int>()
        {
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0)
        };

        for (int i = 0; i< dxdy.Count; i++)
        {
            int xx = x + dxdy[i].x;
            int yy = y + dxdy[i].y;

            if(xx >= 0 && xx < SIZE && yy >= 0 && yy < SIZE)
            {
                if(table[xx, yy] == null)
                {
                    cell.Move(-xx, yy);
                    cell.OnPositionChanged += Cell_OnPositionChanged;
                    return true;
                }
            }
        }

        return false;
    }

    public bool IsSolved()
    {
        if (table[SIZE - 1, SIZE - 1] != null) return false;

        int prev = 0;

        for (int i = 0; i < SIZE; i++)
        {
            for (int j = 0; j < SIZE; j++)
            {
                if (table[j, i] == null) break;

                if(prev > table[j, i].Number) return false;

                prev = table[j, i].Number;

            }
        }
        return true;

    }


    private void Cell_OnPositionChanged(Cell cell, Vector3 prev, Vector3 curr)
    {
        cell.OnPositionChanged -= Cell_OnPositionChanged;
        int x = -Mathf.RoundToInt(prev.x);
        int y = Mathf.RoundToInt(prev.z);
        table[x, y] = null;

         x = -Mathf.RoundToInt(curr.x);
         y = Mathf.RoundToInt(curr.z);
        table[x, y] = cell;

        OnMoveComplete?.Invoke();
    }

    void Start()
    {
        
    }

}
