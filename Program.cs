using System;
using System.Threading;
using System.IO;

class Timer
{
    public event EventHandler Tick;

    public void Start()
    {
        while (true)
        {
            Thread.Sleep(1000); 
            Tick?.Invoke(this, EventArgs.Empty);
        }
    }
}

class Clock
{
    public Clock(Timer timer)
    {
        timer.Tick += OnTick;
    }

    private void OnTick(object sender, EventArgs e)
    {
        Console.WriteLine($"Current time: {DateTime.Now.ToString("HH:mm:ss")}");
    }
}

class Counter
{
    private int count;

    public Counter(Timer timer)
    {
        timer.Tick += OnTick;
    }

    private void OnTick(object sender, EventArgs e)
    {
        count++;
        Console.WriteLine($"Counter: {count}");
    }
}

class BankAccount
{
    private decimal balance;

    public decimal Balance
    {
        get { return balance; }
    }

    public event Action<decimal> BalanceChanged;

    public void Deposit(decimal amount)
    {
        balance += amount;
        OnBalanceChanged(balance);
    }

    public void Withdraw(decimal amount)
    {
        if (amount <= balance)
        {
            balance -= amount;
            OnBalanceChanged(balance);
        }
        else
        {
            Console.WriteLine("Insufficient funds");
        }
    }

    private void OnBalanceChanged(decimal newBalance)
    {
        BalanceChanged?.Invoke(newBalance);
    }
}

class Logger
{
    private string logFileName = "balance_log.txt";

    public void LogBalance(decimal balance)
    {
        File.AppendAllText(logFileName, $"{DateTime.Now}: Balance changed to {balance:C}\n");
    }
}

class Button
{
    private string text;
    private event EventHandler click;

    public string Text
    {
        get { return text; }
        set { text = value; }
    }

    public event EventHandler Click
    {
        add
        {
            if (click == null || click.GetInvocationList().Length < 3)
                click += value;
        }
        remove
        {
            click -= value;
        }
    }

    public void ClickButton()
    {
        Console.WriteLine($"Button clicked: {text}");
        click?.Invoke(this, EventArgs.Empty);
    }
}

class Program
{
    static void Main()
    {

        Timer timer = new Timer();
        Clock clock = new Clock(timer);
        Counter counter = new Counter(timer);

        ThreadPool.QueueUserWorkItem(state => timer.Start());

        BankAccount account = new BankAccount();
        Logger logger = new Logger();

        account.BalanceChanged += logger.LogBalance;

        account.Deposit(1000);
        account.Withdraw(500);

        Console.WriteLine($"Current balance: {account.Balance:C}");

        Button button = new Button();
        button.Text = "Click Me";

        button.Click += (sender, e) => Console.WriteLine("Button was clicked (1)");
        button.Click += (sender, e) => Console.WriteLine("Button was clicked (2)");
        button.Click += (sender, e) => Console.WriteLine("Button was clicked (3)");

        Console.WriteLine("Press Space to click the button.");
        while (Console.ReadKey(true).Key != ConsoleKey.Spacebar) { }

        button.ClickButton();
    }
}
