namespace Services;

public interface IService
{


   int GetHour();
   void Count24hr();
   void BakeNewfood(int newFood);
   void EatFood(int foodportion);
   bool CloseDown();
   void BakerRest();
   bool TimeToBake();
   bool TimeToEat();
   void EaterLeaving();

}