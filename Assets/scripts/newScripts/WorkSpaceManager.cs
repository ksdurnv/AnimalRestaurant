using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkSpaceManager : MonoBehaviour
{
   // ������� �Ŵ������� �߰��Ǵ� Ȯ�尡�� �ⱸ��
    public List<Counter> counters = new List<Counter>();
    public List<FoodMachine> foodMachines = new List<FoodMachine>();
    public List<Table> tables = new List<Table>();
    public List<TrashCan> trashCans = new List<TrashCan>();
    public List<FoodStack> foodStacks = new List<FoodStack>();
    public List<GameObject> endPoint = new List<GameObject>();
    public List<PackingTable> packingTables = new List<PackingTable>();
    public List<AnimalSpawner> spwaners = new List<AnimalSpawner>();

    public int[] unlocks = new int[4];

    //�������� �ִ� ���� ����


    public Food food;
    public PackageFood box;
    public Garbage garbage;
    public GameObject[] particle;
    public RewardsBox rewardsBox;

    private void Awake()
    {

        FoodManager.NewFood(food,1000);
        FoodManager.NewFood(food, 200, true);
        FoodManager.NewPackageBox(box, 100);
        GarbageManager.NewGarbage(garbage, 100);
        for (int i = 0; i < particle.Length; i++)
        {
            ParticleManager.NewParticle(particle[i], 100);
        }

        FoodManager.NewRewardsBox(rewardsBox, 5);


    }
}
