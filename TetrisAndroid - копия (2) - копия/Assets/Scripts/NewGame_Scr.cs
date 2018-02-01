using UnityEngine;
using System.Collections;

public class NewGame_Scr : MonoBehaviour {

    void OnMouseEnter() { //если мышка зашла на коллайдер
        transform.localScale += new Vector3(0.001f,0.001f,0.001f);
    }

    void OnMouseExit()//если мышка сошла с коллайдера
    {
        transform.localScale -= new Vector3(0.001f, 0.001f, 0.001f);
    }

    void OnMouseUp() {//если игрок отпустил кнопку мыши над коллайдером
        Application.LoadLevel(Application.loadedLevel);
    }
}
