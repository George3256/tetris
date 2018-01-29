using System;

public class Figure_Scr
{

    public byte[][] bodyShapeArray { get; set; }
    public int Level { get; set; }
    public Figure_Scr(int lev)
    {
        bodyShapeArray = new byte[][] { new byte[] { 0, 0, 0, 0, 0 },
                                        new byte[] { 0, 0, 0, 0, 0 },
                                        new byte[] { 0, 0, 0, 0, 0 },
                                        new byte[] { 0, 0, 0, 0, 0 },
                                        new byte[] { 0, 0, 0, 0, 0 }};
        Level = lev;
    }
    public Figure_Scr(string str)
    {
        bodyShapeArray = new byte[][] { new byte[5] ,
                                        new byte[5] ,
                                        new byte[5] ,
                                        new byte[5] ,
                                        new byte[5] };
        string[] strArray = str.Split(',');
        Level = Int32.Parse(strArray[0]);
        int k = 1;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                bodyShapeArray[i][j] = Byte.Parse(strArray[k]);
                k++;
            }
        }

    }


    public override string ToString()
    {
        string str = Level.ToString() + ",";
        for (int i = 0; i < bodyShapeArray.Length; i++)
        {
            for (int j = 0; j < bodyShapeArray.Length; j++)
            {
                str += bodyShapeArray[i][j] + ",";
            }
        }
        str += ";";
        return str;
    }

    public Cell[][] getCells()
    {
        Cell[][] res = new Cell[5][];
        int i = 0;
        foreach (byte[] bs in bodyShapeArray)
        {
            res[i] = new Cell[5];
            int j = 0;
            foreach (byte b in bs)
            {
                Cell cell = new Cell(b);
                res[i][j] = cell;
                j++;
            }
            i++;
        }
        ///
        i = 0;
        foreach (Cell[] cs in res)
        {
            int j = 0;
            foreach (Cell cell in cs)
            {
                if (i > 0) cell.neighTop = res[i - 1][j];
                if (j < 4) cell.neighRight = res[i][j + 1];
                if (i < 4) cell.neighBottom = res[i + 1][j];
                if (j > 0) cell.neighLeft = res[i][j - 1];
                j++;
            }
            i++;
        }
        return res;
    }
}
public class Cell
{
    public bool passed { get; set; }
    public bool isChecked { get; set; }
    public Cell neighRight { get; set; }
    public Cell neighBottom { get; set; }
    public Cell neighLeft { get; set; }
    public Cell neighTop { get; set; }

    public Cell(byte b)
    {
        this.isChecked = (b % 2 == 1);
        this.passed = false;
    }
}
