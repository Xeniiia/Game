using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//Переход звука пройденного уровня на следующий уровень, без его уничтожения 
//при смене сцены
//если на сцене уже существует такой звук - удалить тот, что "перенесся"
//с нами на следующую сцену и оставить тот, что уже был на сцене
//(чтобы в компонентах этот звук не исчез и мог проигрываться)
//  !чтобы при переходе на следующую сцену, где уже сущетсвует этот звук, старый проигрывался, а не уничтожался - 
//  !каждую вторую сцену тэг для звука меняется (для нечетных сцен один тэг, для четных - другой)
//  !тогда он будет успевать проигрываться и уничтожаться через одну сцену
public class LongPassLevel : MonoBehaviour
{
    [Header("Tags")]
    [SerializeField] private string Tag;

    //Тэг для 1-ой, 3-ей, 5-ой и т.д. сцен - LongPassLevel_NECHET
    //Тэг для 2-ой, 4-ей, 6-ой и т.д. сцен - LongPassLevel_CHET

    private void Awake()
    {
            GameObject obj = GameObject.FindWithTag(this.Tag); //Находит объект по ранее прописанному в компоненте тегу
                                                               //и создает ссылку на этот объект
            if (obj)                                           //Если объект существует
            {                                                  //
                Destroy(obj);                                  //Уничтожить его
            }
            else                                               //Иначе, если объект не сущетсвует
            {                                                  //
                this.gameObject.tag = this.Tag;                //Дать obj ранее прописанный в компоненте тег
                DontDestroyOnLoad(this.gameObject);            //Вызвать для него метод, не уничтожающий его при смене сцен
            }
    }

}
