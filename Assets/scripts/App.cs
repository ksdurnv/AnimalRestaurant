using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//한글

public enum SceneState
{
    Menu,
    Loading,
    Restaurant,
    Draw
}

public class App : MonoBehaviour
{
    public SceneState currentScene;
    GameInstance gameInstance = new GameInstance();
    public Vector3 pos;
    private void Awake()
    {
        currentScene = SceneState.Restaurant;
        gameInstance.GameIns.app = this;
        DontDestroyOnLoad(this);

        SceneManager.LoadSceneAsync("InteractionScene", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("RestaurantScene", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("GatCharScene_Town", LoadSceneMode.Additive);
       
        //   SceneManager.sceneCount
    }
    private void Start()
    {

     

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);

            // 현재 씬의 모든 루트 오브젝트 비활성화
            foreach (GameObject rootObject in scene.GetRootGameObjects())
            {
                bool visible = (scene.name == "InteractionScene" || scene.name == "RestaurantScene");
              //  rootObject.SetActive(visible); // 특정 씬만 활성화
            }
        }
     
    }
    bool down=false;
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !down )
        {
         
            if (gameInstance.GameIns.animalManager)
            {
              //  if (gameInstance.GameIns.animalManager.animalActionCoroutine != null) { StopCoroutine(gameInstance.GameIns.animalManager.animalActionCoroutine); }

                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);

                    // 현재 씬의 모든 루트 오브젝트 비활성화
                    foreach (GameObject rootObject in scene.GetRootGameObjects())
                    {
                //        rootObject.SetActive(scene.name != "RestaurantScene"); // 특정 씬만 활성화
                        
                    }
               
                }
             
            }
            down = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && down)
        {
            down = false;
        
            if (gameInstance.GameIns.animalManager)
            {
                
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    Scene scene = SceneManager.GetSceneAt(i);

                    // 현재 씬의 모든 루트 오브젝트 비활성화
                    foreach (GameObject rootObject in scene.GetRootGameObjects())
                    {
                        bool visible = (scene.name == "InteractionScene" || scene.name == "RestaurantScene");
             //           rootObject.SetActive(visible); // 특정 씬만 활성화
                    }
                }
            }
             gameInstance.GameIns.inputManager.inputDisAble = false;
            gameInstance.GameIns.inputManager.cameraTrans.position = pos;
            Time.timeScale = 1;
            //

            //gameInstance.GameIns.animalManager.StartRoutine();

            ////
            //for (int i = 0; i < gameInstance.GameIns.animalManager.employeeControllers.Count; i++)
            //{
            //    if (gameInstance.GameIns.animalManager.employeeControllers[i].spawning == false)
            //    {
            //        if (gameInstance.GameIns.animalManager.employeeControllers[i].restartCoroutine != null)
            //        {
            //            gameInstance.GameIns.animalManager.employeeControllers[i].restartCoroutine.Invoke();

            //        }

            //    }
            //    else
            //    {
            //        gameInstance.GameIns.animalManager.employeeControllers[i].StartFalling(false);
            //    }
            //}

            //for (int i = 0; i < gameInstance.GameIns.animalManager.customerControllers.Count; i++)
            //{
            //    if (gameInstance.GameIns.animalManager.customerControllers[i].restartCoroutine != null)
            //    {
            //        gameInstance.GameIns.animalManager.customerControllers[i].restartCoroutine.Invoke();
            //    }
            //}

            //for (int i = 0; i < gameInstance.GameIns.workSpaceManager.foodMachines.Count; i++)
            //{

            //    gameInstance.GameIns.workSpaceManager.foodMachines[i].Cooking();
            //}

            //for (int i = 0; i < gameInstance.GameIns.workSpaceManager.spwaners.Count; i++)
            //{
            //    if (gameInstance.GameIns.workSpaceManager.spwaners[i])
            //    {
            //        gameInstance.GameIns.workSpaceManager.spwaners[i].RestartSpawner();
            //    }
            //}
        }
    }

    public void ChangeScene_Restaurant()
    {
        if (currentScene == SceneState.Restaurant) return;
        currentScene = SceneState.Restaurant; 
        gameInstance.GameIns.inputManager.cameraTrans.position = pos;
      //  gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
        gameInstance.GameIns.inputManager.inputDisAble = false;
    //    gameInstance.GameIns.applianceUIManager.UIClearAll(true);
        Time.timeScale = 1;
    }

    public void ChangeScene_DrawScene()
    {
        if (currentScene == SceneState.Draw) return;
        currentScene = SceneState.Draw;
        gameInstance.GameIns.inputManager.DragScreen_WindowEditor(true);
        gameInstance.GameIns.inputManager.inputDisAble = true;
        pos = gameInstance.GameIns.inputManager.cameraTrans.position;
        Time.timeScale = 0;
        gameInstance.GameIns.inputManager.cameraTrans.position = new Vector3(-80.35f, 0, -1080.7f);
        gameInstance.GameIns.uiManager.drawBtn.gameObject.SetActive(true);
        gameInstance.GameIns.uiManager.drawSpeedUpBtn.gameObject.SetActive(true);

    }
}
