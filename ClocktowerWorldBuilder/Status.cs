namespace ClocktowerWorldBuilder;

public abstract record Status
{
    public abstract Timestamp DeathDay();
    public abstract bool AliveOn(int day, int order);
    public abstract bool AliveOn(Timestamp timestamp);
}
public record Death(
    Timestamp Timestamp,
    bool ByDemon
): Status
{
    public override Timestamp DeathDay()=> Timestamp;
    public override bool AliveOn(Timestamp timestamp)
    {
        return AliveOn(timestamp.Day, timestamp.Order);
    }
    public override bool AliveOn(int day, int order)
    {
        if (day < Timestamp.Day)
        {
            return true;
        }
        if (day == Timestamp.Day)
        {
            return Timestamp.Order > order;
        }

        return false;
    }
};

public record Alive : Status
{
    public override Timestamp DeathDay()=> Timestamp.FOREVER;
    public override bool AliveOn(int day, int order)
    {
        return true;
    }

    public override bool AliveOn(Timestamp timestamp)
    {
        return true;
    }
}