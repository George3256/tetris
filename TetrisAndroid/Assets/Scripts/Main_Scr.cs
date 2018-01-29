using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class Main_Scr : MonoBehaviour
{
    //GUI images:
    Texture2D mainMenu;
    Texture2D pause;
    Texture2D left;
    Texture2D right;
    Texture2D down;
    Texture2D rotate;
    //ALL other:
    public int time;
    public Camera myCamera;
    public int sceneWidth; // ширина игровой области (без учета границ)
    public int sceneHeight; // высота игровой области (без учета границ)
    public Material mainMat; // материал, который будет присвоен кубикам
    public GameObject CubePref; // префаб, который будет играть роль кубика в падающей фигуре
    int[,] _scene; //массив для обозгачения заполненных участков сцены (0 - занято, 1 - свободно)
    public GameObject Box;
    GameObject CubesPivot; //родительский объект для упавших кубиков
    int CubesPivotChilds;
    GameObject secondDot;// количество дочерних объектов в предыдущем объекте (CubesPivot)
    public GameObject scoreScreen;
    public Transform myFirstDot; //первая точка камеры
    public Transform mySecondDot; //вторая точка камеры
    public Text TextLevel;
    public Text TextName;
    public Text TextSc;
    public Text[] TextTop;
    public Text[] TextTopSc;
    private Transform myTarget;
    //[Header("Скорость перемещения камеры")]
    public GameObject can;
    public Slider sliderLevel;
    public Button ButtonNewGame;
    public Button ButtonPrevScene;
    public GameObject text;
    public GameObject text345;
    List<Figure_Scr> listFigure = new List<Figure_Scr>();
    List<Figure_Scr> listFigureLevel = new List<Figure_Scr>();
    int nextFigure; //переменная для случайного выбора следующей фигуры
    Transform actFigure; ////родительский объект для падающих кубиков
    Transform demoFigure;//родительский объект для демонстрационных кубиков
    [HideInInspector]
    public int speed; // скорость падения фигур (1 - 5, 1 - пауза)
    [HideInInspector]
    public Int32 score; //счет игрока
    public bool isPlaying; // переменная для обозначения поражения
    List<Record_Scr> listRecord = new List<Record_Scr>();
    Record_Scr rec;
    public TextMesh testText;
    AudioSource audioSource;

    void Start()//то,что проихойдет на старте сцены
    {
        populatingAnArrayOfTextTop();
        populatingAnArrayOfTextTopSc();
        sliderLevel.onValueChanged.AddListener((delegate { onValueChangeSliderLevel(); }));
        TextLevel.text = "Уровень: " + sliderLevel.value.ToString();
        speed = (int)sliderLevel.value;
        loadFile();
        filtrFigure(speed);
        loadFileUserName();
        displayTop();
        isPlaying = false;
        myTarget = myFirstDot;  //в начале целью камеры является вторая точка
        ButtonNewGame.onClick.AddListener(onClickButtonNewGame);
        ButtonPrevScene.onClick.AddListener(onClickButtonPrevScene);
        secondDot = GameObject.Find("FirstDot");
        Box = GameObject.Find("Box123");
        loadFile();
        loadFileBox();
        rec = new Record_Scr(TextName.text, score);
        Box.transform.localPosition = new Vector3(divideWhole(), Box.transform.localPosition.y, Box.transform.localPosition.z);
        Box.transform.localScale = new Vector3(Box.transform.localScale.x * sceneWidth / 10, Box.transform.localScale.y, Box.transform.localScale.z * sceneHeight / 15);
        mySecondDot.transform.localPosition = new Vector3(((float)sceneWidth), ((float)sceneHeight) / 2, -(sceneHeight + sceneWidth)-7);
        CubesPivot = new GameObject("CubesPivot"); //присваиваем объекту для хранения упавших кубиков пустой объект с именем CubesPivot;
        SceneCreatingFunc(sceneWidth, sceneHeight); // запускаем функцию создания сцены (указываем размеры сцены). В итоге получим заполненный массив _scene с учетом границ
        audioSource = myCamera.GetComponent<AudioSource>();
        isPaused = false;
    }

    void populatingAnArrayOfTextTop()
    {
        int k = 1;
        TextTop = new Text[10];
        for (int j = 0; j < TextTop.Length; j++)
        {
            int z = k;
            TextTop[j] = GameObject.Find("TextTop" + z.ToString()).GetComponent<Text>();
            k++;
        }
    }
    void populatingAnArrayOfTextTopSc()
    {
        int k = 1;
        TextTopSc = new Text[10];
        for (int j = 0; j < TextTopSc.Length; j++)
        {
            int z = k;
            TextTopSc[j] = GameObject.Find("TextTop" + z.ToString() + "sc").GetComponent<Text>();
            k++;
        }
    }
    private void onClickButtonPrevScene()
    {
        SceneManager.LoadScene(0);
    }
    private void onValueChangeSliderLevel()
    {
        speed = (int)sliderLevel.value;
        TextLevel.text = "Уровень: " + sliderLevel.value.ToString();
        filtrFigure(speed);
    }
    private void filtrFigure(int level)
    {
        listFigureLevel.Clear();
        for (int i = 0; i < listFigure.Count; i++)
        {
            if (listFigure[i].Level <= level)
            {
                listFigureLevel.Add(listFigure[i]);
            }
        }
    }

    public bool flag;

    bool isPaused;

    private void onClickButtonNewGame()
    {
        gameStart();
    }

    private float divideWhole()
    {
        float f = 0f;
        if ((sceneWidth % 2) == 1) f = sceneWidth / 2 + 1f;
        else f = sceneWidth / 2 + 0.5f;
        return f;
    }

    private void displayTop()
    {
        string str = File.ReadAllText(Application.persistentDataPath + @"/record.tetr");
        string[] strArray = str.Split(';');
        listRecord.Clear();
        for (int i = 0; i < strArray.Length - 1; i++)
        {
            string[] strArrayRec = strArray[i].Split(',');
            listRecord.Add(new Record_Scr(strArrayRec[0], Int32.Parse(strArrayRec[1])));
        }
        listRecord.Sort(delegate (Record_Scr x, Record_Scr y)
        {
            if (x.Points > y.Points) return -1;
            else if (x.Points < y.Points) return 1;
            else return 0;
        });
        str = "";
        listRecord = listRecord.GetRange(0, 10);
        for (int i = 0; i < 10; i++)
        {
            TextTop[i].text = listRecord[i].Name;
            TextTopSc[i].text = listRecord[i].Points.ToString();
            str += listRecord[i].Name + "," + listRecord[i].Points + ";";
        }
        File.WriteAllText(Application.persistentDataPath + @"/record.tetr", str);
    }
    private void loadFileUserName()
    {
        TextName.text = File.ReadAllText(Application.persistentDataPath + @"/userName.tetr");
    }
    private void loadFile()
    {
        listFigure.Clear();
        if (!File.Exists(Application.persistentDataPath + @"/settings2.tetr"))
        {
            string ss = "";
            File.WriteAllText(Application.persistentDataPath + @"/settings2.tetr", ss);
        }
        string str = File.ReadAllText(Application.persistentDataPath + @"/settings.tetr");
        string[] strArray = str.Split(';');
        for (int i = 0; i < Int32.Parse(strArray[0]); i++)
        {
            listFigure.Add(new Figure_Scr(strArray[i + 1]));
        }
    }
    private void loadFileBox()
    {
        if (!File.Exists(Application.persistentDataPath + @"/settings2.tetr"))
        {
            string ss = "";
            File.WriteAllText(Application.persistentDataPath + @"/settings2.tetr", ss);
        }
        string str = File.ReadAllText(Application.persistentDataPath + @"/settings2.tetr");
        string[] strArray = str.Split(';');
        sceneWidth = Int32.Parse(strArray[0]); // ширина игровой области (без учета границ)
        sceneHeight = Int32.Parse(strArray[1]);
    }

    void Update()// то, что выполняется каждый кадр
    {
        if (!isPaused)
        {
            play();
        }
    }

    void OnGUI()
    {
        if (isPlaying)
        {
            ///variables:
            ///main-menu:
            int mainMenuX = 0;
            int mainMenuY = 0;
            int mainMenuWidth = Screen.width / 8;
            int mainMenuHeight = Screen.height / 5;
            ///pause:
            int pauseX = Screen.width / 8;
            int pauseY = 0;
            int pauseWidth = Screen.width / 8;
            int pauseHeight = Screen.height/5;
            ///down:
            int downX = 0;
            int downY = 2 * Screen.height / 5;
            int downWidth = 3 * Screen.width / 16;
            int downHeight = 3 * Screen.height / 10;
            ///rotate:
            int rotX = 13 * Screen.width / 16;
            int rotY = 2 * Screen.height / 5;
            int rotWidth = 3 * Screen.width / 16;
            int rotHeight = 3 * Screen.height / 10;
            ///left:
            int leftX = 0;
            int leftY = 7 * Screen.height / 10;
            int leftWidth = 3 * Screen.width / 16;
            int leftHeight = 3 * Screen.height / 10;
            ///right:
            int rightX = 13 * Screen.width / 16;
            int rightY = 7 * Screen.height / 10;
            int rightWidth = 3 * Screen.width / 16;
            int rightHeight = 3 * Screen.height / 10;
            //scores:
            int scoresX = 13 * Screen.width / 16;
            int scoresY = 0;
            int scoresWidth = Screen.width / 4;
            int scoresHeight = Screen.height / 5; ;
            //UI:
            //scores:
            GUIStyle scoreStyle = new GUIStyle();
            scoreStyle.fontSize = 25;
            scoreStyle.fontStyle = FontStyle.Bold;
            scoreStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(scoresX, scoresY, scoresWidth, scoresHeight), score.ToString(), scoreStyle);
            //main-menu:
            GUIContent contentMainMenu = new GUIContent();
            contentMainMenu.image = mainMenu;
            if (GUI.Button(new Rect(mainMenuX, mainMenuY, mainMenuWidth, mainMenuHeight), contentMainMenu, scoreStyle))    //в главное меню
            {
                gameLose();
            }
            //pause:
            GUIContent contentPause = new GUIContent();
            contentPause.image = pause;
            if (GUI.Button(new Rect(pauseX, pauseY, pauseWidth, pauseHeight), contentPause, scoreStyle))    //в главное меню
            {
                isPaused = !isPaused;
            }
            //left:
            GUIContent contentLeft = new GUIContent();
            contentLeft.image = left;
            if (GUI.Button(new Rect(leftX, leftY, leftWidth, leftHeight), contentLeft, scoreStyle))
            {
                if (!isPaused && canNotMove(actFigure, Vector3.left) == false)// если функция проверки возможности перемещения выдает false... (тут false означает свободный путь) (Родитель элементов, направление движения)
                {
                    actFigure.position += Vector3.left;// перемещаем фигуру влево на 1
                }
            }
            //right:
            GUIContent contentRight = new GUIContent();
            contentRight.image = right;
            if (GUI.Button(new Rect(rightX, rightY, rightWidth, rightHeight), contentRight, scoreStyle))    //вправо
            {
                if (!isPaused && canNotMove(actFigure, Vector3.right) == false) // если функция проверки возможности перемещения выдает false... (тут false означает свободный путь) (Родитель элементов, направление движения)
                {
                    actFigure.position += Vector3.right; // перемещаем фигуру вправо на 1
                }
            }
            //rotate:
            GUIContent contentRotate = new GUIContent();
            contentRotate.image = rotate;
            if (GUI.Button(new Rect(rotX, rotY, rotWidth, rotHeight), contentRotate, scoreStyle))    //поворот
            {
                if (!isPaused)
                {
                    Transform[] cubes = new Transform[actFigure.childCount]; // объявляем массив, в котором будут все кубики, из которых состоит падающая фигура
                    for (int i = 0; i < cubes.Length; i++) // присваиваем кубики из падающей фигуры массиву
                    {
                        cubes[i] = actFigure.GetChild(i);
                    }
                    if (isCanRot(actFigure, Vector3.back) == false)
                    // если функция проверки возможности поворота выдает false... (тут false означает свободный путь) (фигура для поворота, направление движения)
                    {
                        actFigure.localEulerAngles += Vector3.back * 90;// поворачиваем фигуру
                        for (int i = 0; i < cubes.Length; i++)
                        // это просто поворачивает каждый элемент фигуры таким образом, чтобы он смотрел вверх (как лего)
                        {
                            cubes[i].rotation = Quaternion.Euler(Vector3.right * -90);
                        }
                    }
                }
            }
            //down:
            GUIContent contentDown = new GUIContent();
            contentDown.image = down;
            if (GUI.Button(new Rect(downX, downY, downWidth, downHeight), contentDown, scoreStyle))    
            {
                if (!isPaused)
                {
                    while (canNotMove(actFigure, Vector3.down) == false) //пока фигура может идти вниз, выполняем...
                    {
                        moveFunc(); //функцию движения вниз
                    }
                }
            }
        }
    }

    private void gameStart()
    {
        if (!audioSource.isPlaying) audioSource.Play();
        time = 15;
        can.SetActive(false);
        myTarget = mySecondDot;
        myCamera.transform.position = myTarget.position; //плавное перемещение положения камеры от нынешней точки к цели
        myCamera.transform.transform.rotation = myTarget.rotation;//плавный поворот камеры
        nextFigure = UnityEngine.Random.Range(0, listFigureLevel.Count); // случайным образом выбираем номер следующей фигуры
        FigureSpawnFunc(nextFigure, -Box.transform.localPosition.x, 5, mainMat, true); // запускаем функцию для спавна фигуры (номер фигуры, координаты ее положения, материал, является ли фигуры демонстрационной). Это создаст фигуру, которая выпадет в будущем
        FigureSpawnFunc(UnityEngine.Random.Range(0, listFigureLevel.Count), sceneWidth / 2, sceneHeight, mainMat, false); // это создаст фигуру наверху игрового поля и она начнет падать вниз
        StartCoroutine("MoveDown"); // запускаем корутин для падения фигуры с разной скоростью. }
        isPlaying = true;
        //gui:
        ////main-menu:
        mainMenu = new Texture2D(2, 2);
        mainMenu = Resources.Load<Texture2D>("mainMenu");
        ////pause:
        pause = new Texture2D(2, 2);
        pause = Resources.Load<Texture2D>("pause");
        ////left:
        left = new Texture2D(2, 2);
        left = Resources.Load<Texture2D>("left");
        ////right:
        right = new Texture2D(2, 2);
        right = Resources.Load<Texture2D>("right");
        ////down:
        down = new Texture2D(2, 2);
        down = Resources.Load<Texture2D>("down");
        ////rotate:
        rotate = new Texture2D(2, 2);
        rotate = Resources.Load<Texture2D>("rotate");
    }

    void gameLose()
    {
        //music:
        if (audioSource.isPlaying) audioSource.Stop();
        //camera:
        Application.LoadLevel(Application.loadedLevel);
        myTarget = myFirstDot; //целью является первая точка/*
        myCamera.transform.position = myTarget.position; //плавное перемещение положения камеры от нынешней точки к цели
        myCamera.transform.transform.rotation = myTarget.rotation;//плавный поворот камеры
        //figures off:
        Destroy(GameObject.Find("FigurePivot"));
        Destroy(GameObject.Find("CubesPivot"));
        CubesPivot = new GameObject("CubesPivot");
        //records:
        rec.Points = score;
        File.AppendAllText(Application.persistentDataPath + @"/record.tetr", rec.Name + "," + rec.Points + ";");
        displayTop();
        //score:
        score = 0;
    }

    void play()
    {
        if (isPlaying)
        {
            if (CubesPivotChilds != CubesPivot.transform.childCount) // если изменилось количество упавших кубиков, то...
            {
                CubesPivotChilds = CubesPivot.transform.childCount; // присваиваем переменной, содержащей количество упавших кубиков новое число
                SceneChangerFunc(); //запускаем функцию обновления занятых полей в массиве
            }
            if (canNotMove(actFigure, Vector3.zero) == true) //если кубик при появлении или в какой-то другой момент накрыл собой занятую клетку, то...
            {
                isPlaying = false; //переменная поражения активируется, это включает другой скрипт, который перемещает камеру
                can.SetActive(true);
                gameLose();
            }
        }
    }

    bool defaultTouchCondition()
    {
        Touch touch = Input.GetTouch(0);
        return (touch.deltaTime > 0.1 && touch.phase == TouchPhase.Ended);
    }

    IEnumerator MoveDown() // корутин
    {
        if (!isPaused)
        {
            yield return new WaitForSeconds(1f / (speed)); // ждем 1/(скорость-1) секунд и...
            if (isPlaying) // если игра еще не окончена...
            {
                StartCoroutine("MoveDown");//запускаем корутин на следующее движение вниз
                moveFunc(); // запускаем функцию падения кубика вниз на 1 клетку
            }
        }
    }

    void DeleteRowsFunc()
    { // функция удаления кубиков, которые заполнили всю ширину ряда и сдвиг верхних кубиков вниз

        //НАХОДИМ ТЕ КУБИКИ, КОТОРЫЕ ЗАПОЛНИЛИ СОБОЙ ВСЮ ШИРИНУ
        GameObject[] cubes = new GameObject[CubesPivot.transform.childCount];//объявляем массив для хранения кубиков, которые уже лежат внизу
        for (int i = 0; i < cubes.Length; i++)
        { // присваиваем все упавшие кубики массиву
            cubes[i] = CubesPivot.transform.GetChild(i).gameObject;
        }
        ArrayList rawsNumbers = new ArrayList(); //объявляем массив для хранения номера строки, в которой произошло заполнение
        for (int j = 1; j <= sceneHeight; j++) //перебираем строки от первой (нулевая - граница) до последней по высоте
        {
            int count = 0; // локальная переменная для хранения количества заполенных ячеек в каждой строке

            for (int k = 0; k < cubes.Length; k++) // перебираем все кубики, которые лежат на дне
            {
                if (Mathf.RoundToInt(cubes[k].transform.localPosition.y) == j) // если координа Y у кубика совпадает с номером строки, то это означает, что он лежит на ней и...
                {
                    count++; // к количеству заполненных ячеек в строке добавляем 1
                }
            }
            if (count == sceneWidth) // если количество заполненных ячеек равно ширине поля, то...
            {
                rawsNumbers.Add(j); // запоминаем в массиве номер строки
                score++;//прибавляем к счету 1
            }
        }
        // myMedalScr.spawn(rawsNumbers.Count); //сбрасываем необходимое количество пуговиц

        //УДАЛЯЕМ КУБИКИ И СПУСКАЕМ ОСТАЛЬНЫЕ ВНИЗ
        for (int k = 0; k < cubes.Length; k++) // перебираем все кубики на дне
        {
            int numOfDown = 0; // объявляем локальную переменную, содержащую количество сдвигов кубика вниз на 1 клетку
            for (int i = 0; i < rawsNumbers.Count; i++)
            { //перебираем количество строк, в которых произошло заполнение
                if (Mathf.RoundToInt(cubes[k].transform.localPosition.y) == ((int)rawsNumbers[i])) // если  кубик лежит в этой строке, то...
                {
                    Destroy(cubes[k]); // удаляем кубик
                }
                if (Mathf.RoundToInt(cubes[k].transform.localPosition.y) > ((int)rawsNumbers[i]))//если кубик выше строки, то...
                {
                    numOfDown++; //добавляем к количеству сдвигов вниз 1
                }
            }
            cubes[k].transform.localPosition += Vector3.down * numOfDown; // сдвигаем кубик вниз на нужное количество строк
        }
    }

    void SceneChangerFunc() // функция обновления занятых полей в массиве
    {
        // 1 - свободно 0 - занято
        //ОСВОБОЖДАЕМ ВСЕ ПОЛЕ
        for (int x = 1; x <= sceneWidth; x++) // перебираем все поля по X
        {
            for (int y = 1; y <= sceneHeight; y++) // перебираем все поля по Y
            {
                _scene[x, y] = 1; //выбранная клетка равна 1
            }
        }

        //ЗАПОЛНЯЕМ ПОЛЕ УПАВШИМИ КУБИКАМИ 
        Vector3[] cubesPos = new Vector3[CubesPivotChilds]; // массив для хранения координат кубиков
        for (int i = 0; i < cubesPos.Length; i++)
        { // перебираем все кубики
            cubesPos[i] = CubesPivot.transform.GetChild(i).localPosition; // присваиваем координату кубика массиву, это можно не делать но ниже придется написать это if ((Mathf.RoundToInt(CubesPivot.transform.GetChild(i).localPosition.x) == x) && (Mathf.RoundToInt(CubesPivot.transform.GetChild(i).localPosition.y) == y))
            for (int x = 1; x <= sceneWidth; x++) // перебираем поля по X
            {
                for (int y = 1; y <= sceneHeight; y++) // перебираем поля по Y
                {
                    if ((Mathf.RoundToInt(cubesPos[i].x) == x) && (Mathf.RoundToInt(cubesPos[i].y) == y)) //если положение кубика совпадает с координатой клетки, то...
                    {
                        _scene[x, y] = 0; // эта клетка занята
                    }
                }
            }
        }
    }

    void FigureSpawnFunc(int figNum, float figureXPos, float figureYPos, Material mat, bool isNextFig) //функция спавна фигуры (номер фигурыб положение по X, положение по Y, материал для кубиков фигуры, является ли фигура демонстрационной)
    {
        Color col = Color.black;


        switch (figNum % 7)
        {
            case 0:
                col = Color.red;
                break;
            case 1:
                col = Color.blue;
                break;
            case 3:
                col = Color.green;
                break;
            case 4:
                col = Color.white;
                break;
            case 5:
                col = Color.cyan;
                break;
            case 6:
                col = Color.yellow;
                break;
            case 7:
                col = Color.magenta;
                break;
        }


        GameObject figure = new GameObject("FigurePivot"); //создаем пустой объект, который будет родителем для кубиков (элементов) фигуры

        figure.transform.position = new Vector3(figureXPos, figureYPos, 0);

        for (int i = 0; i < listFigureLevel[figNum].bodyShapeArray.Length; i++)
        {
            BitArray b = new BitArray(listFigureLevel[figNum].bodyShapeArray[i]);
            for (int j = 0; j < listFigureLevel[figNum].bodyShapeArray[i].Length; j++)
            {
                if (b[j * 8]) CubeCreatingFunc(-2 + i, -2 + j, col, figure.transform);
            }
        }

        if (isNextFig == false) //если фигура не для демонстрации, то...
        {
            actFigure = figure.transform;
        }
        else
        { //иначе, если фигура демонстрационная, то...
            figure.AddComponent<Rigidbody>(); //придаем физику фигуре
            demoFigure = figure.transform;
        }
    }

    void moveFunc()
    { //функция падения фигуры вниз
        Transform[] cubes = new Transform[actFigure.transform.childCount];//локальная переменная для хранения кубиков (элементов фигуры)
        for (int i = 0; i < cubes.Length; i++)
        {//перебираем каждый кубик
            cubes[i] = actFigure.transform.GetChild(i);//присваиваем каждый кубик массиву
        }
        if (canNotMove(actFigure.transform, Vector3.down) == true) //если кубик НЕ может двигаться вниз, то...
        {
            for (int i = 0; i < cubes.Length; i++) //перебираем каждый кубик в фигуре
            {
                cubes[i].SetParent(CubesPivot.transform); //делаем кубик дочерним к элементу, содержащему все кубики, которые лежат внизу
            }
            Destroy(actFigure.gameObject); //уничтожаем элемент фигуру (пустой объект)
            DeleteRowsFunc(); //вызываем функцию удаления кубиков, которые заполнили ряд
            SceneChangerFunc();//вызываем функцию для обновления занятых и пустых мест на поле
            Destroy(demoFigure.gameObject);
            FigureSpawnFunc(nextFigure, sceneWidth / 2, sceneHeight, mainMat, false);//спавним новую фигуру сверху
            nextFigure = UnityEngine.Random.Range(0, listFigureLevel.Count);//случайног выбираем номер следующей фигуры
            FigureSpawnFunc(nextFigure, -Box.transform.localPosition.x, 5, mainMat, true);//спавним демонстрационную фигуру
        }
        else
        { //иначе, если фигура может двигаться вниз, то...
            actFigure.transform.position += Vector3.down; //двигаем фигуру вниз на 1
        }
    }

    void SceneCreatingFunc(int width, int height) // функция создания поля для игры
    {
        _scene = new int[width + 2, height + 4]; //создаем двумерный массив (+2 - учет границ по бокам, +4 - учет границы снизу 
        //и высоты фигуры)
        for (int x = 0; x < _scene.GetLength(0); x++) //перебираем все поля по X (с учетом границ)
        {
            for (int y = 0; y < _scene.GetLength(1); y++)//перебираем все поля по Y (с учетом границ)
            {
                if ((x > 0) && (x < _scene.GetLength(0) - 1) && (y != 0))//если клетка по X между 0 границей и по Y не ноль, то...
                {
                    _scene[x, y] = 1;//клетка равна 1 (значит свободна)
                }
                else
                { //иначе...
                    _scene[x, y] = 0; //клетка равно 0 (занята)
                }
            }
        }
    }

    void CubeCreatingFunc(float x, float y, Color color, Transform parent)//функция создания кубика (координаты, цвет, родитель)
    {
        GameObject cube = Instantiate(CubePref);//создаем новый объект из префаба
        cube.transform.parent = parent;//делаем его дочерним радителю
        cube.transform.localPosition = new Vector3(x, y, 0);//настраиваем координты
        cube.GetComponent<Renderer>().material.color = color;//даем цвет
    }
    bool canNotMove(Transform figure, Vector3 direction) //функция проверки на передвижение (кубики (массив), направление)
    {
        Vector3[] cubes = new Vector3[figure.childCount];//локальная переменная для хранения положения кубиков - элементов массива
        for (int i = 0; i < cubes.Length; i++)// перебираем каждый кубик
        {
            cubes[i] = figure.GetChild(i).position;
            //если в том направлении, куда направится кубик на поле сцены стоит 0, то вернуть True, т.е. идти нельзя
            if (_scene[Mathf.RoundToInt(cubes[i].x + direction.x), Mathf.RoundToInt(cubes[i].y + direction.y)] == 0) return true;
        }

        return false; //вернуть False, т.е. идти можно
    }
    bool isCanRot(Transform pivot, Vector3 direction)//функция проверки фигуры на поворот (фигура, направление)
    {
        pivot.localEulerAngles += direction * 90; // поворачиваем фигуру в нужном направлении на 90 градусов
        Vector3[] cubes = new Vector3[pivot.childCount]; // объявляем массив для хранения положения кубиков - элементов фигуры
        for (int i = 0; i < cubes.Length; i++)//перебираем кубики
        {
            cubes[i] = pivot.GetChild(i).position; //присваиваем кубик массиву
            if (_scene[Mathf.RoundToInt(cubes[i].x), Mathf.RoundToInt(cubes[i].y)] == 0) // если после поворота кубик находится на клетке сцены, которая занята, то...
            {
                pivot.localEulerAngles -= direction * 90; //поврачиваем фигуру обратно
                return true; //вернуть True, т.е.поврот запрещен
            }
        }
        pivot.localEulerAngles -= direction * 90; // поворачиваем фигуру обратно
        return false; //возвращаем false, т.е. поворот разрешен
    }
}