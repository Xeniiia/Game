using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPause = false; //Переменная, определяющая, активно ли меню паузы
    public GameObject pauseMenuUI;          //Ссылка на объект Panel

    public void Awake()                                     //Метод, вызывающийся во время загрузки экземпляра сценария
    {                                                       //
        if (SceneManager.GetActiveScene().name != "menu")   //Если текущая сцена не меню
        {                                                   //
            Cursor.visible = false;                         //Скрыть курсор
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))               //если нажимается кнопка Esc
        {
            if (GameIsPause)                                //если игра уже на "паузе"
            {                                               //
                Resume();                                   //продолжить (метод ниже)
            }                                               //
            else                                            //если игра не на "паузе"
            {                                               //
                Pause();                                    //поставить паузу (метод ниже)
            }
        }
    }

    public void Resume()                        //метод продолжения игры после паузы
    {
        Cursor.visible = false;                 //скрыть курсор
        pauseMenuUI.SetActive(false);           //скрыть меню паузы (Panel)
        Time.timeScale = 1f;                    //возвращает игру к оригинальной скорости
        GameIsPause = false;                    //игра не на паузе -> установим переменной значение ложь
    }

    void Pause()                                //метод остановки игры при вызове паузы
    {
        pauseMenuUI.SetActive(true);            //вызвать меню паузы (Panel)
        Cursor.visible = true;                  //показать курсор
        Time.timeScale = 0f;                    //останавливает игру (где 1 - оригинальная скорость)
        GameIsPause = true;                     //игра на паузе -> установим переменной значение истины
    }

    public void LoadMenu()                      //метод загрузки сцены меню
    {
        Time.timeScale = 1f;                    //возвращает игру к оригинальной скорости
        SceneManager.LoadScene(0);              //загрузка нужной сцены
    }
}
