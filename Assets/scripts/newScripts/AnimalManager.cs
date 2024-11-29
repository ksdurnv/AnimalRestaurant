using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllAnimals
{
    public List<Animal> activateAnimals = new List<Animal>();
    public Queue<Animal> deactivateAnimals = new Queue<Animal>();
}

public class AnimalManager : MonoBehaviour
{
    public enum Mode
    {
        GameMode,
        DebugMode
    }

    public Mode mode;

    static GameInstance gameInstance = new GameInstance();
    public List<Employee> employeeControllers = new List<Employee>();
    public List<Customer> customerControllers = new List<Customer>();
    Queue<Employee> deactivateEmployeeControllers = new Queue<Employee>();
    Queue<Customer> deactivateCustomerControllers = new Queue<Customer>();

    [SerializeField]
    Transform employeeStart;

    [SerializeField]
    Transform customerStart;

    public Employee employeeController;
    public Customer customerController;

    public GameObject addanimalController;
    public List<Animal> animals = new List<Animal>(); // *** 배열에서 List로 변경
    List<AllAnimals> allAnimals = new List<AllAnimals>(); // *** 배열에서 List로 변경

    public float minX;
    public float minY;
    public float maxX;
    public float maxY;
    public float height;
    public float distanceSize;

    GameObject animalParent;

    [SerializeField]
    GameObject eatParticle;

    public Coroutine animalActionCoroutine;

    private void Awake()
    {
        WorkSpaceManager workSpaceManager = GetComponent<WorkSpaceManager>();
        gameInstance.GameIns.animalManager = this;
        gameInstance.GameIns.workSpaceManager = workSpaceManager;
        gameInstance.GameIns.calculatorScale.minX = minX;
        gameInstance.GameIns.calculatorScale.minY = minY;
        gameInstance.GameIns.calculatorScale.maxX = maxX;
        gameInstance.GameIns.calculatorScale.maxY = maxY;
        gameInstance.GameIns.calculatorScale.height = height;
        gameInstance.GameIns.calculatorScale.distanceSize = distanceSize;

        animalParent = new GameObject();
        animalParent.name = "animalRoot";

        // 직원 초기화
        for (int i = 0; i < 10; i++)
        {
            Employee controller = Instantiate(employeeController, animalParent.transform).GetComponentInChildren<Employee>();
            controller.instance = gameInstance;
            controller.gameObject.SetActive(false);
            deactivateEmployeeControllers.Enqueue(controller);
        }

        // 손님 초기화
        for (int i = 0; i < 1000; i++)
        {
            Customer controller = Instantiate(customerController, animalParent.transform).GetComponentInChildren<Customer>();
            controller.instance = gameInstance;
            controller.gameObject.SetActive(false);
            deactivateCustomerControllers.Enqueue(controller);
        }

        // 동물 초기화
        foreach (var animal in animals) // *** List 기반으로 초기화
        {
            AllAnimals aa = new AllAnimals();
            for (int j = 0; j < 100; j++)
            {
                Animal newAnimal = Instantiate(animal, animalParent.transform);
                newAnimal.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                newAnimal.gameObject.SetActive(false);                
                aa.deactivateAnimals.Enqueue(newAnimal);
            }
            allAnimals.Add(aa); // *** 새로 생성된 AllAnimals를 추가
        }

        MoveCalculator.CheckArea(gameInstance.GameIns.calculatorScale);
    }

    private void Start()
    {
        if (mode == Mode.GameMode) StartRoutine();
    }

    public void StartRoutine()
    {
        StartCoroutine(AnimalRoutine());
    }

    IEnumerator AnimalRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            foreach (var employee in employeeControllers)
            {
                if (!employee.busy)
                {
                    employee.busy = true;
                    employee.FindEmployeeWorks();
                }
            }

            foreach (var customer in customerControllers)
            {
                if (!customer.busy && customer.foodsAnimalsWant.spawnerType != AnimalSpawner.SpawnerType.Delivery)
                {
                    customer.busy = true;
                    customer.FindCustomerActions();
                }
            }
        }
    }

    public Employee SpawnEmployee()
    {
        int type = 0;

        if (deactivateCustomerControllers.Count > 0 && allAnimals[type].deactivateAnimals.Count > 0)
        {
            Employee employee = deactivateEmployeeControllers.Dequeue();
            employee.gameObject.SetActive(true);
            employeeControllers.Add(employee);
            Animal animal = SpawnAnimal(employee, type);
            employee.id = employeeControllers.Count;
            employee.ui = animal.GetComponentInChildren<SliderController>();
            employee.headPoint = animal.GetComponentInChildren<Head>().gameObject.transform;
            employee.mousePoint = animal.GetComponentInChildren<Mouse>().gameObject.transform;
            employee.animator = animal.GetComponentInChildren<Animator>();
            employee.eatParticle = eatParticle;
            employee.transform.localPosition = Vector3.zero;
            employee.transform.localRotation = Quaternion.identity;
            return employee;
        }
        return null;
    }

    public Customer SpawnCustomer(FoodsAnimalsWant foodsAnimalsWant, bool onlyOrder = false)
    {
        int r = Random.Range(1, animals.Count); // *** List.Count 기반으로 변경
        if (deactivateCustomerControllers.Count > 0 && allAnimals[r].deactivateAnimals.Count > 0)
        {
            Customer customer = deactivateCustomerControllers.Dequeue();
            customer.gameObject.SetActive(true);

            if (!onlyOrder)
            {
                Animal animal = SpawnAnimal(customer, r);

                customer.animalType = (AnimalType)r;
                customer.headPoint = animal.GetComponentInChildren<Head>().gameObject.transform;
                customer.mousePoint = animal.GetComponentInChildren<Mouse>().gameObject.transform;
                customer.animator = animal.GetComponentInChildren<Animator>();
                customer.eatParticle = eatParticle;
            }
            customer.Setup(foodsAnimalsWant);
            customerControllers.Add(customer);
            customer.transform.localPosition = Vector3.zero;
            customer.transform.localRotation = Quaternion.identity;
            if (onlyOrder)
            {
                customer.busy = true;
            }

            return customer;
        }
        return null;
    }

    Animal SpawnAnimal(AnimalController controller, int type)
    {
        Animal animal = allAnimals[type].deactivateAnimals.Dequeue();
        animal.gameObject.SetActive(true);
        controller.trans = animal.trans;
        controller.modelTrans = animal.modelTrans;
        controller.transform.SetParent(animal.transform);

        allAnimals[type].activateAnimals.Add(animal);

        return animal;
    }

    public void AddNewAnimal(Animal newAnimal) // *** 동적 동물 추가 메서드
    {
        if (animals.Contains(newAnimal)) // *** 이미 리스트에 포함된 경우
        {
            return; // *** 메서드 종료
        }

        animals.Add(newAnimal); // *** 새로운 동물을 List에 추가        

        AllAnimals newAllAnimals = new AllAnimals(); // *** 새로운 AllAnimals 생성
        for (int i = 0; i < 100; i++)
        {
            Animal animal = Instantiate(newAnimal, animalParent.transform);
            animal.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            animal.gameObject.SetActive(false);
            newAllAnimals.deactivateAnimals.Enqueue(animal);            
        }
        allAnimals.Add(newAllAnimals); // *** 새로운 AllAnimals를 리스트에 추가
    }


    public void DespawnEmployee(Employee employee)
    {
        employee.foodStacks.Clear();
        employee.gameObject.SetActive(false);
        employee.employeeState = EmployeeState.Wait;
        employee.busy = false;
        employeeControllers.Remove(employee);
        int type = 0;
        DespawnAnimal(employee, type);
        deactivateEmployeeControllers.Enqueue(employee);
    }

    public void DespawnCustomer(Customer customer, bool onlyOrder = false)
    {
        customer.customerState = CustomerState.Walk;
        customer.busy = false;
        customerControllers.Remove(customer);
        if (!onlyOrder)
        {
            customer.foodStacks.Clear();
            customer.gameObject.SetActive(false);
            int t = (int)customer.animalType;
            DespawnAnimal(customer, t);
        }
        else
        {
            foreach (var stack in customer.foodStacks)
            {
                foreach (var food in stack.foodStack)
                {
                    FoodManager.RemovePackageBox((PackageFood)food);
                }
            }
            customer.foodStacks.Clear();
        }
        deactivateCustomerControllers.Enqueue(customer);
    }

    void DespawnAnimal(AnimalController controller, int type)
    {
        Animal al = controller.GetComponentInParent<Animal>();
        allAnimals[type].activateAnimals.Remove(al);
        al.gameObject.SetActive(false);
        controller.transform.SetParent(animalParent.transform);
        allAnimals[type].deactivateAnimals.Enqueue(al);
    }

    public void UpdateEmployeeUpgrade(Employee animal)
    {
        animal.EmployeeData = gameInstance.GameIns.restaurantManager.employeeDatas[animal.id - 1];
    }
}
