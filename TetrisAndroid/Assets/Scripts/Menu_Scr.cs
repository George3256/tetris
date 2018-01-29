using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class Menu_Scr : MonoBehaviour
{

    public InputField inputField;

    private void Start()
    {
        if (!File.Exists(Application.persistentDataPath + @"/settings2.tetr"))
        {
            string ss = "7;10";
            File.WriteAllText(Application.persistentDataPath + @"/settings2.tetr", ss);
        }
        if (!File.Exists(Application.persistentDataPath + @"/settings.tetr"))
        {
            string ss = "8;3,0,8,8,0,0,4,5,25,24,0,0,6,7,17,16,0,0,2,2,0,0,0,0,0,0,;3,0,0,8,8,0,0,12,13,17,16,4,5,19,18,0,0,2,2,0,0,0,0,0,0,0,;1,0,0,8,0,0,0,12,9,24,0,4,5,23,17,16,0,2,2,2,0,0,0,0,0,0,;1,0,0,8,0,0,0,4,9,16,0,0,4,11,16,0,0,4,11,16,0,0,4,3,16,0,;1,0,0,0,0,0,0,0,8,0,0,0,4,1,16,0,0,0,2,0,0,0,0,0,0,0,;1,0,0,0,0,0,0,8,8,0,0,4,13,25,16,0,4,7,19,16,0,0,2,2,0,0,;5,0,0,8,0,0,0,4,9,16,0,0,12,11,16,0,4,5,19,16,0,0,2,2,0,0,;5,0,0,8,0,0,0,4,9,16,0,0,4,11,24,0,0,4,7,17,16,0,0,2,2,0,;";
            File.WriteAllText(Application.persistentDataPath + @"/settings.tetr", ss);
        }
    }

    public void onClickButtonPlay()
    {
        string s = inputField.text;
        File.WriteAllText(Application.persistentDataPath + @"/userName.tetr", s);
        SceneManager.LoadScene(1);
        if (!File.Exists(Application.persistentDataPath + @"/record.tetr"))
        {
            string str = " ,0; ,0; ,0; ,0; ,0; ,0; ,0; ,0; ,0; ,0; ,0; ,0;";
            File.WriteAllText(Application.persistentDataPath + @"/record.tetr", str);
        }
    }
    public void onClickButtonAdmin()
    {
        SceneManager.LoadScene(2);
    }
    public void onClickButtonExit()
    {
        Application.Quit();
    }
}
