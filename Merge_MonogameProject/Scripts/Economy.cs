//holds the player amount of gears
public static class MoneyBank
{
    public static long Gears { get; private set; } = 10;
    public static void Add(long amount)
    {
        if (amount <= 0) return;
        Gears += amount;
    }
    public static bool Spend(long amount)
    {
        if (amount <= 0) return false;
        if (Gears < amount) return false;
        Gears -= amount;
        return true;
    }
}