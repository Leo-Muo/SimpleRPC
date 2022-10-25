namespace Clients;

using Services;
using Microsoft.Extensions.DependencyInjection;
using SimpleRpc.Serialization.Hyperion;
using SimpleRpc.Transports;
using SimpleRpc.Transports.Http.Client;
using NLog;

class Eater
{
	Logger log = LogManager.GetCurrentClassLogger();

	private void ConfigureLogging()
	{
		var config = new NLog.Config.LoggingConfiguration();

		var console =
			new NLog.Targets.ConsoleTarget("console")
			{
				Layout = @"${date:format=HH\:mm\:ss}|${level}| ${message} ${exception}"
			};
		config.AddTarget(console);
		config.AddRuleForAllLevels(console);

		LogManager.Configuration = config;
	}


    private void Run()
    {
        ConfigureLogging();

        while (true)
        {
            try
            {
                var sc = new ServiceCollection();
				sc
					.AddSimpleRpcClient(
						"service", 
						new HttpClientTransportOptions
						{
							Url = "http://127.0.0.1:5000/simplerpc",
							Serializer = "HyperionMessageSerializer"
						}
					)
					.AddSimpleRpcHyperionSerializer();

				sc.AddSimpleRpcProxy<IService>("service");

				var sp = sc.BuildServiceProvider();
				var service = sp.GetService<IService>();

                while(true){
                    
					var rnd = new Random();
					int foodportion = rnd.Next(1,10);            

					if (!service.CloseDown())
					{
						if(service.TimeToEat()){
							log.Info($"Eater going to eat {foodportion} portion of food");
							service.EatFood(foodportion);
							log.Info("---");
						}else{
							service.EaterLeaving();
							if(service.GetHour() > 18){
								Thread.Sleep (TimeSpan.FromSeconds((double)((24-service.GetHour())+11)*2));
							}else{
								Thread.Sleep (TimeSpan.FromSeconds((double)(11-service.GetHour())*2));
							}
						}
						
                    	Thread.Sleep(1500);
					}else{
						Thread.Sleep (TimeSpan.FromSeconds(4.0));
					}
                }   
            }
            catch (Exception e)
            {
				log.Warn(e, "Unhandled exception caught. Will restart main loop.");
				Thread.Sleep(2000);
            }
        }
    }



    static void Main(string[] args)
    {
		var self = new Eater();
		self.Run();
	}
    
}