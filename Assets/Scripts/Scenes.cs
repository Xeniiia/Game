using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;              //Подключение библиотеки для управления сценами

public class Scenes : MonoBehaviour
{
    private Animator anim;
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "menu")                       //Если сцена Меню - удаляет все звуки прохождения уровня, созданные ранее
        {
            Cursor.visible = true;                                              //Показать курсор
            GameObject obj = GameObject.FindWithTag("LongPassLevel_NECHET");    //Поиск объекта с тэгом LongPassLevel_NECHET
            GameObject obj2 = GameObject.FindWithTag("LongPassLevel_CHET");     //Поиск объекта с тэгом LongPassLevel_CHET
            Destroy(obj);                                                       //Удаление этих объектов
            Destroy(obj2);
        } 
        anim = GetComponent<Animator>();                                        //Получение компонента Animator
    }
    public void ChangeScenes(int numberScenes)  //Функция загрузки сцены, в нее передается номер загружаемой сцены
    {                                           //
        SceneManager.LoadScene(numberScenes);   //Загрузить сцену с номером numberScenes
    }

    public void Exit ()                         //Функция выхода из приложения
    {
        Application.Quit();
    }
    public void FadeToLevel()                   //Функция анимации затемнения при загрузки меню                       
    {
        anim.SetTrigger("fade");
    }
}
