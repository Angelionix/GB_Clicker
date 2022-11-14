using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class IOData
{
    //string path = @"D:\Cloud Mail.Ru\MY_Projects\UnityProjects\GB_Clicker\Temp\Save.json";
    /// <summary>
    /// Метод созранения состояния. сперва серриализуем данные в джейсон, затем сохраняем их в строковую переменную
    /// в ПлеерПрефс
    /// </summary>
    /// <param name="gm">ссылка на гейм менеджер</param>
    /// <param name="wm">ссылка на ворлд менеджер</param>
    public void SaveToFile(GameManager gm,ref WorldManager wm)
    {
        
        JObject gameManagerObj = new JObject();
        gameManagerObj["timeOfSave"] = gm.TimeOfAfkStart;
        gameManagerObj["coins"] = gm.Coins;

        JObject worldManager = new JObject();
        worldManager["passiveIncoming"] = wm.WorldPassiveIncoming;
        worldManager["clckDamageMult"] = wm.ClickDamageMult;
        worldManager["passiveMoneyMult"] = wm.PassiveMult;
        worldManager["clickCritChance"] = wm.CritChance;
        string temp = string.Empty;
        if (wm.EntityToSpawnNextField.GetLength(0)>0)
        {
            for (int i = 0; i < wm.EntityToSpawnNextField.GetLength(0); i++)
            {
                temp += $"{wm.EntityToSpawnNextField[i, 0]}.{wm.EntityToSpawnNextField[i, 1]}";
                if (i < wm.EntityToSpawnNextField.GetLength(0)-1)
                {
                    temp += '/';
                }
            }
        }

        worldManager["entitiesToSpawnNextLvl"] = temp;
        JArray floors = new JArray();
        
        for (int i = 0; i < wm.Floors.Length; i++)
        {
            JObject floor = new JObject();
            floor["index"] = wm.Floors[i].IndexFloor;
            floor["isOpened"] = wm.Floors[i].IsOpened;
            floor["timeOfExitFromFloor"] = wm.Floors[i].TimeOfExit;

            JArray slots = new JArray();
            for (int j = 0; j < wm.Floors[i].Slots.Length; j++)
            {
                    JObject slot = new JObject();
                    if (wm.Floors[i].Slots[j].EntityData != null)
                    {
                        slot["entity"] = wm.Floors[i].Slots[j].EntityData.LevelEntity;
                    }
                    else
                    {
                      slot["entity"] = -1;
                    }
                    slots.Add(slot);
            }
            floor["slots"] = slots;
            floors.Add(floor);
        }

        worldManager["floors"] = floors;
        gameManagerObj["world"] = worldManager;

        string json = gameManagerObj.ToString();
        //File.WriteAllText(path, json);
        PlayerPrefs.SetString("save", json);
        PlayerPrefs.Save();
    }
    /// <summary>
    /// Метод для загрузки данных с диска, и обработки их
    /// </summary>
    /// <param name="gm">ссылка на геймменеджер</param>
    /// <param name="wm">ссылка на ворлд менеджер</param>
    /// <returns>возращает будин, который показывает нам были ли сведения созраненные</returns>
    public bool LoadFromJsonFile(ref GameManager gm, ref WorldManager wm)
    {
        //bool fileEst = File.Exists(path);

        if (PlayerPrefs.HasKey("save"))
        {

            string json = PlayerPrefs.GetString("save");

            gm.Coins = double.Parse(JObject.Parse(json)["coins"].ToString());
            gm.TimeOfAfkStart = System.DateTime.Parse(JObject.Parse(json)["timeOfSave"].ToString());
            string worldManager = JObject.Parse(json)["world"].ToString();
            wm.WorldPassiveIncoming = float.Parse(JObject.Parse(worldManager)["passiveIncoming"].ToString());
            wm.ClickDamageMult = float.Parse(JObject.Parse(worldManager)["clckDamageMult"].ToString());
            wm.PassiveMult = int.Parse(JObject.Parse(worldManager)["passiveMoneyMult"].ToString());
            wm.CritChance = float.Parse(JObject.Parse(worldManager)["clickCritChance"].ToString());

            string[] temp = JObject.Parse(worldManager)["entitiesToSpawnNextLvl"].ToString().Split('/');
            for (int i = 0; i < temp.Length; i++)
            {
                string[] tempValue = temp[i].Split('.');
                wm.EntityToSpawnNextField[i, 0] = int.Parse(tempValue[0]);
                wm.EntityToSpawnNextField[i, 1] = int.Parse(tempValue[1]);
            }

            var floors = JObject.Parse(worldManager)["floors"].ToArray();
            for (int i = 0; i < floors.Length; i++)
            {
                wm.Floors[i].IndexFloor = int.Parse(floors[i]["index"].ToString());
                wm.Floors[i].IsOpened = bool.Parse(floors[i]["isOpened"].ToString());
                wm.Floors[i].TimeOfExit = System.DateTime.Parse(floors[i]["timeOfExitFromFloor"].ToString());

                if (wm.Floors[i].IsOpened)
                {
                    var slots = JObject.Parse(floors[i].ToString())["slots"].ToArray();
                    for (int j = 0; j < slots.Length; j++)
                    {
                        wm.Floors[i].EntitiesToSpawnFromFile[j] = int.Parse(slots[j]["entity"].ToString());
                    }
                }
            }
            return true;
        }
        //return fileEst;
        else
        {
            return false;
        }
    }
}
