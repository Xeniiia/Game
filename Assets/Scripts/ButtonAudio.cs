using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAudio : MonoBehaviour
{
    public AudioSource myFx;    //ссылка на AudioSource
    public AudioClip hoverFx;   //ссылка на AudioClip
    public AudioClip clickFx;   //ссылка на AudioClip

    public void HoverSound()    //Звук при наведении
    {
        myFx.PlayOneShot(hoverFx); //Проиграть один раз
    }

    public void ClickSound()    //Звук при нажатии
    {
        myFx.PlayOneShot(clickFx); //Проиграть один раз
    }
}
