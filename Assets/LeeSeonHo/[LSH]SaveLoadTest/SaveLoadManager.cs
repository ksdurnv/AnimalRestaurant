using System.IO;
using UnityEngine;
using CryingSnow.FastFoodRush;
using Newtonsoft.Json;
using System.Data;
using System.Collections.Generic;
using System;



public struct CombineDatas
{
    public List<PlayData> playData;
    public List<LevelData> levelData;
    public PlayerData playerData;
    public List<LevelData> employeeData;
}

public class SaveLoadManager
{
    public enum SaveState
    {
        ONLY_SAVE_UPGRADE,
        ONLY_SAVE_PLAYERDATA,
        ONLY_SAVE_EMPLOYEEDATA,
        ALL_SAVES
    }

    public bool isLoading;

    private static string saveFilePath = Application.persistentDataPath;// + "/save/savefile.json";

    public static void SaveGame(bool saveCheck = true, bool levelCheck = true)
    {   
        List<PlayData> playDataList = new List<PlayData>();
        List<LevelData> levelDataList = new List<LevelData>();
     
        GameInstance instance = new GameInstance();

        for (int i = 0; i < instance.GameIns.restaurantManager.levels.Length; i++)
        {
            if (instance.GameIns.restaurantManager.levels[i].TryGetComponent(out FoodMachine machine))
            {
                PlayData playData = new PlayData(i + 1, 2, i <= instance.GameIns.restaurantManager.level - 1);
                playDataList.Add(playData);

                int level = machine.level > 0 ? machine.level : 1; 
                LevelData levelData = new LevelData(i + 1, level,0);
                levelDataList.Add(levelData);
                // json += JsonConvert.SerializeObject(playData);
            }
            else if (instance.GameIns.restaurantManager.levels[i].TryGetComponent(out Table table))
            {
                PlayData playData = new PlayData(i + 1, 1, i <= instance.GameIns.restaurantManager.level - 1);
                playDataList.Add(playData);
                // json += JsonConvert.SerializeObject(playData);
            }
            else if (instance.GameIns.restaurantManager.levels[i].TryGetComponent(out Counter counter))
            {
                PlayData playData = new PlayData(i + 1, 3, i <= instance.GameIns.restaurantManager.level - 1);
                playDataList.Add(playData);
                //   json += JsonConvert.SerializeObject(playData);
            }
            else
            {
                PlayData playData = new PlayData(i + 1, 0, i <= instance.GameIns.restaurantManager.level - 1);
                playDataList.Add(playData);
            }
        }

     

        if (saveCheck)
        {
            var json = JsonConvert.SerializeObject(playDataList);

            string path = Path.Combine(saveFilePath, "save/savefile.json");

            File.WriteAllText(path, json);
        }

        if (levelCheck)
        {
            var json_level = JsonConvert.SerializeObject(levelDataList);

            string path_level = Path.Combine(saveFilePath, "save/savelevel.json");

            File.WriteAllText(path_level, json_level);
        }
    }

    public static void PlayerStateSave(bool isNotlood = false)
    {        
        GameInstance instance = new GameInstance();

       
       
        if(isNotlood)
        {
            PlayerData playerData = new PlayerData(1, 100, 100, 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            instance.GameIns.restaurantManager.playerData = playerData;
            string json_state = JsonConvert.SerializeObject(playerData);
            string path_state = Path.Combine(saveFilePath, "save/savestate.json");
            File.WriteAllText(path_state, json_state);
        }
        else
        {
            instance.GameIns.restaurantManager.playerData.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string json_state = JsonConvert.SerializeObject(instance.GameIns.restaurantManager.playerData);
            string path_state = Path.Combine(saveFilePath, "save/savestate.json");
            File.WriteAllText(path_state, json_state);
        }
      
    }

    public static void Save(SaveState state)
    {
        switch (state)
        {
            case SaveState.ONLY_SAVE_UPGRADE:
                SaveGame();
                break;
            case SaveState.ONLY_SAVE_PLAYERDATA:
                PlayerStateSave();
                break;
            case SaveState.ONLY_SAVE_EMPLOYEEDATA:
             //   EmployeeLevelSave();
                break;
            case SaveState.ALL_SAVES:
                SaveGame();
                PlayerStateSave();
              //  EmployeeLevelSave();
                break;
        }
    }

    public static PlayerData PlayerStateLoad()
    {
        string path_state = Path.Combine(saveFilePath, "save/savestate.json");
       
        string levelInfo = File.ReadAllText(path_state);

        PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(levelInfo);

        return playerData;
    }
    public static void EmployeeLevelSave(bool exist = true)
    {
        GameInstance instance = new GameInstance();


        List<LevelData> levelData = new List<LevelData>();

        if (exist)
        {
            for (int i = 0; i < 8; i++)
            {
                if(i >= instance.GameIns.animalManager.employeeControllers.Count)
                {
                    levelData.Add(new LevelData(i + 1, 1,0));
                }
                else levelData.Add(new LevelData(instance.GameIns.animalManager.employeeControllers[i].id, instance.GameIns.animalManager.employeeControllers[i].EmployeeData.level, instance.GameIns.animalManager.employeeControllers[i].EXP));
            }
       
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                levelData.Add(new LevelData(i + 1, 1,0));
            }
        }
        string path_state = Path.Combine(saveFilePath, "save/employeelevel.json");

        string json = JsonConvert.SerializeObject(levelData);

        File.WriteAllText(path_state, json);
    }
    public static List<LevelData> EmployeeLevelLoad()
    {
        string path_state = Path.Combine(saveFilePath, "save/employeelevel.json");

        string levelInfo = File.ReadAllText(path_state);

        List<LevelData> employeeLevelData = JsonConvert.DeserializeObject<List<LevelData>>(levelInfo);

        return employeeLevelData;
    }

    public static CombineDatas LoadGame()
    {
        GameInstance instance = new GameInstance();
        List<PlayData> data = new List<PlayData>();
        List<LevelData> level = new List<LevelData>();
        List<LevelData> employee = new List<LevelData>();

        string directoryPath = Path.Combine(saveFilePath, "save");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);        
        }
        //    saveFilePath = Path.Combine(Application.persistentDataPath, "save/savefile.json");
        string savePath = Path.Combine(saveFilePath, "save/savefile.json");
        string saveLevelPath = Path.Combine(saveFilePath, "save/savelevel.json");
        string employeeLevelPath = Path.Combine(saveFilePath, "save/employeelevel.json");

        bool dosentSavePathExist = !File.Exists(savePath);
        bool dosentLevelPathExist = !File.Exists(saveLevelPath);
    
        if (dosentSavePathExist || dosentLevelPathExist) SaveGame(dosentSavePathExist, dosentLevelPathExist);

        string gameInfo = File.ReadAllText(savePath);

        data = JsonConvert.DeserializeObject<List<PlayData>>(gameInfo);

        string levelInfo = File.ReadAllText(saveLevelPath);

        level = JsonConvert.DeserializeObject<List<LevelData>>(levelInfo);

        bool saveEmployeeLevelPathExist = File.Exists(employeeLevelPath);
        if(saveEmployeeLevelPathExist)
        {
            employee = EmployeeLevelLoad();
           
        }
        else
        {
            EmployeeLevelSave(saveEmployeeLevelPathExist);
            employee = EmployeeLevelLoad();
        }
      
       

        PlayerData returnPD = new PlayerData(0,0,0,0, null);
        string saveStatePath = Path.Combine(saveFilePath, "save/savestate.json");
        bool saveStatePathExist = File.Exists(saveStatePath);
        if (saveStatePathExist)
        {
            PlayerData pd = PlayerStateLoad(); 

            instance.GameIns.restaurantManager.playerData = pd; 
            Save(SaveState.ONLY_SAVE_PLAYERDATA);

            returnPD = instance.GameIns.restaurantManager.playerData;
        }
        else
        {
            PlayerStateSave(true);
            //Save(SaveState.ONLY_SAVE_PLAYERDATA);

            PlayerData pd = PlayerStateLoad();       
            
            returnPD = instance.GameIns.restaurantManager.playerData;
        }



        CombineDatas combineDatas = new CombineDatas();
        combineDatas.playData = data;
        combineDatas.levelData = level;
        combineDatas.playerData = returnPD;
        combineDatas.employeeData = employee;
        return combineDatas;



    }    

}
