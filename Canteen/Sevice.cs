namespace Servers;

using Services;

public class Service : IService 
{
    private ServiceLogic logic = new ServiceLogic();


    public void Count24hr(){
        logic.Count24hr();
    }

    public bool TimeToBake(){
        return logic.TimeToBake();
    }

     public bool TimeToEat(){
         return logic.TimeToEat();
    }

    public void BakeNewfood(int newFood){
		    logic.BakeNewfood(newFood);
    }

    public void EatFood(int newFood){
		    logic.EatFood(newFood);
    }

    public bool CloseDown(){
        return logic.CloseDown();
    }

    public void BakerRest(){
      logic.BakerRest();
    }

    public void EaterLeaving(){
      logic.EaterLeaving();
    }

    public int GetHour(){
        return logic.GetHour();
    }


}