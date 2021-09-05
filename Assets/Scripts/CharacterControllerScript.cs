using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterControllerScript : MonoBehaviour
{
    [Header("Character")]
    Rigidbody2D rigidBody;        //компонент RigidBody2D персонажа
    public float moveSpeed = 10f; //скорость передвижения персонажа
    Animator anim;                //компонент Animator

    [Header("Rotate")]
    bool isFacingRight = true;    //переменная, определяющая, повернут ли персонаж сейчас вправо

    [Header("Jump on ground")]
    bool isGrounded = false;            //Определяет, на земле ли персонаж
    public Transform GroundCheck;       //Ссылка на соприкосновение с землей
    readonly float groundRadius = 0.1f; //Радиус определения
    public LayerMask whatIsGround;      //Ссылка на слой


    [Header("Wall Jump")]
    public float wallJumpTime = 0.2f;    //время прыжка от стены
    public float wallSlideSpeed = 0.3f;  //скорость скольжения персонажа по стене
    public float wallDistance = 0.5f;    //на каком расстоянии должна быть стена,
                                         //чтобы персонаж скользил и мог сделать прыжок

    public bool isWallSliding = false;   //переменная, определяющая на стене ли сейчас персонаж
    RaycastHit2D WallCheckHit;           //Ссылка на объект RaycastHit2D, определяющий, есть ли рядом стена
    float jumpTime;                      //время прыжка
    public LayerMask WallForJump;        //ссылка на слой

    [Header("Dash")]
    public float dashDistance = 15f;     //дистанция дэша
    public bool isDashing;               //переменная, определяющая, происходит ли дэш

    [Header("Dead Space")]
    public float SX;                     //координаты начала уровня
    public float SY;

    [Header("Audio")]
    [SerializeField] private AudioSource Jumping;       //ссылка на звук прыжка
    [SerializeField] private AudioSource Death;         //ссылка на звук смерти
    [SerializeField] private AudioSource PassLevel;     //ссылка на звук пройденного уровня
    [SerializeField] private AudioSource Walk;          //ссылка на звук движения
    [SerializeField] private AudioSource DashAudio;     //ссылка на звук дэша

    //--------------------------------конец инициализации переменных-----------------------------------------------

    private void Start()
    {
        SX = transform.position.x;                  //присваиваем координатам начала уровня те, на которых
        SY = transform.position.y;                  //находится персонаж при старте
        anim = GetComponent<Animator>();            //получаем компонент Animator
        rigidBody = GetComponent<Rigidbody2D>();    //получаем компонент RigidBody2D
    }

    private void FixedUpdate()
    {
        Run();

        //Определение, на земле ли персонаж:
        isGrounded = Physics2D.OverlapCircle(GroundCheck.position, groundRadius, whatIsGround); //переменная, определяющая пересечение
                                                                                                //объекта GroundCheck с объектом на слое whatIsGround

        anim.SetBool("Ground", isGrounded);                         //Изменяется переменная в Animator соответсвенно находится персонаж на земле сейчас или нет
        anim.SetFloat("vSpeed", rigidBody.velocity.y);              //Изменяется переменная в Animator соотвественно движению персонажа
                                                                    //по вертикали
        if (!isGrounded) return;                                    //Если персонаж прыгнул - выход из передвижения
    }

    //Движение
    public void Run()
    {
        if (!isDashing) { 
        float move = Input.GetAxis("Horizontal");               //значение move - движение персонажа по горизонтали,
                                                                //где -1 влево, 1 вправо, а 0 состояние покоя
        anim.SetFloat("Speed", Mathf.Abs(move));                //Изменяется переменная в Animator соотвественно модулю
                                                                //движения персонажа по горизонтали
            rigidBody.velocity = new Vector2(move * moveSpeed, rigidBody.velocity.y);   //Задается скорость в виде вектора в направлениях x, y

            if (Input.GetKeyDown(KeyCode.A)) Walk.Play();       //Если зажата кнопка A или D - воспроизвести звук движения
            else if (Input.GetKeyUp(KeyCode.A)) Walk.Stop();    //иначе остановить его
            if (Input.GetKeyDown(KeyCode.D)) Walk.Play();
            else if (Input.GetKeyUp(KeyCode.D)) Walk.Stop();

            Flip();
        }
    }

    //Поворот персонажа в сторону движения
    void Flip() 
    {
        if (Input.GetAxis("Horizontal") > 0)                        //Если происходит перемещение в сторону
                                                                    //и значение больше 0 (то есть движение вправо)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);    //присвоить значения Rotation для объекта 0 по Х, 0 по У и 0 по Z
                                                                    //(т.е. стандартное положение спрайта)
            isFacingRight = true;                                   //
        }
        if (Input.GetAxis("Horizontal") < 0)                        //Если нажата кнопка перемещения в сторону
                                                                    //и значение меньше 0 (то есть движение влево)
        {
            transform.localRotation = Quaternion.Euler(0, 180, 0);  //присвоить значения Rotation для объекта 0 по Х, 180 по У и 0 по Z
                                                                    //т.е. зеркально отражение по горизонтали спрайта)
            isFacingRight = false;                                  //
        }
    }

    private void Update()
    {
        //Dash Left
        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.A))     //Если нажата клавиша движения А и происходит нажатие Левого шифта
        {
                anim.SetBool("isDashing", true);                                //параметр в Animator isDashing присвоить истину
                                                                                //(для проигрывания анимации по условию isDashing==1)
                StartCoroutine(Dash(-1f));                                      //вызов сопрограммы (корутина),
                                                                                //принимающего в качестве значения IEnumerator 
        }

        //Dash Right - аналогично Dash Left
        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKey(KeyCode.D))
        {
                anim.SetBool("isDashing", true);
                StartCoroutine(Dash(1f));
        }


        //Wall Jump
        if (isFacingRight)                                                      //Если персонаж повернут вправо
        {                                                                       //WallCheckHit - перемененная, определяющая, есть ли рядом стена
            WallCheckHit = Physics2D.Raycast(transform.position,                //С помощью Raycast бросается "луч" из координат персонажа
                                            new Vector2(wallDistance, 0),       //в горизонтальном направлении
                                            wallDistance,                       //на максимальное расстояние WallDistance
                                            WallForJump);                       //с фильтром для обнаружения только коллайдеров на слое WallForJump
        } 
        else                                                                    //Если персонаж повернут влево
        {                                                                       //то же самое, но в противоположную сторону
            WallCheckHit = Physics2D.Raycast(transform.position, 
                                            new Vector2(-wallDistance, 0), 
                                            wallDistance, 
                                            WallForJump);
        }

        if (WallCheckHit && !isGrounded && Input.GetAxis("Horizontal") != 0)    //Если рядом стена, персонаж в воздухе и движется по горизонтали
        {                                                                       //
            isWallSliding = true;                                               //Персонаж "скользит"
            jumpTime = Time.time + wallJumpTime;                                //Время прыжка = текущее время с запуска приложения + время для прыжка от стены
        }                                                                       //
        else if (jumpTime < Time.time)                                          //Иначе если время прыжка меньше текущего (истекло)
        {                                                                       //
            isWallSliding = false;                                              //Персонаж "не скользит"
        }                                                                       //

        if (isWallSliding)                                                      //Если персонаж "скользит", его скорость по вертикали
        {                                                                       //не ниже wallSlideSpeed и не выше float.MaxValue
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, Mathf.Clamp(rigidBody.velocity.y, wallSlideSpeed, float.MaxValue));
        }
        if (isWallSliding && Input.GetButtonDown("Jump"))                       //Если персонаж "скользит" и нажимается клавиша прыжка
        {                                                                       //
            rigidBody.AddForce(new Vector2(0, 190));                            //Придаем силу по вертикали
            Jumping.Play();                                                     //Звук прыжка
        }
        ////////////

        
        //JUMP
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))      //Если "на земле" и зажата клавиша Space
        {
            if (!isDashing)
            {
                anim.SetBool("Ground", false);                  //Изменение переменной в Animator (персонаж уже не на земле)
                rigidBody.AddForce(new Vector2(0, 220));        //Придаем персонажу силу прыжка с помощью вектора скорости
                Jumping.Play();
            }
        }
    }

    //Dead
    private void OnCollisionEnter2D(Collision2D collision)      //Функция, вызываемая при пересечении коллайдеров
    {
        if (collision.gameObject.name == "DeadSpace")           //Если персонаж пересек коллайдер с именем DeadSpace
        {
            Death.Play();                                       //Звук смерти
            transform.position = new Vector3(SX, SY, transform.position.z); //Возврат на начальные координаты
        }
        if (collision.gameObject.name == "NextLevel")           //Если персонаж пересек коллайдер с именем NextLevel
        {
            PassLevel.Play();                                   //Звук окончания уровня
            int num = SceneManager.GetActiveScene().buildIndex; //Запоминаем номер текущей сцены
            SceneManager.LoadScene(num+1);                      //Загружаем сцену под следующим номером
        }
        if (collision.gameObject.name == "NextLevel2")          //Если персонаж пересек коллайдер с именем NextLevel2
        {                                                       //(в данном случае это означает конец последнего уровня в игре)
            PassLevel.Play();                                   //Звук окончания уровня
            SceneManager.LoadScene("menu");                     //Загрузка меню
        }
    }


    IEnumerator Dash (float direction)
    {
        isDashing = true;                                                                   //Переменная, определяющая, что персонаж делает дэш - истина:
        rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0f);                         //Придадим персонжау движение вектором по горизонтали
        rigidBody.AddForce(new Vector2(dashDistance * direction, 0f), ForceMode2D.Impulse); //Добавим мгновенный импульс силы, учитывая его массу
        DashAudio.Play();                                                                   //Воспроизвести звук дэша
        float gravity = rigidBody.gravityScale;                                             //Запоминаем значение гравитации объекта
        rigidBody.gravityScale = 0;                                                         //обнуляем
        yield return new WaitForSeconds(0.4f);                                              //ждем 0.4 секунды
        isDashing = false;                                                                  //Переменная, определяющая, что персонаж делает дэш - ложь:
        anim.SetBool("isDashing", false);                                                   //Изменение переменной в Animator
        rigidBody.gravityScale = gravity;                                                   //Возвращаем объекту значение гравитации
    }
}
