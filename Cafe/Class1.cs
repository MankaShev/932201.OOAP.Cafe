using System;
using System.Linq;


// Save this program as MainReturnValTest.cs.
class MainReturnValTest
{
    static int Main()//то с чего начинается программа 
    {
        IShow inter;//создание интерфейса
        Kitchen kitchen = new Kitchen();//создание кухни с контроллером
        Controller controller = kitchen.controller;//вытаскивание контролера с кухней внутри

        Console.WriteLine("Welcome to our Onlne book&order service 'Goida'!!!\n");
        Console.WriteLine("Please sign in or create an account:\n");

        //пользователь создаёт себе аккаунт или выбирает из БД в контроллере и помещает в переменную
        int ourVisitor = 2;
        Console.WriteLine("Nice to see you again,"+controller.data.visitors[ourVisitor].FIO+"\n");

        Console.WriteLine("Choose your table:\n");

        //контролер связывается с БД и показывает свободные столики
        controller.data.show_tables();

        //пользователь выбирает столик и помещает в переменную
        int choosenTable = int.Parse(Console.ReadLine());
        
        // Говорим интерфейсу, что он сейчас выводит Menu
        inter = new Menu();
        Console.WriteLine($"\tMenu: ");
        controller.data.dishes = inter.Sorted(controller.data.dishes);
        inter.Show(controller.data.dishes);//вывод секционного меню

        //сюда помещаются выбранные блюда, что выберет пользователь
        int[] choosenDishes = new int[] { 2, 5, 8, 1 };

        int do_order = 0;
        do_order = controller.Create_Order(choosenTable, choosenDishes, ourVisitor, inter); //создается заказ и идёт обращение к кухне, возврат времени готовки+вывод на экран
        //возвращает номер сформированного заказа

        if (do_order!=0)
        {
            kitchen.SetTimer(do_order); // "Установка таймера" на кухне и его мгновенное срабатывание
        }
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
        if (menuSection == "drinks") // drinks
        {
            Menu_Sec_ID = 3;
        }
        else if (menuSection == "salads") // salads
        {
            Menu_Sec_ID = 1;
        }
        else if (menuSection == "soups") // soups
        {
            Menu_Sec_ID = 2;
        }
        else if (menuSection == "desserts") // desserts
        {
            Menu_Sec_ID = 4;
        }
        else
        {
            Menu_Sec_ID = -1; // Некорректный раздел меню
        }
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
            Console.WriteLine("Table " + Table_ID + " " + "is free for " + Capacity + " people");
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

    //Конструктор
    public Kitchen()
    {
        Random random = new Random(10);
        int rn = random.Next(0, 15);
        SetWorkload(rn);
        Kitchen_ID = 0;
        Orders_IDs = new int[10];
        Workload = 0;
        controller = new Controller(this); //контроллер для кухни, который будет к ней привязан
    }

    
    //так же отдельно метод для установки загруженности
    public void SetWorkload(int wl)
    {
        Workload = wl;
    }

    //метод для расчета времени заказа после получения списка блюд
    public int Count_Order_Time(Order order)
    {
        int time = 0;

        for (int i = 0; i < order.Dish_List.Length; i++)
        {
            int dish_ID = order.Dish_List[i];
            int t = controller.data.dishes[dish_ID].Time_Of_Cook; //кухня заходит в контроллер, потом через контролер открывает БД, смотрит в списке блюд по ID наше блюдо и возвращает его время готовки
            time += t;
        }
        time += Workload; //прибавляем к общему времени заказа загруженность кухни
        return (int)(time / 3) + 1;
    }

    //метод для добавления заказа в список выполняемых этой кухней
    public void SetOrder(Order order)
    {
        Orders_IDs.Append(order.Order_ID); //добавления заказа в список выполняемых этой кухней
        Random random = new Random(Workload);
        int rn = random.Next(0, 30);
        Workload += rn; //увеличиваем нагруженность
    }

    //Установка таймера
    public void SetTimer(int orID)
    {
        controller.data.orders.ElementAt(orID).Is_Ready =true; //меняем статус заказа в БД
        controller.Order_is_ready(orID); //выыодим
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
        Dish_List= dish_ids;
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
    public int Create_Order(int Table_Number, int[] dishes_number, int visitor_id, IShow inter)
    {
        //метод на проверку и бронирование стола тут должен быть (если стол занят, то вывести это пользователю)
        if (data.Table_Check(Table_Number))
        {
            Console.WriteLine("Sorry, table №" + Table_Number + " is booked");
            return 0;
        }
        //создаём новый заказ
        Order order = new Order(Table_Number, dishes_number, visitor_id);
        //создаём по нему список Блюд
        Dish[] dishes = new Dish[dishes_number.Length];
        for (int i = 0; i < dishes_number.Length; i++)
        {
            dishes[i] = data.dishes[dishes_number[i]];
        }

        //Выводим составленный список блюд в заказе
        inter = new OrderShow();
        dishes=inter.Sorted(dishes);
        Console.WriteLine($"ORDER - ID: {order.Order_ID}");
        inter.Show(dishes);

        //метод для вычисления итоговой стоимости и вывода её на экран
        double chek = this.Get_Chek(dishes_number);
        order.Total_Price = chek;
        Console.WriteLine("Total Cost of your order = " + chek);
        //потом вызов метода у кухни k{поле объекта} для расчёта времени готовки заказа
        int t = k.Count_Order_Time(order);
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
        else { return 0;}
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
    public Visitor[] visitors = new Visitor[10]; //гости
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
 public void show_tables()//вывод свободных столиков из БД
    {
        for (int i = 0; i < n; i++)
        {
            tables.ElementAt(i).show_free_table();//для каждого стола идем в бд и выполняем метод, который смотрит наличие брони
        }
    }
    public void AddOrder(Order order) //добавление заказа в БД
    {
        orders[order.Order_ID] = order;
    }
    //метод на проверку бронирования стола 
    public bool Table_Check(int table_id)
    {
        return tables.ElementAt(table_id).Is_Booked;
    }
}
//Интрефейс вывода
public interface IShow
{
    Dish[] Sorted(Dish[] dishes); //сортировка по секции меню
    void Show(Dish[] dishes); //секционный вывод 
}

public class Menu : IShow
{
    public Dish[] Sorted(Dish[] dishes)
    {
        Array.Sort(dishes, (d1, d2) => d1.Menu_Sec_ID.CompareTo(d2.Menu_Sec_ID));
        return dishes;
    }
    public void Show(Dish[] dishes)
    {
        // Sorting dishes by menu section
        var groupedDishes = dishes
            .Where(d => d.Is_Available)
            .GroupBy(d => d.Menu_Section);

        foreach (var section in groupedDishes)
        {
            Console.WriteLine($"Menu Section: {section.Key}");

            foreach (var dish in section)
            {
                Console.WriteLine($"\tID: {dish.Dish_ID},\t Name: {dish.Name},\t Price: {dish.Price}");
            }

            Console.WriteLine(); // Empty line for separating sections
        }
    }



    //public void Show(Dish[] dishes)
    //{
    //    foreach (var dish in dishes)
    //    {
    //        if (dish.Is_Available)
    //        {
    //            Console.WriteLine($"ID: {dish.Dish_ID},\t Name: {dish.Name},\t Price: {dish.Price}");
    //        }
    //    }
    //}
}
public class OrderShow : IShow
{
    public Dish[] Sorted(Dish[] dishes)
    {
        // Пример сортировки (по возрастанию)
        Array.Sort(dishes, (d1, d2) => d1.Menu_Sec_ID.CompareTo(d2.Menu_Sec_ID));
        return dishes;
    }

    public void Show(Dish[] dishes)
    {        
        // Sorting dishes by menu section
        var groupedDishes = dishes
            .Where(d => d.Is_Available)
            .GroupBy(d => d.Menu_Section);

        foreach (var section in groupedDishes)
        {
            Console.WriteLine($"Menu Section: {section.Key}");

            foreach (var dish in section)
            {
                Console.WriteLine($"Name: {dish.Name}, Price: {dish.Price}");
            }

            Console.WriteLine(); // Empty line for separating sections
        }
    }
}
