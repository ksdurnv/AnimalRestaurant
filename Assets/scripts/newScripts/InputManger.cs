//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class InputManger : MonoBehaviour
//{
//    GameInstance gameInstance = new GameInstance();
//    public Transform cameraTrans; // 카메라의 Transform
//    public Transform cameraRange; // 카메라 이동 범위
//    public float maxCameraLocX; // 카메라의 최대 X 위치
//    public float maxCameraLocZ; // 카메라의 최대 Z 위치
//    public float minCameraLocX; // 카메라의 최소 X 위치
//    public float minCameraLocZ; // 카메라의 최소 Z 위치
//    public float cameraSpeed; // 카메라 이동 속도
//    int money; // 현재 돈
//    AnimalManager animalManager; // 동물 관리
//    RestaurantManager restaurantManager; // 레스토랑 관리
//    public UIManager UIManager; // UI 관리
//    Vector2 cameraLoc = new Vector2(0, 0); // 카메라 위치
//    public int Money { get { return money; } set { money = value; UIManager.UpdateMoneyText(Money); } } // 돈 속성

//    public GameObject go;
//    // Start는 첫 프레임 전에 호출됩니다.
//    void Start()
//    {

//        gameInstance.GameIns.uiManager = UIManager;
//        gameInstance.GameIns.inputManager = this;
//        Money = 10000; // 시작할 때 돈을 10000으로 설정
//        restaurantManager = GetComponent<RestaurantManager>(); // RestaurantManager 컴포넌트 가져오기
//        animalManager = GetComponent<AnimalManager>(); // AnimalManager 컴포넌트 가져오기
//        animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // 직원 동물 생성
//        animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // 또 다른 직원 동물 생성
//    }

//    // Update는 매 프레임마다 호출됩니다.
//    void Update()
//    {
//        // if (animalManager.mode == AnimalManager.Mode.GameMode)
//        {
//            // 카메라가 이동 중이라면 입력 무시
//            if (Zoom.isMoving)
//                return;
//            if (Input.touchCount > 0)
//            { 
//                Touch touch = Input.GetTouch(0);



//                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 클릭한 위치에서 Ray 생성
//                RaycastHit hit; // Ray가 충돌한 정보를 담을 변수
//                if (Physics.Raycast(ray, out hit, 5000f)) // Raycast로 충돌 검사
//                {
//                    NextTarget nt = hit.collider.gameObject.GetComponent<NextTarget>(); // NextTarget 컴포넌트 가져오기
//                    if (nt)
//                    {
//                        // 돈이 충분하면 레스토랑 레벨 업
//                        if (money >= nt.money)
//                        {
//                            Money -= nt.money; // 돈 차감
//                            restaurantManager.LevelUp(); // 레벨 업 호출
//                        }
//                    }
//                }

//            }
//            //touch.position;
//           // Camera.main.ScreenPointToRay(touch.position);
//            // 스페이스바를 눌렀을 때 고객 동물 생성
//            if (Input.GetKeyDown(KeyCode.Space))
//            {

//                //  animalManager.SpawnAnimal(AnimalController.PlayType.Customer, new FoodsAnimalsWant());
//            }

//            // 마우스 왼쪽 버튼 클릭 시
//            if (Input.GetMouseButtonDown(0))
//            {
//                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 클릭한 위치에서 Ray 생성
//                RaycastHit hit; // Ray가 충돌한 정보를 담을 변수
//                if (Physics.Raycast(ray, out hit, 5000f)) // Raycast로 충돌 검사
//                {
//                    NextTarget nt = hit.collider.gameObject.GetComponent<NextTarget>(); // NextTarget 컴포넌트 가져오기
//                    if (nt)
//                    {
//                        // 돈이 충분하면 레스토랑 레벨 업
//                        if (money >= nt.money)
//                        {
//                            Money -= nt.money; // 돈 차감
//                            restaurantManager.LevelUp(); // 레벨 업 호출
//                        }
//                    }

//                    MoneyPile pile = hit.collider.gameObject.GetComponent<MoneyPile>();
//                    if (pile)
//                    {
//                        pile.RemoveAllChildren();
//                    }
//                }
//            }

//            // 입력받은 수평 및 수직 축 값
//            float h = Input.GetAxisRaw("Horizontal");
//            float v = Input.GetAxisRaw("Vertical");

//            if (Vector3.zero != new Vector3(h, 0, v))
//            {
//                // 카메라 위치 가져오기
//                Vector3 loc = cameraTrans.position;
//                // 카메라 이동
//                cameraTrans.Translate(new Vector3(h, 0, v).normalized * cameraSpeed * Time.deltaTime, Space.Self);

//                bool isWall = Physics.CheckSphere(cameraRange.position, 1, LayerMask.GetMask("wall"));
//                bool isDoor = Physics.CheckSphere(cameraRange.position, 1, LayerMask.GetMask("door"));
//                if (isWall || isDoor)
//                {
//                    cameraTrans.position = loc; // 원래 위치로 설정
//                }
//            }
//        }
//    }
//}


using CryingSnow.FastFoodRush;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
//using static DG.Tweening.DOTweenModuleUtils;

public class InputManger : MonoBehaviour
{
    GameInstance gameInstance = new GameInstance();
    public Transform cameraTrans; // 카메라의 Transform
    public Transform cameraRange; // 카메라 이동 범위
    public float maxCameraLocX; // 카메라의 최대 X 위치
    public float maxCameraLocZ; // 카메라의 최대 Z 위치
    public float minCameraLocX; // 카메라의 최소 X 위치
    public float minCameraLocZ; // 카메라의 최소 Z 위치
    public float cameraSpeed; // 카메라 이동 속도
    //float money; // 현재 돈
    AnimalManager animalManager; // 동물 관리
    RestaurantManager restaurantManager; // 레스토랑 관리
    public UIManager UIManager; // UI 관리
    Vector2 cameraLoc = new Vector2(0, 0); // 카메라 위치
    //public float Money { get { return money; } set { money = value; UIManager.UpdateMoneyText(Money); } } // 돈 속성
    private Vector3 nowPos, prePos;
    private Vector3 movePos;
    public GameObject go;

    public bool inputDisAble;
    public bool bCleaningMode; //탁자를 치울 수 있는가
 //   public bool a;
//    Vector3 lastPoint = new Vector3();

    public float size;
    public AudioSource audioSource;
    public bool inOtherAction;
    public ParticleSystem particleSystems;
    float zOffset;
    Vector3 lastMousePosition;
    RaycastHit hit;
    public bool manyFingers;

    public Table clickedTable;
    //  bool dragging=false;
    //private Vector3 dragOrigin;
    private void Awake()
    {
        Application.targetFrameRate = 60;//(int)Screen.currentResolution.refreshRateRatio.numerator;
      //  gameInstance.GameIns.uiManager = UIManager;
        gameInstance.GameIns.inputManager = this;
    }
    
   
    // Start는 첫 프레임 전에 호출됩니다.
    void Start()
    {
      
        //Money = 100000; // 시작할 때 돈을 10000으로 설정

        restaurantManager = GetComponent<RestaurantManager>(); // RestaurantManager 컴포넌트 가져오기
        animalManager = GetComponent<AnimalManager>(); // AnimalManager 컴포넌트 가져오기
     //   animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // 직원 동물 생성
        //animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // 또 다른 직원 동물 생성
       // animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // 또 다른 직원 동물 생성
       // animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // 또 다른 직원 동물 생성
       // animalManager.SpawnAnimal(AnimalController.PlayType.Employee, new FoodsAnimalsWant()); // 또 다른 직원 동물 생성
        zOffset = cameraTrans.position.z - Camera.main.transform.position.z;


        originSize = Camera.main.orthographicSize;
    }
    public float dragSpeed = 2.0f;
    public Transform centerObject;       // 화면 중앙 기준 오브젝트 (레이를 쏘는 기준)
    public LayerMask navigationLayer;    // 이동 가능한 영역의 레이어

    private Vector3 dragOrigin;          // 드래그 시작 위치의 월드 좌표
    private bool isDragging = false;
    float diff;
    Vector3 lastDir;
    float dragTimer;
    RaycastHit deltaResult;
    Vector3 dir;
    float reduceSpeed;
    Vector3 targetVector;
    RaycastHit hits;
    Vector3 target;
 //   bool entireStart;
    float doubleClickTimer = 0.2f;
    float lastClick = -1f;
    public Vector3 preLoc;
    public Vector3 curLoc;
    void Update()
    {
        preLoc = curLoc;
        curLoc = cameraRange.position;
        if (!inputDisAble)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                DragScreen_WindowEditor();
                Unlock_T();
                ClickMachine();
            }

            if (Application.platform == RuntimePlatform.Android)
            {
                DragScreen_Android();
                if (clickedTable == null) Unlock();

            }


            DoubleClick();
        
            if (clickedTable != null)
            {
                Vector2 viewLocation;

                if (Application.platform == RuntimePlatform.WindowsEditor) viewLocation = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                else if (Input.touchCount > 0)
                {
                    Touch t = Input.GetTouch(0);
                    viewLocation = Camera.main.ScreenToViewportPoint(t.position);
                }
                else viewLocation = new Vector2(0.5f, 0.5f);

                if (viewLocation.x < 0.01f)
                {
                    //   Debug.Log(v3.x);
                    Vector3 dir = Quaternion.Euler(0, 45, 0) * new Vector3(-1, 0, 0);
                    RayMove(dir, 30);
                }
                if (viewLocation.x > 0.99f)
                {
                    Vector3 dir = Quaternion.Euler(0, 45, 0) * new Vector3(1, 0, 0);
                    RayMove(dir, 30);
                }
                if (viewLocation.y > 0.99f)
                {
                    Vector3 dir = Quaternion.Euler(0, 45, 0) * new Vector3(0, 0, 1);
                    RayMove(dir, 30);
                }
                if (viewLocation.y < 0.01f)
                {
                    Vector3 dir = Quaternion.Euler(0, 45, 0) * new Vector3(0, 0, -1);
                    RayMove(dir, 30);
                }

            }

        }  
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")))
            {
                dragOrigin = hit.point;
            }
            isDragging = false;
        }
    }

    bool RayMove(Vector3 direction, float speed)
    {
        double d = 0;
        float currentSpeed = 0;
        bool check = false;
        Vector3 hitpoibt = new Vector3(0, 0, 0);
        while (d <= 1.01)
        {

            float lerp = Mathf.Lerp(0, speed, (float)d);

           // Debug.DrawLine(cameraRange.position, cameraRange.position + direction * lerp *Time.deltaTime , Color.red, 0.5f);
            RaycastHit h1;
            RaycastHit h2;
            bool isWall = Physics.Raycast(cameraRange.position, direction, out h1, lerp, LayerMask.GetMask("wall"));
            bool isDoor = Physics.Raycast(cameraRange.position, direction, out h2, lerp, LayerMask.GetMask("door"));

            if (!isWall && !isDoor)
            {
                currentSpeed = lerp;
            }
            else
            {
                if (isWall) hitpoibt = h1.point;
                if (isDoor) hitpoibt = h2.point;
               // Debug.DrawLine(cameraRange.position, hitpoibt, Color.red, 0.5f);
                check = true;
                break;
            }
         //   if ((int)(d) == 1) Debug.Log(lerp + " " + speed);
            d += 0.01;
            //Debug.Log(lerp);
        }

        if (check == true)
        {

            Vector3 pos = cameraRange.position;
            Vector3 addPos = pos + direction * currentSpeed;
            Vector3 orginPos = pos + direction * speed * Time.deltaTime;
            if ((pos - hitpoibt).magnitude > (pos - orginPos).magnitude)
            {
                cameraTrans.position += direction * speed * Time.deltaTime;
            }
            else
            {
                cameraTrans.position += direction * currentSpeed * Time.deltaTime;
                return false;
            }
        }
        else
        {
            cameraTrans.position += direction * speed * Time.deltaTime;
        }

        return true;
    }

    float originSize;
    Vector3 deltaMouse;
    Vector3 cameraSpeeds;
    public void DragScreen_WindowEditor(bool draged = false)
    {
        if (Input.GetMouseButtonDown(0) && !inOtherAction || draged)
        {
            GraphicRaycaster ggr = gameInstance.GameIns.uiManager.GetComponent<GraphicRaycaster>();
            EventSystem es = GetComponent<EventSystem>();
         
            PointerEventData ped = new PointerEventData(es);
            ped.position = Input.mousePosition;
            List<RaycastResult> raycastResults = new List<RaycastResult>();
            ggr.Raycast(ped, raycastResults);
            bool chck = false;
            Debug.Log(raycastResults.Count);
            for (int i = 0; i < raycastResults.Count; i++)
            {
                if (raycastResults[i].gameObject.GetComponentInParent<UIManager>())
                {
                    chck = true;
                    break;
                }
            }

            if (!chck)
            {


                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")) || draged)
                {
                    dragOrigin = hit.point;
                    isDragging = true;
                    deltaResult = hit;
                }
                if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, 1 << 11))
                {
                    
                    if (h.collider.GetComponent<Table>() != null && bCleaningMode)
                    { 
                        if (h.collider.GetComponent<Table>().isDirty)
                        {
                          
                            clickedTable = h.collider.GetComponent<Table>();
                            Debug.Log("TrashCan Enabled");
                            gameInstance.GameIns.applianceUIManager.trashCan.SetActive(true);

                            clickedTable.interacting = true;
                            gameInstance.GameIns.workSpaceManager.trashCans[0].throwPlace.SetActive(true);
                            //        originSize = Camera.main.orthographicSize;
                            targetVector = Input.mousePosition;
                        
                         //   StopAllCoroutines();
                           // StartCoroutine(ViewEntireScreen());
                        }
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && !inOtherAction)
        {
            isDragging = false;
            deltaMouse = Input.mousePosition;
        }

        if (isDragging)
        {
            if (clickedTable != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, 1 << 5))
                {
                    clickedTable.trashPlate.transform.position = h.point;
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
                {
                    Vector3 currentPoint = hit.point;
                    Vector3 moveDirection = dragOrigin - currentPoint;
                    dir = new Vector3(moveDirection.x, 0, moveDirection.z);

                    if (RayMove(dir, cameraSpeed))
                    {
                        dragTimer = (deltaResult.point - hit.point).magnitude;

                        diff = cameraSpeed / 2;
                        dragTimer = 1f < dragTimer ? 1f : dragTimer;
                        dragTimer = 0.5f > dragTimer ? 0.5f : dragTimer;
                        reduceSpeed = (cameraSpeed / 2) / dragTimer;
                    }
                    else
                    {

                    }
                    deltaResult = hit;
                }
            }
        }
        else
        {
            if(clickedTable != null && inOtherAction ==false)
            {
                EventSystem es = GetComponent<EventSystem>();
                GraphicRaycaster ggr2 = gameInstance.GameIns.applianceUIManager.GetComponent<GraphicRaycaster>();

                PointerEventData ped2 = new PointerEventData(es);
                ped2.position = Input.mousePosition;
                List<RaycastResult> raycastResults2 = new List<RaycastResult>();
                ggr2.Raycast(ped2, raycastResults2);
                bool chck2 = false;
                for (int i = 0; i < raycastResults2.Count; i++)
                {
                    if (raycastResults2[i].gameObject.GetComponentInParent<ApplianceUIManager>() && raycastResults2[i].gameObject.CompareTag("trashcan"))
                    {

                        chck2 = true;
                        break;
                    }
                }

                if (chck2)
                {
                    Vector3 hh = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                     
                    clickedTable.trashPlate.transform.position = hh;
                    //clickedTable.interacting=true;
                    //   targetVector = Input.mousePosition;
                    clickedTable.CleanTableManually(hh);
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

             
                if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, 1<<11))
                {
                    if (h.collider.GetComponentInParent<TrashCan>() != null)
                    {
                        gameInstance.GameIns.workSpaceManager.trashCans[0].throwPlace.SetActive(false);
                        clickedTable.trashPlate.transform.position = h.point;
                        //clickedTable.interacting=true;
                        //   targetVector = Input.mousePosition;
                        clickedTable.CleanTableManually(h.point);
                        //   if (Physics.Raycast(ray, out RaycastHit hh, Mathf.Infinity, 1 << 5)) clickedTable.CleanTableManually(hh.point);
                    }
                    else
                    {
                        gameInstance.GameIns.workSpaceManager.trashCans[0].throwPlace.SetActive(false);
                        clickedTable.trashPlate.transform.position = clickedTable.plateLoc;
                        clickedTable.interacting = false;
                    }
                
                }
                else
                {
                    gameInstance.GameIns.workSpaceManager.trashCans[0].throwPlace.SetActive(false);
                    clickedTable.trashPlate.transform.position = clickedTable.plateLoc;
                    clickedTable.interacting = false;
                }
                clickedTable = null;
            }
            
            if (dragTimer > 0)
            {
                dragTimer -= Time.deltaTime;
                Vector3 currentPoint = hit.point;
                if (RayMove(dir, diff) == false) dragTimer = 0;
                diff -= Time.deltaTime * reduceSpeed;
            }
        }
    }
    Vector3 tempCameraLoc;
    public IEnumerator BecomeToOrgin()
    {
        float retunSize = Camera.main.orthographicSize;

        double r = Camera.main.orthographicSize;
        float currentSize = (float)r;
        double t = 0;

        Ray ray2 = Camera.main.ScreenPointToRay(targetVector);
        if (Physics.Raycast(ray2, out RaycastHit hitres2, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            target = hitres2.point;
        }
            //    StartCoroutine(MoveCamera());//cameraTrans.DOMove(tempCameraLoc, 5f);
        while (t < 1)
        {
            t += 5 *Time.deltaTime;
            currentSize = Mathf.Lerp((float)r, originSize, (float)t);
            Camera.main.orthographicSize = currentSize;
            Vector3 newLoc;
             Ray ray = Camera.main.ScreenPointToRay(targetVector);
            if (Physics.Raycast(ray, out RaycastHit hitres, Mathf.Infinity, LayerMask.GetMask("Default")))
            { 
                newLoc = hitres.point;
                float s = (target - newLoc).magnitude;
                Vector3 dir = (target - newLoc).normalized;
                if (RayMove(dir, s / Time.deltaTime) == false)
                {
                    Camera.main.orthographicSize = retunSize;
                    //         cameraTrans.position = cV;
                    yield break;
                }

                retunSize = currentSize;
            }
            yield return null;
        }
    }

    IEnumerator MoveCamera()
    {
        bool t = true;
        cameraTrans.DOMove(tempCameraLoc, 0.5f).SetEase(Ease.InBack).OnComplete(() => { t = false; });
        while (t)
        {
            yield return null;
        }
    }

    IEnumerator ViewEntireScreen()
    {
        float retunSize = Camera.main.orthographicSize;

        if(Application.platform == RuntimePlatform.WindowsEditor) targetVector = Input.mousePosition;
        if(Application.platform == RuntimePlatform.Android) targetVector = Input.GetTouch(0).position;
        Ray ray = Camera.main.ScreenPointToRay(targetVector);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
        {
            target = hit.point;
        }
        double r = Camera.main.orthographicSize;
        float currentSize = (float)r;
        double t = 0;
        while (t < 1)
        {
          //      retunSize = currentSize;
            t += 1 * Time.deltaTime;
            currentSize = Mathf.Lerp((float)r, 50, (float)t);

            //     Camera.main.WorldToScreenPoint(targetVector);
          
            Camera.main.orthographicSize = currentSize;
            Vector3 cV = cameraTrans.position;
            Ray ray2 = Camera.main.ScreenPointToRay(targetVector);

            if (Physics.Raycast(ray2, out RaycastHit hit2, Mathf.Infinity, LayerMask.GetMask("Default")))
            {
                Vector3 dir = target - hit2.point;
                float s = dir.magnitude;
                Vector3 d = new Vector3(dir.x, 0, dir.z).normalized;
                //  cameraTrans.Translate(d * Time.deltaTime * 50);
               // Debug.Log(s);
               if (RayMove(d, s/ Time.deltaTime) == false)
                {
                    tempCameraLoc = cameraTrans.position;
                    Camera.main.orthographicSize = retunSize;
           //         cameraTrans.position = cV;
                    yield break;
                }
               else
                {
                    tempCameraLoc = cameraTrans.position;
                }
               //Debug.Log((cV - cameraTrans.position).magnitude);
                //      target = hit2.point;
                //Camera.main.orthographicSize = retunSize;
                //  hit = hit2; 
                //     cameraTrans.position = new Vector3(hit2.point.x,0, hit2.point.z);
                retunSize = currentSize;
            }
          //  yield break;
            yield return null;
        }

    }

    Vector3 lastVector;
    Touch dragTouch;
    void DragScreen_Android()
    {
        if (Input.touchCount > 0 && inOtherAction == false)
        {
            if (Input.touchCount == 1)
            {
                dragTouch = Input.GetTouch(0);
                if (dragTouch.phase == TouchPhase.Began || manyFingers)
                {
                    manyFingers = false;
                    Ray ray = Camera.main.ScreenPointToRay(dragTouch.position);
                    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Default")))
                    {
                        dragOrigin = hit.point;
                        isDragging = true;
                        deltaResult = hit;
                    }

                    if (dragTouch.phase == TouchPhase.Began)
                    { 
                        if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, LayerMask.GetMask("block")))
                        {
                            if (h.collider.GetComponent<Table>() != null)
                            {
                                if (h.collider.GetComponent<Table>().isDirty)
                                {
                                    clickedTable = h.collider.GetComponent<Table>();
                                    clickedTable.interacting = true;
                                    gameInstance.GameIns.workSpaceManager.trashCans[0].throwPlace.SetActive(true);
                                    //        originSize = Camera.main.orthographicSize;
                                    targetVector = dragTouch.position;
                                  //  entireStart = true;
                                    StopAllCoroutines();
                                    StartCoroutine(ViewEntireScreen());
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (clickedTable != null)
                    {
                        lastVector = dragTouch.position;
                        Ray ray = Camera.main.ScreenPointToRay(dragTouch.position);
                        if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, 1 << 5))
                        {
                            clickedTable.trashPlate.transform.position = h.point;
                        }
                    }
                    else
                    {


                        Ray dragray = Camera.main.ScreenPointToRay(dragTouch.position);
                        if (Physics.Raycast(dragray, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
                        {
                            Vector3 currentPoint = hit.point;
                            Vector3 moveDirection = dragOrigin - currentPoint;

                            dir = new Vector3(moveDirection.x, 0, moveDirection.z);

                            if (RayMove(dir, cameraSpeed))
                            {
                                dragTimer = (deltaResult.point - hit.point).magnitude;

                                diff = cameraSpeed / 2;
                                dragTimer = 1 < dragTimer ? 1 : dragTimer;
                                dragTimer = 0.5f > dragTimer ? 0.5f : dragTimer;
                                reduceSpeed = (cameraSpeed / 2) / dragTimer;
                            }
                            else
                            {
                                manyFingers = true;
                            }
                            deltaResult = hit;
                        }
                    }
                }

            }

            if (Input.touchCount == 2 && clickedTable ==null)
            {
                Touch touchZero = Input.GetTouch(0);
                Touch touchOne = Input.GetTouch(1);


                // 이전 프레임에서 각 터치의 위치 차이 계산
                Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                // 이전 및 현재 터치 간의 거리 계산
                float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

                // 두 거리의 차이로 줌 비율 계산
                float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                Camera.main.orthographicSize += deltaMagnitudeDiff * Time.deltaTime;

                if (Camera.main.orthographicSize < 8) Camera.main.orthographicSize = 8;
                else if (Camera.main.orthographicSize > 50) Camera.main.orthographicSize = 50;

            }

            if (Input.touchCount > 1) manyFingers = true;
        }
        else
        {
            if (clickedTable != null && inOtherAction == false)
            {
                Ray ray = Camera.main.ScreenPointToRay(lastVector);
                if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, 1 << 11))
                {
                    if (h.collider.GetComponentInParent<TrashCan>() != null)
                    {
                        clickedTable.trashPlate.transform.position = h.point;
                        //   targetVector = Input.mousePosition;
                        gameInstance.GameIns.workSpaceManager.trashCans[0].throwPlace.SetActive(false);
                        clickedTable.CleanTableManually(h.point);
                        //   if (Physics.Raycast(ray, out RaycastHit hh, Mathf.Infinity, 1 << 5)) clickedTable.CleanTableManually(hh.point);
                    }
                    else
                    {
                        gameInstance.GameIns.workSpaceManager.trashCans[0].throwPlace.SetActive(false);
                        clickedTable.trashPlate.transform.position = clickedTable.plateLoc;
                    }

                }
                else
                {
                    gameInstance.GameIns.workSpaceManager.trashCans[0].throwPlace.SetActive(false);
                    clickedTable.trashPlate.transform.position = clickedTable.plateLoc;
                }
                clickedTable.interacting = false;
                clickedTable = null;
            }

            if (dragTimer > 0)
            {
                dragTimer -= Time.deltaTime;
                Vector3 currentPoint = hit.point;
                if (RayMove(dir, diff) == false) dragTimer = 0;
                diff -= Time.deltaTime * reduceSpeed;
                if(diff < 0) diff = 0;
            }
        }
    }


    void DoubleClick()
    {
       // Input.touchCount == 1
        if (Input.GetMouseButtonDown(0) || Input.touchCount == 1)
        {
            if(Input.touchCount == 1)
            {
                if (Input.GetTouch(0).phase != TouchPhase.Began) return;
            }
            if (Time.time < lastClick + doubleClickTimer)
            {
                if (Application.platform == RuntimePlatform.WindowsEditor) targetVector = Input.mousePosition;
                if (Application.platform == RuntimePlatform.Android) targetVector = Input.GetTouch(0).position;
                bool check = false;
            
                Ray ray = Camera.main.ScreenPointToRay(targetVector);
                if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, LayerMask.GetMask("block")))
                {
                    if (h.collider.GetComponent<Table>() != null)
                    {
                        if (h.collider.GetComponent<Table>().isDirty)
                        {
                            clickedTable = h.collider.GetComponent<Table>();
                            clickedTable.interacting = true;
                            //        originSize = Camera.main.orthographicSize;
                            if(Application.platform == RuntimePlatform.WindowsEditor) targetVector = Input.mousePosition;
                            if(Application.platform == RuntimePlatform.Android) targetVector = Input.GetTouch(0).position;
                         //   entireStart = true;
                            StopAllCoroutines();
                            StartCoroutine(ViewEntireScreen());
                            check = true;
                        }
                    }
                }

                if(!check)
                {
                //    entireStart = false;
                    StopAllCoroutines();
                    StartCoroutine(BecomeToOrgin());
                }
            }
            else lastClick = Time.time;
        }
    }

    UnlockableBuyer currentUnlockableBuyer;

    void Unlock()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, LayerMask.GetMask("LevelUp")))
                {
                    UnlockableBuyer unlockableBuyer = h.collider.gameObject.GetComponent<UnlockableBuyer>();
                    if (unlockableBuyer != null)
                    {
                        currentUnlockableBuyer = unlockableBuyer;
                        unlockableBuyer.MouseDown();
                    }
                }
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, LayerMask.GetMask("LevelUp")))
                {
                    UnlockableBuyer unlockableBuyer = h.collider.gameObject.GetComponent<UnlockableBuyer>();
                    if(unlockableBuyer != currentUnlockableBuyer)
                    {
                        currentUnlockableBuyer.MouseUp();
                        currentUnlockableBuyer = null;
                        isDragging = false;
                    }
                }
                else
                {
                    if (currentUnlockableBuyer != null)
                    {
                        currentUnlockableBuyer.MouseUp();
                        currentUnlockableBuyer = null;
                        isDragging = false;
                    }
                }
            }
        }
        else
        {
            if (currentUnlockableBuyer != null)
            {
                currentUnlockableBuyer.MouseUp();
                currentUnlockableBuyer = null;
                isDragging = false;
            }
        }
    }
    void Unlock_T()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray();
            if (Application.platform == RuntimePlatform.WindowsEditor) ray = Camera.main.ScreenPointToRay(Input.mousePosition);
          
            if (Physics.Raycast(ray, out RaycastHit h, Mathf.Infinity, LayerMask.GetMask("LevelUp")))
            {
                UnlockableBuyer unlockableBuyer = h.collider.gameObject.GetComponent<UnlockableBuyer>();
                if (unlockableBuyer != null)
                {
                    currentUnlockableBuyer = unlockableBuyer;
                    unlockableBuyer.MouseDown();
                }
            }
        }
        else if (Input.GetMouseButton(0))
        {

        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (currentUnlockableBuyer != null)
            {
                currentUnlockableBuyer.MouseUp();
                currentUnlockableBuyer = null;
                isDragging = false;
            }
        }
        else
        {
            if (currentUnlockableBuyer != null)
            {
                currentUnlockableBuyer.MouseUp();
                currentUnlockableBuyer = null;
                isDragging = false;
            }
        }
    }

    // private void OnDrawGizmos()
    // {
    // Gizmos.color = Color.green;
    // Gizmos.DrawWireSphere(cameraRange.position, Camera.main.orthographicSize / 2.5f); //new Vector3(Camera.main.orthographicSize / 2.5f * 2, 0, Camera.main.orthographicSize / 2.5f * 2));
    //  }

    public Vector3 lastLoc;
    private void ClickMachine()
    {
       // Debug.Log(preLoc  + " " + curLoc);

        float test = (preLoc - curLoc).magnitude;
     
        if (Input.GetMouseButtonUp(0) && test < 0.01f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
          
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 13))
            {
                if (hit.collider.gameObject.GetComponentInParent<FoodMachine>() != null)
                {
                    gameInstance.GameIns.applianceUIManager.ShowApplianceInfo(hit.collider.gameObject.GetComponentInParent<FoodMachine>());
                }
            }
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Animal")))
            {
                Animal animal = hit.collider.gameObject.GetComponentInParent<Animal>();
                if (animal)
                {
                    if (animal.GetComponentInChildren<Employee>()!=null)
                    {
                        gameInstance.GameIns.applianceUIManager.ShowPenguinUpgradeInfo(animal.GetComponentInChildren<Employee>());
                    }
                }
            }
        }
    }
    // bool dragging;
    //// Update는 매 프레임마다 호출됩니다.
    //  void Update()
    //  {

    //     //  카메라가 이동 중이라면 입력 무시
    //      if (Zoom.isMoving)
    //          return;
    //      HandleDragging();
    //    //   터치 입력을 처리

    //      if (Application.platform == RuntimePlatform.WindowsEditor)
    //      {
    //    //       마우스 왼쪽 버튼 클릭 시
    //          if (Input.GetMouseButtonDown(0))
    //          {
    //              Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // 클릭한 위치에서 Ray 생성
    //              RaycastHit hit; // Ray가 충돌한 정보를 담을 변수
    //              if (Physics.Raycast(ray, out hit, 5000f)) // Raycast로 충돌 검사
    //              {
    //                  NextTarget nt = hit.collider.gameObject.GetComponent<NextTarget>(); // NextTarget 컴포넌트 가져오기
    //                  if (nt)
    //                  {
    //          //             돈이 충분하면 레스토랑 레벨 업
    //                      if (money >= nt.money)
    //                      {
    //                          Money -= nt.money; // 돈 차감
    //                          restaurantManager.LevelUp(); // 레벨 업 호출
    //                      }
    //                  }

    //                  MoneyPile pile = hit.collider.gameObject.GetComponent<MoneyPile>();
    //                  if (pile)
    //                  {
    //                      pile.RemoveAllChildren();
    //                  }
    //              }
    //          }


    //          if (inOtherAction == false)
    //          {
    //              if (a)
    //              {
    //                  a = false;
    //                  prePos = Input.mousePosition;//Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
    //              }
    //              else
    //              {
    //                  if (Input.GetMouseButtonDown(0))
    //                  {
    //                      RaycastHit hit;
    //                      if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, LayerMask.GetMask("Default")))
    //                      {
    //                          inLine = true;
    //                          audioSource.Play();
    //                          particleSystem.Play();
    //                          prePos = Input.mousePosition; //Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y));
    //                          Vector3 vector3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //                          particleSystem.gameObject.transform.position = new Vector3(hit.point.x, 0.1f, hit.point.z);
    //                          dragging = true;
    //                      }
    //                      else { inLine = false; }
    //                  }

    //                  if (dragging && inLine)
    //                  {

    //                      nowPos = Input.mousePosition;//Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 0, Input.mousePosition.y));
    //                      movePos = (prePos - nowPos).normalized;

    //                      Vector3 dir =  (Quaternion.Euler(0,45,0) * new Vector3(movePos.x, 0, movePos.y)).normalized;

    //                      float mag = (prePos - nowPos).magnitude * cameraSpeed * Time.deltaTime;
    //                      if (movePos != Vector3.zero) Debug.Log(nowPos.x + " " + nowPos.y);
    //                      CheckRay(mag, dir);
    //                      prePos = nowPos;
    //                      RaycastHit hit;
    //                      if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100f, LayerMask.GetMask("Default")))
    //                      {
    //                           particleSystem.gameObject.transform.position = new Vector3(hit.point.x, 0.1f, hit.point.z);
    //                          particleSystem.Pause();
    //                      }
    //                      else
    //                      {
    //                          particleSystem.Clear();
    //                          particleSystem.Stop();
    //                      }
    //                  }
    //                  if (Input.GetMouseButtonUp(0))
    //                  {
    //                      dragging = false;
    //                      particleSystem.Clear();
    //                      particleSystem.Stop();
    //                  }
    //              }

    //          }
    //      }

    //      if (Application.platform == RuntimePlatform.Android)
    //      {
    //          if (Input.touchCount > 0 && inOtherAction == false)
    //          {
    //        //       카메라 이동: 한 손가락 드래그로 구현
    //              if (Input.touchCount == 1)
    //              {
    //                  Touch touch = Input.GetTouch(0);

    //                  if (a)
    //                  {
    //                      prePos = touch.position - touch.deltaPosition;
    //                      a = false;
    //                  }
    //                  else
    //                  {
    //                      if (touch.phase == TouchPhase.Began)
    //                      {
    //                          if (Physics.Raycast(Camera.main.ScreenPointToRay(touch.position), 100f))
    //                          {
    //                              inLine = true;
    //                              audioSource.Play();
    //                              prePos = touch.position - touch.deltaPosition;
    //                          }
    //                          else
    //                          {
    //                              inLine = false;
    //                          }
    //                      }
    //                      else if (touch.phase == TouchPhase.Moved && inLine)
    //                      {

    //                          nowPos = touch.position - touch.deltaPosition;
    //                          movePos = (Vector3)(prePos - nowPos);

    //                          if (movePos != Vector3.zero)
    //                          {
    //                              Vector3 dir = Quaternion.Euler(0, 45, 0) * movePos.normalized;
    //                              float mag = (prePos - nowPos).magnitude;// * Camera.main.orthographicSize / (size + Mathf.Abs(dir.x) * (Screen.height / Screen.width)) * cameraSpeed;
    //                               mag = mag > 50f ? 50f : mag;
    //                              CheckRay(mag, dir);
    //                          }
    //                          prePos = nowPos;
    //                      }
    //                  }
    //              }

    //           //    핀치 줌 구현: 두 손가락으로 확대/축소
    //              if (Input.touchCount == 2)
    //              {
    //                  a = true;
    //                  Touch touchZero = Input.GetTouch(0);
    //                  Touch touchOne = Input.GetTouch(1);


    //                //   이전 프레임에서 각 터치의 위치 차이 계산
    //                  Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
    //                  Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

    //              //     이전 및 현재 터치 간의 거리 계산
    //                  float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
    //                  float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

    //               //    두 거리의 차이로 줌 비율 계산
    //                  float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;



    //                  Camera.main.orthographicSize += deltaMagnitudeDiff * Time.deltaTime;



    //                  if (Camera.main.orthographicSize < 8) Camera.main.orthographicSize = 8;
    //                  else if (Camera.main.orthographicSize > 50) Camera.main.orthographicSize = 50;

    //                  nowPos = prePos;
    //              }
    //          }
    //      }
    //  }
    //  void HandleDragging()
    //  {
    //      if (Input.GetMouseButtonDown(1))
    //      {
    //      //     드래그 시작 위치를 월드 좌표로 변환해서 저장 (오소그래픽)
    //           // movePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,Camera.main.nearClipPlane));

    //         dragOrigin = Input.mousePosition;
    //         movePos = Input.mousePosition;
    //      }

    //      if (Input.GetMouseButton(1))
    //      {
    //         // 드래그 중의 현재 마우스 위치 월드 좌표를 구함 (오소그래픽)
    //         Vector3 currentPoint = Input.mousePosition; //Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane));
    //          Vector3 difference = dragOrigin - currentPoint; // 시작 위치와 현재 위치 차이 계산

    //          Vector3 move = new Vector3(difference.x, difference.y, difference.z);
    //          CheckRay(difference.magnitude * Time.deltaTime * cameraSpeed, move);

    //         dragOrigin = currentPoint;
    //      }
    //  }


    //  void CheckRay(float speed , Vector3 dir)
    //  {
    //      float currentMagnitude = 0;
    //      float divdedSpeed = speed / 1000f;
    //      float f = 0.001f;

    //      Vector3 dirs = new Vector3();
    //      while (f <= 1f) {
    //          dirs = (Quaternion.Euler(0, 45, 0) * new Vector3(dir.x , dir.z, dir.y)).normalized;

    //  //        dirs = cameraTrans.transform.TransformDirection(movePos.x, 0, movePos.y*1.15f);
    //          RaycastHit hit;
    //          RaycastHit hit2;
    //          float lerps =  Mathf.Lerp(divdedSpeed, speed , f);

    //    //       Debug.DrawLine(cameraRange.position, cameraRange.position + dirs * cameraSpeed * Time.deltaTime, Color.red, 0.5f);
    //          bool isWall = Physics.CheckSphere(cameraRange.position + dirs * lerps, 1f, LayerMask.GetMask("wall"));
    //          bool isDoor = Physics.CheckSphere(cameraRange.position + dirs * lerps, 1f, LayerMask.GetMask("door"));
    //          //bool isWall = Physics.Raycast(cameraRange.position, dir, out hit, lerps , LayerMask.GetMask("wall"));
    //          // bool isDoor = Physics.Raycast(cameraRange.position, dir, out hit2, lerps , LayerMask.GetMask("door"));
    //          if (!isWall && !isDoor)
    //          {
    //              currentMagnitude = lerps;
    //           //   cameraTrans.Translate(dir * currentMagnitude * Time.deltaTime, Space.World);
    //          }
    //          else
    //          {
    //          //    Debug.DrawLine(cameraRange.position, cameraRange.position + dirs * lerps , Color.red);
    //              break;
    //          }
    //          f += 0.001f;
    //      }
    //      if (dirs != Vector3.zero)
    //      {
    //          Vector3 origin = cameraRange.position;
    //          Vector3 target = cameraRange.position + dirs * currentMagnitude * Time.deltaTime;
    //          //target -= new Vector3(0, dirs.y * currentMagnitude * Time.deltaTime, 0);

    //          Vector3 pos = cameraTrans.position + dirs * currentMagnitude * Time.deltaTime;
    //           //cameraTrans.position += move;
    //          cameraTrans.Translate(dirs * cameraSpeed * Time.deltaTime, Space.World);
    //       //   cameraRange.position -= new Vector3(0, dir.y * cameraSpeed * Time.deltaTime, 0);
    //              Debug.Log("A");

    //          float player = (new Vector3(cameraRange.position.x, 0, cameraRange.position.z) - new Vector3(origin.x, 0, origin.z)).magnitude;
    //          float t = (new Vector3(target.x,0, target.z) - new Vector3(origin.x,0, origin.z)).magnitude;
    //       //   cameraTrans.Translate(new Vector3(dirs.x, 0, dirs.z)* cameraSpeed * Camera.main.orthographicSize / size * speed * Time.deltaTime, Space.World);
    //         // if ((cameraRange.position - origin).magnitude > (target - origin).magnitude)
    //          {
    //              Debug.Log("A");
    //        //      cameraTrans.position = pos;
    //      //        cameraRange.position -= new Vector3(0, dirs.y * currentMagnitude * Time.deltaTime, 0);
    //          }
    //      }
    //  }
}
