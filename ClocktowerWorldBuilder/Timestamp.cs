namespace ClocktowerWorldBuilder;

public record Timestamp(
   int Day,
   int Order
) : IComparable<Timestamp>
{
   public static Timestamp GENESIS = new Timestamp(-1, 0);
   public static Timestamp FOREVER = new Timestamp(9999999, 9999999);

   public Timestamp Before()
   {
      return new Timestamp(Day, Order-1);
   }
   public static  bool operator>=(Timestamp left, Timestamp right)
   {
      return left==right||(left > right);
   }
   public static  bool operator<=(Timestamp left, Timestamp right)
   {
      return left==right||(left < right);
   }
   public static  bool operator>(Timestamp left, Timestamp right)
   {
      return !(left < right) && left != right;
   }
   public static  bool operator<(Timestamp left, Timestamp right)
   {
      if (left.Day < right.Day)
      {
         return true;
      }

      if (left.Day == right.Day)
      {
         return left.Order < right.Order;
      }
      return false;
   }

   public int CompareTo(Timestamp? other)
   {
      if (ReferenceEquals(this, other)) return 0;
      if (other is null) return 1;
      if(this>other)
      {
         return 1;
      }
      if(this<other)
      {
         return -1;
      }

      return 0;
   }
};
   