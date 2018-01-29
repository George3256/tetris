using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
// using UnityEditor; 
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateShape_Scr : MonoBehaviour
{
    
    GameObject gameObjectBox;
    GameObject gameObjectCreateShape;
    GameObject gameObjectGlassSettings;
    GameObject gameObjectContainerButtonsNext;
    GameObject gameObjectContainerButtonsPrev;

    Slider sliderWidth;
    Slider sliderHeight;
    Slider sliderLevel;


    public Text warning;
    Text textLevel;
    Text textWigth;
    Text textHeight;
    //Text textCurrentNode;

    Button buttonGlass;
    Button buttonPrev;
    Button buttonLoad;
    Button buttonSave;
    Button buttonDelete;
    Button buttonAdd;
    Button buttonPreviewScn;
    Button buttonNextNode;
    Button buttonPreviousNode;


    Button[][] button = new Button[5][];
    Button[][] buttonP = new Button[5][];
    Button[][] buttonN = new Button[5][];

    List<Figure_Scr> listFigure = new List<Figure_Scr>();
    int currentNode = 0;

    Color notPressedButton = Color.white;
    Color pressedButton = Color.green;

    public void onValueChangeSliderSize()
    {
        gameObjectBox.transform.localScale = new Vector3(9.0f + sliderWidth.value / 10, 6.0f, 14.0f + sliderHeight.value / 15);
        textHeight.text = "Высота: " + sliderHeight.value.ToString();
        textWigth.text = "Ширина: " + sliderWidth.value.ToString();
    }
    public void onValueChangeSliderLevel()
    {
        listFigure[currentNode].Level = (int)sliderLevel.value;
        textLevel.text = "Уровень: " + listFigure[currentNode].Level.ToString();
    }
    void displayCurrentNode()
    {
        for (int i = 0; i < listFigure[currentNode].bodyShapeArray.Length; i++)
        {
            BitArray b = new BitArray(listFigure[currentNode].bodyShapeArray[i]);
            for (int j = 0; j < listFigure[currentNode].bodyShapeArray[i].Length; j++)
            {
                if (b[j * 8])
                {
                    button[i][j].image.color = pressedButton;
                }
                else if (!b[j * 8])
                {
                    button[i][j].image.color = notPressedButton;
                }
            }
        }
        if (currentNode + 1 < listFigure.Count)
        {
            gameObjectContainerButtonsNext.SetActive(true);
            displayCurrentNodeN(currentNode + 1);
        }
        else
        {
            gameObjectContainerButtonsNext.SetActive(false);
        }
        if (currentNode > 0)
        {
            gameObjectContainerButtonsPrev.SetActive(true);
            displayCurrentNodeP(currentNode - 1);
        }
        else
        {
            gameObjectContainerButtonsPrev.SetActive(false);
        }
        textLevel.text = "Уровень: " + listFigure[currentNode].Level.ToString();
        sliderLevel.value = listFigure[currentNode].Level;
        warning.text = "Текущая фигура " + (currentNode + 1).ToString() + " из " + listFigure.Count.ToString();
    }
    void displayCurrentNodeN(int index)
    {
        for (int i = 0; i < listFigure[index].bodyShapeArray.Length; i++)
        {
            BitArray b = new BitArray(listFigure[index].bodyShapeArray[i]);
            for (int j = 0; j < listFigure[index].bodyShapeArray[i].Length; j++)
            {
                if (b[j * 8]) buttonN[i][j].image.color = pressedButton;
                else if (!b[j * 8]) buttonN[i][j].image.color = notPressedButton;
            }
        }
    }
    void displayCurrentNodeP(int index)
    {
        for (int i = 0; i < listFigure[index].bodyShapeArray.Length; i++)
        {
            BitArray b = new BitArray(listFigure[index].bodyShapeArray[i]);
            for (int j = 0; j < listFigure[index].bodyShapeArray[i].Length; j++)
            {
                if (b[j * 8]) buttonP[i][j].image.color = pressedButton;
                else if (!b[j * 8]) buttonP[i][j].image.color = notPressedButton;
            }
        }
    }

    void componentInitialization()
    {

        gameObjectBox = GameObject.Find("Box");
        
        string s = File.ReadAllText(Application.persistentDataPath + @"/settings2.tetr");
        string[] str = s.Split(';');

        sliderWidth = GameObject.Find("SliderWidth").GetComponent<Slider>();
        sliderHeight = GameObject.Find("SliderHeight").GetComponent<Slider>();
        sliderLevel = GameObject.Find("SliderLevel").GetComponent<Slider>();
        sliderWidth.value = Int32.Parse(str[0]);
        sliderHeight.value = Int32.Parse(str[1]);
        sliderHeight.onValueChanged.AddListener((delegate { onValueChangeSliderSize(); }));
        sliderWidth.onValueChanged.AddListener((delegate { onValueChangeSliderSize(); }));
        sliderLevel.onValueChanged.AddListener((delegate { onValueChangeSliderLevel(); }));

        gameObjectCreateShape = GameObject.Find("Con1");
        gameObjectGlassSettings = GameObject.Find("Con2");
        gameObjectContainerButtonsNext = GameObject.Find("ContainerButtonsNext");
        gameObjectContainerButtonsPrev = GameObject.Find("ContainerButtonsPrev");

        textLevel = GameObject.Find("TextLevel").GetComponent<Text>();
        textWigth = GameObject.Find("TextWigth").GetComponent<Text>();
        textHeight = GameObject.Find("TextHeight").GetComponent<Text>();

        textHeight.text = "Высота: " + sliderHeight.value.ToString();
        textWigth.text = "Ширина: " + sliderWidth.value.ToString();

        buttonPrev = GameObject.Find("ButtonPrev").GetComponent<Button>();
        buttonGlass = GameObject.Find("ButtonGlass").GetComponent<Button>();
        buttonLoad = GameObject.Find("ButtonLoad").GetComponent<Button>();
        buttonSave = GameObject.Find("ButtonSave").GetComponent<Button>();
        buttonDelete = GameObject.Find("ButtonDelete").GetComponent<Button>();
        buttonAdd = GameObject.Find("ButtonAdd").GetComponent<Button>();
        buttonPreviewScn = GameObject.Find("ButtonPreviewScn").GetComponent<Button>();
        buttonNextNode = GameObject.Find("ButtonNextNode").GetComponent<Button>();
        buttonPreviousNode = GameObject.Find("ButtonPreviousNode").GetComponent<Button>();

        buttonPrev.onClick.AddListener(onClickButtonPrev);
        buttonGlass.onClick.AddListener(onClickButtonGlass);
        buttonLoad.onClick.AddListener(onClickButtonLoadFile);
        buttonSave.onClick.AddListener(onClickButtonSaveFile);
        buttonDelete.onClick.AddListener(onClickButtonDeleteNode);
        buttonAdd.onClick.AddListener(onClickButtonAddNode);
        buttonNextNode.onClick.AddListener(onClickButtonNextNode);
        buttonPreviousNode.onClick.AddListener(onClickButtonPreviousNode);
        buttonPreviewScn.onClick.AddListener(onButtonPreviewScn);

        gameObjectBox.transform.localScale = new Vector3(9.0f + sliderWidth.value / 10, 6.0f, 14.0f + sliderHeight.value / 15);
        gameObjectCreateShape.SetActive(true);
        gameObjectGlassSettings.SetActive(false);
    }

    public void onButtonPreviewScn()
    {
        SceneManager.LoadScene(0);
    }

    public void onClickButtonGlass()
    {
        gameObjectCreateShape.SetActive(false);
        gameObjectGlassSettings.SetActive(true);
    }
    public void onClickButtonPrev()
    {
        string s = sliderWidth.value.ToString() + ";" + sliderHeight.value.ToString() + ";";
        File.WriteAllText(Application.persistentDataPath + @"/settings2.tetr", s);
        gameObjectCreateShape.SetActive(true);
        gameObjectGlassSettings.SetActive(false);
    }

    private void Start()
    {
        populatingAnArrayOfButtons();
        populatingAnArrayOfButtonsNext();
        populatingAnArrayOfButtonsPrev();
        listFigure.Add(new Figure_Scr(1));
        componentInitialization();
        onClickButtonLoadFile();
    }

    void populatingAnArrayOfButtons()
    {
        int k = 0;
        for (int i = 0; i < 5; i++)
        {
            button[i] = new Button[5];
            for (int j = 0; j < 5; j++)
            {
                int z = k;
                button[i][j] = GameObject.Find(k.ToString()).GetComponent<Button>();
                button[i][j].image.color = notPressedButton;
                button[i][j].onClick.AddListener(delegate { onClickButton(z.ToString()); });
                k++;
            }
        }
    }

    void populatingAnArrayOfButtonsNext()
    {
        int k = 0;
        for (int i = 0; i < 5; i++)
        {
            buttonN[i] = new Button[5];
            for (int j = 0; j < 5; j++)
            {
                int z = k;
                buttonN[i][j] = GameObject.Find( k.ToString() + "n").GetComponent<Button>();
                buttonN[i][j].image.color = notPressedButton;
                k++;
            }
        }
    }

    void populatingAnArrayOfButtonsPrev()
    {
        int k = 0;
        for (int i = 0; i < 5; i++)
        {
            buttonP[i] = new Button[5];
            for (int j = 0; j < 5; j++)
            {
                int z = k;
                buttonP[i][j] = GameObject.Find( k.ToString() + "p").GetComponent<Button>();
                buttonP[i][j].image.color = notPressedButton;
                k++;
            }
        }
    }

    public void onClickButton(string str)
    {
        int row = Int32.Parse(str) / 5;
        int cell = Int32.Parse(str) % 5;
        if (button[row][cell].image.color == pressedButton) button[row][cell].image.color = notPressedButton;
        else if (button[row][cell].image.color == notPressedButton) button[row][cell].image.color = pressedButton;
        changeCellValue(row, cell);
    }

    public void onClickButtonNextNode()
    {
        if (validationFigure())
        {
            currentNode++;
            if (currentNode < listFigure.Count) displayCurrentNode();
            else currentNode--;
        }
    }

    public void onClickButtonPreviousNode()
    {
        if (validationFigure())
        {
            currentNode--;
            if (currentNode > -1) displayCurrentNode();
            else currentNode++;
        }
        
    }
    public void onClickButtonAddNode()
    {
        if (validationFigure())
        {
            listFigure.Add(new Figure_Scr(1));
            currentNode = listFigure.Count - 1;
            displayCurrentNode();
        }
    }
    public void onClickButtonDeleteNode()
    {
        if (listFigure.Count == 1)
        {
            currentNode = 0;
            CleanNode();
            displayCurrentNode();
        }
        if (listFigure.Count > 1)
        {
            listFigure.RemoveAt(currentNode);
            if (currentNode>=listFigure.Count)
            currentNode--;
            displayCurrentNode();
        }
    }

    public void CleanNode()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                listFigure[currentNode].bodyShapeArray[i][j] = 0;
            }
        }        
    }

    public void onClickButtonSaveFile()
    {
        if (validationFigure())
        {
            string s = listFigure.Count.ToString() + ";";
            for (int i = 0; i < listFigure.Count; i++)
            {
                s += listFigure[i].ToString();
            }
            File.WriteAllText(Application.persistentDataPath + @"/settings.tetr", s);
        }
    }
    private bool integrityCheck()
    {
        Cell[][] cells = listFigure[currentNode].getCells();
        Cell first=null;
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (cells[i][j].isChecked)
                {
                    first = cells[i][j];
                    break;
                } 
            }
            if (first != null) break;
        }
        if (first == null) return false;
        passAll(first);
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (cells[i][j].isChecked && !cells[i][j].passed) return false;
            }
        }
        return true;
    }

    private bool validationFigure()
    {
        if (!notEmpty())
        {
            warning.text = "Фигура пустая";
            //EditorUtility.DisplayDialog("Проверка правильности", "Фигура пустая", "ok");
            return false;
        }
        if (!integrityCheck())
        {
            warning.text = "Фигура не целостная";
            //EditorUtility.DisplayDialog("Проверка правильности", "Фигура не целостная", "ok");
            return false;
        }
        if (!uniquenessCheck())
        {
            warning.text = "Такая фигура уже есть";
            //EditorUtility.DisplayDialog("Проверка правильности", "Такая фигура уже есть", "ok");
            return false;
        }
        return true;
    }
    private void passAll(Cell cell)
    {
        if (!cell.passed)
        {
            cell.passed = true;
            if (cell.neighBottom != null && cell.neighBottom.isChecked) passAll(cell.neighBottom);
            if (cell.neighLeft != null && cell.neighLeft.isChecked) passAll(cell.neighLeft);
            if (cell.neighTop != null && cell.neighTop.isChecked) passAll(cell.neighTop);
            if (cell.neighRight != null && cell.neighRight.isChecked) passAll(cell.neighRight);
        }
    }
    private bool uniquenessCheck()
    {
        for (int i = 0; i < listFigure.Count; i++)
        {
            if (i != currentNode)
            {
                if (comparison(displacementFigure(i), displacementFigure(currentNode))) return false;
            }
        }
        return true;
    }
    private byte[][] overwrite(int index)
    {
        byte[][] f = new byte[5][];
        for (int i = 0; i < 5; i++)
        {
            f[i] = new byte[5];
            for (int j = 0; j < 5; j++)
            {
                f[i][j] = listFigure[index].bodyShapeArray[i][j];
            }
        }
        return f;
    }

    private bool comparison(byte [][] b1 , byte[][] b2)
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (b1[i][j] % 2 != b2[i][j] % 2) return false;
            }
        }
        return true;
    }
    private bool notEmpty()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if(listFigure[currentNode].bodyShapeArray[i][j] % 2 == 1) return true;
            }
        }
        return false;
    }
    private byte[][] displacementFigure(int index)
    {
        bool flag =true;
        byte[][] f = overwrite(index);
        int k = 0;

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                flag = flag && ((f[i][j] % 2) == 0);
            }
            if (!flag) break;
            k++;
        }

        if (k > 0 && k < 5)
        {
            for (int i = 0; i < 5; i++)
            {
                if (i < 5 - k)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        f[i][j] = f[i + k][j];
                    }
                }
                else
                {
                    for (int j = 0; j < 5; j++)
                    {
                        f[i][j] = 0;
                    }
                }
            }
        }

        k = 0;
        flag = true;

        for (int j = 0; j  < 5 ; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                flag = flag && ((f[i][j] % 2) == 0);
            }
            if (!flag) break;
            k++;
        }

        if (k > 0 && k < 5)
        {
            for (int j = 0; j < 5; j++)
            {
                if (j < 5 - k)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        f[i][j] = f[i][j + k];
                    }
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        f[i][j] = 0;
                    }
                }
            }
        }
        return f;
    }
    public void onClickButtonLoadFile()
    {
        listFigure.Clear();
        currentNode = 0;
        
        string str = File.ReadAllText(Application.persistentDataPath + @"/settings.tetr");
        string[] strArray = str.Split(';');
        for (int i = 0; i < Int32.Parse(strArray[0]); i++)
        {
            listFigure.Add(new Figure_Scr(strArray[i+1]));
            currentNode++;
        }
        currentNode--;
        displayCurrentNode();
    }

    void changeCellValue( int currentRow, int currenCell)
    {
        int indexCurrentCell = 8 * currenCell;

        BitArray b2 = new BitArray(listFigure[currentNode].bodyShapeArray[currentRow]);
        b2[indexCurrentCell] = !b2[indexCurrentCell];
        if (currentRow != 0)
        {
            BitArray b1 = new BitArray(listFigure[currentNode].bodyShapeArray[currentRow - 1]);
            b1[indexCurrentCell + 3] = !b1[indexCurrentCell + 3];
            b1.CopyTo(listFigure[currentNode].bodyShapeArray[currentRow - 1], 0);
        }
        if (currentRow != 4)
        {
            BitArray b3 = new BitArray(listFigure[currentNode].bodyShapeArray[currentRow + 1]);
            b3[indexCurrentCell + 1] = !b3[indexCurrentCell + 1];
            b3.CopyTo(listFigure[currentNode].bodyShapeArray[currentRow + 1], 0);
        }
        if (currenCell != 4)
        {
            b2[indexCurrentCell + 8 + 4] = !b2[indexCurrentCell + 8 + 4];
        }
        if (currenCell != 0)
        {
            b2[indexCurrentCell - 8 + 2] = !b2[indexCurrentCell - 8 + 2];
        }
        b2.CopyTo(listFigure[currentNode].bodyShapeArray[currentRow], 0);
    }
}