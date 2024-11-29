[System.Serializable]
public class PlayerData
{
    public int level;
    public float money;
    public int fishesNum;
    public int employeeNum;
    public string time;

    public PlayerData(int level, float money, int fishesNum, int employeeNum, string time)
    {
        this.level = level;
        this.money = money;
        this.employeeNum = employeeNum;
        this.time = time;
        this.fishesNum = fishesNum;
    }
}
