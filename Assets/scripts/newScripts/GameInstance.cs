using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameInstance
{
    static GameInstance gameInstance = new GameInstance();

    public App app;
    public CalculatorScale calculatorScale;
    public AnimalManager animalManager;
    public WorkSpaceManager workSpaceManager;
    public InputManger inputManager;
    public UIManager uiManager;
    public MoneyManager moneyManager;
    public RestaurantManager restaurantManager;
    public ApplianceUIManager applianceUIManager;
    public GatcharManager gatcharManager;
    public PlayerAnimalDataManager playerAnimalDataManager;
    public GameInstance GameIns{ get { return gameInstance; } }

    public static Stack<Coord> GetCoords(Coord coord)
    {
        Coord cor = coord;
        Stack<Coord> stack = new Stack<Coord>();
        stack.Push(cor);
        while (cor.parent != null)
        {
            cor = cor.parent;
            stack.Push(cor);
        }
        return stack;
    }
}
