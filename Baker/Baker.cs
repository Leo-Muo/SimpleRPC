namespace Clients;

using Services;
using Microsoft.Extensions.DependencyInjection;
using SimpleRpc.Serialization.Hyperion;
using SimpleRpc.Transports;
using SimpleRpc.Transports.Http.Client;
using NLog;


class Baker
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
				sc.AddSimpleRpcClient(
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
					int food = rnd.Next(1,10);
                    
					if (!service.CloseDown())
					{
						if(service.TimeToBake()){
							log.Info($"Baker going to bake {food} portion of food");
							service.BakeNewfood(food);
							log.Info("---");
						}else{
							service.BakerRest();
							if(service.GetHour() > 17){
								Thread.Sleep (TimeSpan.FromSeconds((double)((24-service.GetHour())+7)*2));
							}else{
								Thread.Sleep (TimeSpan.FromSeconds((double)(7-service.GetHour())*2));
							}
						}

						Thread.Sleep(2500);
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
        var self = new Baker();
		self.Run();
	}
}