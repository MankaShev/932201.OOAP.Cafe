using System;
using System.Linq;


// Save this program as MainReturnValTest.cs.
class MainReturnValTest
{
    static int Main()
    {
        Kitchen kitchen = new Kitchen();
        Controller controller = new Controller(kitchen);


        Console.WriteLine("Welcome to our Onlne book&order service 'Goida'!!!\n");
        Console.WriteLine("Please sign in or create an account:\n");

        int ourVisitor;

        Console.WriteLine("Choose your table:\n");

        controller.data.show_tables();

        //взять например с консоли
        int choosenTable;

        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine("some kind of dish bla bla bla");
        }

        //controller.show_menu();

        int[] choosenDishes; //maybe buttons(?)


        //controller.getOrder(); // обращение к кухне установка таймера

        //kitchen.settimer(); // conroller.dosmth();




        //controller { 
        
        //    show(menu or order) // show(Ishow w){ w.print()}
        //}








        Console.ReadLine();
        return 0;

    }
}


public class Dish
{
    public int Dish_ID;
    public string Name;
    public double Price;
    public string Menu_Section;
    public int Time_Of_Cook;
    public bool Is_Available;

    public Dish(int dishID, string name, double price, string menuSection, int timeOfCook)
    {
        Dish_ID = dishID;
        Name = name;
        Price = price;
        Menu_Section = menuSection;
        Time_Of_Cook = timeOfCook;
        Is_Available = true;
    }



}

public class Visitor
{
    public int Visitor_ID;
    public string FIO;
    public bool An_Adult;
    public string Number;

    public Visitor(int visitor_ID, string fio, bool anadult, string number)
    {
        Visitor_ID = visitor_ID;
        FIO = fio;
        An_Adult = anadult;
        Number = number;
    }



}

public class Table
{
    public int Table_ID;
    public bool Is_Booked;
    private string Location;
    private int Capacity;

    public Table(int table_ID, bool is_Booked, string location, int capacity)
    {
        Table_ID = table_ID;
        Is_Booked = is_Booked;
        Location = location;
        Capacity = capacity;
    }

    public int GetCapacity()
    {
        return Capacity;
    }
    public void BookTable()
    {
        Is_Booked = true;
    }
    public void show_free_table()
    {
        if (!Is_Booked)
        {
            Console.WriteLine("Table " + Table_ID + " " + "is free for " + Capacity + " people" + "\n");
        }
    }
}

public class Kitchen
{
    public int Kitchen_ID;
    public int[] Orders_IDs;
    public int Workload;

    public Kitchen()
    {
        Kitchen_ID = 0;
        Orders_IDs = new int[10];
        Workload = 0;
    }
    public Kitchen(int ID, int[] ordersIDs, int workload)
    {
        Kitchen_ID = ID;
        Orders_IDs = ordersIDs;
        Workload = workload;
    }
}

public class Order
{
    static int ID = 0;
    public int Order_ID;
    public int Table_ID;
    public int Visitor_ID;
    public int[] Dish_List;
    public bool Is_Ready;
    public double Total_Price = 0;
    public Order()
    {
        Order_ID = ID;
        ID++;
        Table_ID = 0;
        Visitor_ID = 0;
        Is_Ready = false;
        Total_Price = 0;
    }

    public Order(int table_id, int[] dish_ids, int visitor_id)
    {
        Order_ID = ID;
        ID++;
        Table_ID = table_id;
        Visitor_ID = visitor_id;
        Is_Ready = false;
    }

}

public class Controller
{
    public Kitchen k;
    public DataBase data = new DataBase();
    public Controller(Kitchen kit)
    {
        k = kit;
    }

    public void Create_Order(int Table_Number, int[] dishes_number, int visitor_id)
    {
        Order order = new Order(Table_Number, dishes_number, visitor_id);

        
    }

    public double Get_Price(Dish dish)
    {
        return dish.Price;
    }



}



public class DataBase
{
    Visitor[] visitors = new Visitor[10];
    Table[] tables = new Table[10];
    Dish[] dishes = new Dish[10];
    Order[] orders = new Order[10];
    int n = 10;

    public DataBase()
    {
        #region DBO
        string[] FIOs = new string[10];
        FIOs[0] = "Ivanov Ivan";
        FIOs[1] = "Montichelli Heugo";
        FIOs[2] = "Tuefan David";
        FIOs[3] = "Wedington Kate";
        FIOs[4] = "Thuvorov Alex";
        FIOs[5] = "Friman Gordon";
        FIOs[6] = "Satanisti Ruliat";
        FIOs[7] = " ";
        FIOs[8] = " ";
        FIOs[9] = " ";

        string[] phoneNumbers = new string[10];
        phoneNumbers[0] = "123-456-7890"; // Ivanov Ivan
        phoneNumbers[1] = "234-567-8901"; // Montichelli Heugo
        phoneNumbers[2] = "345-678-9012"; // Tuefan David
        phoneNumbers[3] = "456-789-0123"; // Wedington Kate
        phoneNumbers[4] = "567-890-1234"; // Thuvorov Alex
        phoneNumbers[5] = "678-901-2345"; // Friman Gordon
        phoneNumbers[6] = "789-012-3456"; // Satanisti Ruliat
        phoneNumbers[7] = " ";
        phoneNumbers[8] = " ";
        phoneNumbers[9] = " ";

        string[] locations = new string[2];
        locations[0] = "cafe";
        locations[1] = "outside";

        string[] menu_sections = new string[4];
        menu_sections[0] = "main";
        menu_sections[1] = "salads";
        menu_sections[2] = "soups";
        menu_sections[3] = "desserts";
        #endregion

        for (int i = 0; i < 10; i++)
        {
            visitors[i] = new Visitor(i, FIOs.ElementAt(i), true, phoneNumbers.ElementAt(i));
            Random random = new Random(i);
            int rn = random.Next(0, 10);
            bool isbook = (rn + 1) % 2 == 0;
            tables[i] = new Table(i, isbook, locations.ElementAt(rn % 2), rn);
            //Console.WriteLine("Table " + i + " " + tables.ElementAt(i).Is_Booked);
        }
    }
    public DataBase(int n)
    {
        this.n = n;
        Visitor[] visitors = new Visitor[n];
        Table[] tables = new Table[n];
        Dish[] dishes = new Dish[n];
        Order[] orders = new Order[n];
        #region DBO
        string[] FIOs = new string[n];
        FIOs[0] = "Ivanov Ivan";
        FIOs[1] = "Montichelli Heugo";
        FIOs[2] = "Tuefan David";
        FIOs[3] = "Wedington Kate";
        FIOs[4] = "Thuvorov Alex";
        FIOs[5] = "Friman Gordon";
        FIOs[6] = "Satanisti Ruliat";

        string[] phoneNumbers = new string[n];
        phoneNumbers[0] = "123-456-7890"; // Ivanov Ivan
        phoneNumbers[1] = "234-567-8901"; // Montichelli Heugo
        phoneNumbers[2] = "345-678-9012"; // Tuefan David
        phoneNumbers[3] = "456-789-0123"; // Wedington Kate
        phoneNumbers[4] = "567-890-1234"; // Thuvorov Alex
        phoneNumbers[5] = "678-901-2345"; // Friman Gordon
        phoneNumbers[6] = "789-012-3456"; // Satanisti Ruliat


        string[] locations = new string[2];
        locations[0] = "cafe";
        locations[1] = "outside";

        string[] menu_sections = new string[4];
        menu_sections[0] = "main";
        menu_sections[1] = "salads";
        menu_sections[2] = "soups";
        menu_sections[3] = "desserts";
        #endregion

        for (int i = 0; i < n; i++)
        {
            visitors[i] = new Visitor(i, FIOs.ElementAt(i), true, phoneNumbers.ElementAt(i));
            Random random = new Random(i);
            int rn = random.Next(0, 10);
            bool isbook = (rn + 1) % 2 == 0;
            tables[i] = new Table(i, isbook, locations.ElementAt(rn % 2), rn);
            //Console.WriteLine("Table " + i + " " + tables.ElementAt(i).Is_Booked);
        }
    }

    public void show_tables()
    {
        for (int i = 0; i < n; i++)
        {
            tables.ElementAt(i).show_free_table();
        }
    }

}