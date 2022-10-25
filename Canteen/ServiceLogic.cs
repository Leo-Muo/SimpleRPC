namespace Servers;

using NLog;
using System;
using Timer = System.Timers.Timer;

class ServiceLogic
{
     int hour=1;
     int foodstorage = 0;
     int _daysClosed = 0;  
     int _profit = 0;
     double _reputation = 0;
     bool _isClosed = false;
    int _badBusinessCounter = 0;
    int day = 1;
    int _numSec = 2;

    private Logger log = LogManager.GetCurrentClassLogger();
    private readonly Object accessLock = new Object();

    public ServiceLogic(){
        Count24hr();
    }

    public void BakeNewfood(int newFood){
        lock (accessLock){
            if(TimeToBake()){
                    foodstorage += newFood;
                    log.Info($"Baker baked: {newFood} portions of food");
            }   
        }
    }

    public bool TimeToBake(){
        lock (accessLock){
            return (hour >= 7 && hour <= 17);
        }
    }
     public void BakerRest(){
       log.Info($"Baker going to rest... ");
    }

    public void EatFood(int eat){
        lock (accessLock){
            if (TimeToEat())
            {
                if(foodstorage >= eat){
                        foodstorage -= eat;
                        log.Info($"Eater   ate: {eat} portions of food");
                        IncreaseProfit(eat);
                        IncreaseReputation(eat);
                    
                }else{
                    DecreaseReputation(eat);
                }
            }
        }
    }

    public bool TimeToEat(){
        lock (accessLock){
            return (hour > 10 && hour < 19);
        }
    }
    
    public void EaterLeaving(){
        log.Info("Eater leaving the bakery.... ");
    }

    private void ThrowAwayFood(){
        lock (accessLock)
        {
            DecreaseProfit(foodstorage);
            foodstorage = 0;
        }
    }

    private void IncreaseProfit(int food){
        _profit += (int)Math.Ceiling((double)food/2.0);
    }

    private void DecreaseProfit(int food){
         _profit -= (int)Math.Ceiling((double)food);
    }

    private void IncreaseReputation(int foodeaten){
        int rank = (int)Math.Ceiling((double)foodeaten/2.0);
        _reputation += (foodeaten*rank);
    }

    private void DecreaseReputation(int foodeaten){
        int rank = (int)Math.Ceiling((double)foodeaten/2.0);
        _reputation -= (foodeaten*rank);
    }

    
    public void Count24hr(int interval = 2000){

        var timer = new Timer(interval){
            AutoReset = true
        };
        timer.Elapsed += (_,_) =>
        {
            HourChecker();
        };
        timer.Enabled = true;
    }


    private void HourChecker(){
        lock (accessLock){
         if (hour == 19 && !_isClosed){
                log.Info($"Food thrownaway: {foodstorage}");
                ThrowAwayFood();
            }

            if (hour == 24){ 
                log.Info($"-------------------- Night {day} is over" + 
                         $"| Profit: {_profit} | Reputation: {Math.Round(_reputation,2)} stars --------------------");
                log.Info(" ");

                if(_profit < 0 || _reputation < 0){
                    _badBusinessCounter++;
                    if(_badBusinessCounter == 2){
                        _isClosed = true;
                        FreshStart();
                        log.Info(" Canteen is Closed --------------------");
                        log.Info(" ");
                        _badBusinessCounter = 0;
                    }
                }else{
                    _badBusinessCounter = 0;
                }

                if(_daysClosed < 1 && _isClosed){
                        _daysClosed++;
                }else{
                    _daysClosed = 0;
                    _isClosed = false;
                }

                hour=1;
                day++;
                log.Info($"-------------------- Night {day} is Starts --------------------");
            }
            else{ hour++; }
        }
    }


    public bool CloseDown(){
        lock (accessLock){
            return _isClosed;
        }
    }

    private void FreshStart(){
            foodstorage = 0;
            _profit = 0;
            _reputation = 0;
    }

     public int GetHour(){ 
      lock (accessLock){
        return hour;
      }
    }
    
}