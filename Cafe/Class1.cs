using System;
using System.Linq;


// Save this program as MainReturnValTest.cs.
class MainReturnValTest
{
    static int Main()//то с чего начинается программа 
    {
        Kitchen kitchen = new Kitchen();//создание кухни с контроллером
        Controller controller = kitchen.controller;//вытаскивание контролера с кухней внутри
        //надо будет ещё в кухню добавить контроллер, т.к. она должна будет его вызывать, когда заказ будет готов

        Console.WriteLine("Welcome to our Onlne book&order service 'Goida'!!!\n");
        Console.WriteLine("Please sign in or create an account:\n");

        //пользователь создаёт себе аккаунт или выбирает из БД в контроллере и помещает в переменную
        int ourVisitor;

        Console.WriteLine("Choose your table:\n");

        //контролер связывается с БД и показывает свободные столики
        controller.data.show_tables();

        //пользователь выбирает столик и помещает в переменную
        int choosenTable;

        //пока не прописала вывод меню (будет через интерфейс сделано) сделала так
        for (int i = 0; i < 5; i++)
        {
            Console.WriteLine("some kind of dish bla bla bla");
        }

        //controller.show_menu(); (пока не уверена как точно надо реализовать


        //сюда помещаются выбранные блюда, что выберет пользователь
        int[] choosenDishes;


        int do_order = controller.Create_Order(choosenTable, choosenDishes, ourVisitor); //создается заказ и идёт обращение к кухне, возврат времени готовки+вывод на экран

        if (do_order!=0)
        {
            //С установкой таймера в коде МИ сказала не заморачиваться, поэтому делаем через вызов в main, когда заказ закончит создаваться
            //kitchen.SetTimer(do_order); // conroller.dosmth();

            //чуть позже распишу, надо просто чтобы вы тоже понимали, как у нас детально происходит обработка заказа
            //1)контроллером 
            //2)кухней - если непонятно по коду - пишите

        }

        //пример реализации интерфейса
        //controller { 

        //    show(menu or order) // show(Ishow w){ w.print()}
        //}








        Console.ReadLine();
        return 0;

    }
}

//Блюдо
public class Dish
{
    public int Dish_ID;
    public string Name;//Название
    public double Price;//Цена
    public int Menu_Sec_ID; //ID секции меню, чтобы было удобно сортировать и выводить
    public string Menu_Section;//секция меню
    public int Time_Of_Cook;
    public bool Is_Available; //Для проверки на наличие, вроде там в какой-то из диаграмм нужно If использовать, это будет при формировании заказа

    //конструктор
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

// Посетитель
public class Visitor
{
    public int Visitor_ID;
    public string FIO;
    public bool An_Adult; //Совершеннолетний или нет
    public string Number; //телефон

    //конструктор
    public Visitor(int visitor_ID, string fio, bool anadult, string number)
    {
        Visitor_ID = visitor_ID;
        FIO = fio;
        An_Adult = anadult;
        Number = number;
    }



}

//Стол
public class Table
{
    public int Table_ID;
    public bool Is_Booked; //Занят или нет, тоже для проверок при брони стола
    private string Location;//расположение
    private int Capacity; //вместимость на ... человек

    //Конструктор
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

    // Показать свободен ли этот стол, если свободен выведется его номер и кол-во человек
    public void show_free_table()
    {
        if (!Is_Booked)
        {
            Console.WriteLine("Table " + Table_ID + " " + "is free for " + Capacity + " people" + "\n");
        }
    }
}


//Кухня
public class Kitchen
{
    public int Kitchen_ID;
    public int[] Orders_IDs;
    public int Workload; //загруженность (прибавлять к времени исполнения заказа)
    public Controller controller; //чтобы вызывать, когда заказ готов

    //КОнструктор
    public Kitchen()
    {
        Kitchen_ID = 0;
        Orders_IDs = new int[10];
        Workload = 0;
        controller = new Controller(this); //контроллер для кухни, который будет к ней привязан
    }


    //добавить метод для установки контроллера в уже созданную кухню !вроде не надо уже!
    //так же отдельно метод для установки загруженности
    public void SetWorkload(int wl)
    {
        Workload = wl;
    }

    //метод для расчета времени заказа после получения списка блюд
    public int Count_Order_Time(Order order)
    {
        int time = 0;
        foreach (int dish_ID in order.Dish_List)
        {
            int t = controller.data.dishes[dish_ID].Time_Of_Cook; //кухня заходит в контроллер, потом через контролер открывает БД, смотрит в списке блюд по ID наше блюдо и возвращает его время готовки
            time += t;
        }
        time += Workload; //прибавляем к общему времени заказа загруженность кухни
        return time;
    }

    public void SetOrder(Order order)
    {
        Orders_IDs.Append(order.Order_ID);
        Random random = new Random(Workload);
        int rn = random.Next(0, 30);
        Workload += rn;
    }

    public void SetTimer(int orID)
    {
        controller.data.orders.ElementAt(orID).Is_Ready =true;
        controller.Order_is_ready(orID);
    }
}

//Заказ
public class Order
{
    static int ID = 1; //статичный счётчик для заказов, чтобы у каждого нового заказа был уникальный порядковый номер
    public int Order_ID; //номер заказа
    public int Table_ID; //номер столика
    public int Visitor_ID; //номер посетителя
    public int[] Dish_List; //список НОМЕРОВ блюд, не самих блюд
    public bool Is_Ready; //готовность
    public double Total_Price = 0; //итоговая цена
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
        ID++; //при создании нового заказа через конструктор увеличиваем счётчик на +1
        Table_ID = table_id;
        Visitor_ID = visitor_id;
        Is_Ready = false;
    }

}


//Контроллер
public class Controller
{
    public Kitchen k; //кухня, к которой он привязан
    public DataBase data = new DataBase(); //база данных для контроллера
    //конструктор с кухней
    public Controller(Kitchen kit)
    {
        k = kit;
    }
    
    


    //Формирование заказа
    public int Create_Order(int Table_Number, int[] dishes_number, int visitor_id)
    {
        //метод на проверку и бронирование стола тут должен быть (если стол занят, то вывести это пользователю)
        if (!data.Table_Check(Table_Number))
        {
            Console.WriteLine("Sorry, table №" + Table_Number+" is booked");
            return 0;
        }

        //создаём новый заказ
        Order order = new Order(Table_Number, dishes_number, visitor_id);
        //метод для вычисления итоговой стоимости и вывода её на экран
        double chek = this.Get_Chek(dishes_number);
        order.Total_Price = chek;
        Console.WriteLine("Total Cost of your order = " + chek);
        //потом вызов метода у кухни k{поле объекта} для расчёта времени готовки заказа
        int t = k.Count_Order_Time(order);

        //Выводим составленный заказ
        //controller.showOrder(order);

        Console.WriteLine("Time of your order = " + t);

        Console.WriteLine();
        //потверждение от пользователя об оплате (просто написать "да" в консоль)
        Console.WriteLine("Do you confirm the order?");
        Console.WriteLine("\t yes or no");
        String confirm = Console.ReadLine();
        if (confirm.ToLower() == "yes")
        {
            //метод для отправки заказа на кухню 
            k.SetOrder(order);
            //controller.showOrder(order);

            //добавление заказа в data.orders{поле объекта} (БД с заказами)
            this.data.AddOrder(order);

            return order.Order_ID;
        }
        else { return 0; }
    }
    //для подсчёта итогового чека
    public double Get_Chek(int[] dishes_number)
    {
        double chek = 0;
        for (int i = 0; i < dishes_number.Length; i++)
        {
            chek += data.dishes.ElementAt(i).Price;
        }
        return chek;
    }
    
    public void Order_is_ready(int order) 
    {
        Console.WriteLine("Order №"+ order+" is ready!");
    }


}


//База данных (МИ сказала так можно делать, но если будут идеи - опять же пишите в чат)
public class DataBase
{
    Visitor[] visitors = new Visitor[10]; //гости
    public Table[] tables = new Table[10]; //столы
    public Dish[] dishes = new Dish[10]; //блюда
    public Order[] orders = new Order[10]; //заказы
    int n = 10;

    //конструктор для 10 элементов
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
        menu_sections[0] = "drinks";
        menu_sections[1] = "salads";
        menu_sections[2] = "soups";
        menu_sections[3] = "desserts";



        // Заполнение массива блюдами
        dishes[0] = new Dish(1, "caesar", 250.0, "salads", 15);
        dishes[1] = new Dish(2, "olivier", 200.0, "salads", 10);
        dishes[2] = new Dish(3, "kharcho soup", 150.0, "soups", 30);
        dishes[3] = new Dish(4, "borscht", 120.0, "soups", 25);
        dishes[4] = new Dish(5, "coca-cola", 100.0, "drinks", 0);
        dishes[5] = new Dish(6, "orange juice", 120.0, "drinks", 0);
        dishes[6] = new Dish(7, "tiramisu", 220.0, "desserts", 20);
        dishes[7] = new Dish(8, "napoleon", 180.0, "desserts", 15);
        dishes[8] = new Dish(9, "greek salad", 240.0, "salads", 10);
        dishes[9] = new Dish(10, "chicken broth", 140.0, "soups", 40);

        // Для проверки вывода
        foreach (var dish in dishes)
            {
                Console.WriteLine($"{dish.Name} - {dish.Price} руб.");
            }
        
    

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
    //конструктор для n элементов
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

    //вывод свободных столиков из БД
    public void show_tables()
    {
        for (int i = 0; i < n; i++)
        {
            tables.ElementAt(i).show_free_table();
        }
    }
    public void AddOrder(Order order)
    {
        orders.Append(order);
    }
    //метод на проверку и бронирование стола тут должен быть (если стол занят, то вывести это пользователю)
    public bool Table_Check(int table_id)
    {
        return tables.ElementAt(table_id).Is_Booked;
    }
}