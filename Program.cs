using System;
using System.Linq;

class TransportProblem
{
    public static bool ClosedTask(int[] supply, int[] demand)
    {
        int totalSupply = 0;
        int totalDemand = 0;
        for (int i = 0; i < supply.Length; i++)
            totalSupply += supply[i];

        for (int i = 0; i < demand.Length; i++)
            totalDemand += demand[i];

        if (totalSupply == totalDemand)
        {
            Console.WriteLine("\nТранспортная задача закрытая");
            return true;
        }
        else
        {
            Console.WriteLine("\nТранспортная задача открытая");
            Console.WriteLine($"Сумма поставок: {totalSupply}, сумма спроса: {totalDemand}");
            return false;
        }
    }

    public static int[,] MinElementMethod(int[] supply, int[] demand, int[,] cost)
    {
        int suppliers = supply.Length;
        int consumers = demand.Length;
        int[,] allocation = new int[suppliers, consumers];
        int totalCost = 0;

        while (true)
        {
            int minCost = int.MaxValue;
            int minRow = -1, minCol = -1;

            for (int i = 0; i < suppliers; i++)
            {
                for (int j = 0; j < consumers; j++)
                {
                    if (cost[i, j] < minCost && supply[i] > 0 && demand[j] > 0)
                    {
                        minCost = cost[i, j];
                        minRow = i;
                        minCol = j;
                    }
                }
            }

            if (minRow == -1 || minCol == -1)
                break;

            int allocated = Math.Min(supply[minRow], demand[minCol]);
            allocation[minRow, minCol] = allocated;
            totalCost += allocated * cost[minRow, minCol];

            supply[minRow] -= allocated;
            demand[minCol] -= allocated;
        }

        Console.WriteLine("Метод минимального элемента:");
        for (int i = 0; i < suppliers; i++)
        {
            for (int j = 0; j < consumers; j++)
                Console.Write(allocation[i, j] + " ");
            Console.WriteLine();
        }
        Console.WriteLine("Общая стоимость: " + totalCost);

        return allocation;
    }

    public static int[,] NorthWestCornerMethod(int[] supply, int[] demand, int[,] cost)
    {
        int suppliers = supply.Length;
        int consumers = demand.Length;
        int[,] allocation = new int[suppliers, consumers]; // Матрица распределения
        int totalCost = 0; // Общая стоимость перевозки

        int i = 0, j = 0; // С верхнего левого угла
        while (i < suppliers && j < consumers)
        {
            // Определяем, сколько можно поставить в текущую ячейку
            int allocated = Math.Min(supply[i], demand[j]);
            allocation[i, j] = allocated;
            totalCost += allocated * cost[i, j]; // Учитываем стоимость

            // Уменьшаем доступный запас и спрос
            supply[i] -= allocated;
            demand[j] -= allocated;

            // Если запас исчерпан — переходим к следующему поставщику
            if (supply[i] == 0) i++;
            // Если спрос исчерпан — переходим к следующему потребителю
            if (demand[j] == 0) j++;
        }

        // Вывод результата
        Console.WriteLine("Метод северо-западного угла:");
        for (int i2 = 0; i2 < allocation.GetLength(0); i2++)
        {
            for (int j2 = 0; j2 < allocation.GetLength(1); j2++)
            {
                Console.Write(allocation[i2, j2] + " ");
            }
            Console.WriteLine();
        }
        Console.WriteLine("Общая стоимость: " + totalCost);
        return allocation;
    }



    public static void MetodPotancialov(int[,] cost, int[,] allocation)
    {
        int suppliers = cost.GetLength(0);
        int consumers = cost.GetLength(1);
        int?[] u = new int?[suppliers];
        int?[] v = new int?[consumers];

        //Хранение координат занятых ячеек
        int[,] occupiedCells = new int[suppliers * consumers, 2];
        int count = 0;
        //Создание списка занятых ячеек в матрице 
        for (int i = 0; i < suppliers; i++)
        {
            for (int j = 0; j < consumers; j++)
            {
                if (allocation[i, j] > 0)
                {
                    occupiedCells[count, 0] = i;
                    occupiedCells[count, 1] = j;
                    count++;
                }
            }
        }

        u[0] = 0;
        bool updated;
        do
        {
            updated = false;
            for (int k = 0; k < count; k++) //все занятые ячейки матрицы
            {
                int i = occupiedCells[k, 0];
                int j = occupiedCells[k, 1];

                if (u[i] != null && v[j] == null)
                {
                    v[j] = cost[i, j] - u[i];
                    updated = true;
                }
                else if (v[j] != null && u[i] == null)
                {
                    u[i] = cost[i, j] - v[j];
                    updated = true;
                }
            }
        } while (updated);

        Console.WriteLine("Потенциалы:");
        Console.WriteLine("u: " + string.Join(", ", u));
        Console.WriteLine("v: " + string.Join(", ", v));

        bool isOptimal = true;
        for (int i = 0; i < suppliers; i++)
        {
            for (int j = 0; j < consumers; j++)
            {
                if (allocation[i, j] == 0)
                {
                    int delta_uv = (u[i] ?? 0) + (v[j] ?? 0) - cost[i, j];

                    if (delta_uv > 0)
                    {
                        Console.WriteLine($"delta[{i + 1}][{j + 1}]: {delta_uv} - опорный план не оптимален!");
                        isOptimal = false;
                    }
                    else
                    {
                        Console.WriteLine($"delta[{i + 1}][{j + 1}]: {delta_uv}");
                    }
                }
            }
        }

        Console.WriteLine();
        if (isOptimal)
        {
            Console.WriteLine("Опорный план оптимален!");
        }
        else
        {
            Console.WriteLine("Опорный план не оптимален!");
        }
    }

    static void Main()
    {
        Console.Write("Введите количество поставщиков: ");
        int suppliers = int.Parse(Console.ReadLine());
        Console.Write("Введите количество потребителей: ");
        int consumers = int.Parse(Console.ReadLine());

        int[] supply = new int[suppliers];//поставщики
        int[] demand = new int[consumers];//спрос
        int[,] cost = new int[suppliers, consumers];

        Console.WriteLine("Введите объемы поставок:");
        for (int i = 0; i < suppliers; i++)
            supply[i] = int.Parse(Console.ReadLine());

        Console.WriteLine("Введите объемы спроса:");
        for (int j = 0; j < consumers; j++)
            demand[j] = int.Parse(Console.ReadLine());

        Console.WriteLine("Введите тарифный план:");
        for (int i = 0; i < suppliers; i++)
        {
            for (int j = 0; j < consumers; j++)
            {
                Console.Write($"Тариф от поставщика {i + 1} к потребителю {j + 1}: ");
                cost[i, j] = int.Parse(Console.ReadLine());
            }
        }
        
        //Проверка на закрытость
        if (supply.Sum() == demand.Sum())
        {
            int[,] allocation = MinElementMethod((int[])supply.Clone(), (int[])demand.Clone(), cost);
            //int[,] allocation = NorthWestCornerMethod((int[])supply.Clone(), (int[])demand.Clone(), cost);
            Console.WriteLine("\nПроверка оптимальности метода минимального элемента:");
            MetodPotancialov(cost, allocation);
        }
    }
}
