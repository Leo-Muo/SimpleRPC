namespace Servers;

using System.Net;
using NLog;
using SimpleRpc.Transports;
using SimpleRpc.Transports.Http.Server;
using SimpleRpc.Serialization.Hyperion;
using Services;

public class Server{

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


    public static void Main(string[] args)
    {
		var self = new Server();
		self.Run(args);
    }

    private void Run(string[] args) 
	{
		ConfigureLogging();

		//indicate server is about to start
		log.Info("Server is about to start");
		
        //start the server
		StartServer(args);

		// ServiceLogic servicelog = new ServiceLogic();
		// Thread backgroundThread = new Thread ( () => servicelog.Count24hr() );
		// backgroundThread.IsBackground =true;
		// backgroundThread.Start();
		// backgroundThread.Join();
	}


    private void StartServer(string[] args)
	{
		///create web app builder
		var builder = WebApplication.CreateBuilder(args);

        //configure integrated server
		builder.WebHost.ConfigureKestrel(opts => {
			opts.Listen(IPAddress.Loopback, 5000);
		});

        //add SimpleRPC services
		builder.Services
			.AddSimpleRpcServer(new HttpServerTransportOptions { Path = "/simplerpc" })
			.AddSimpleRpcHyperionSerializer();

        //add our custom services
		builder.Services.AddSingleton<IService, Service>();

        //build the server
		var app = builder.Build();

        //add SimpleRPC middleware
		app.UseSimpleRpcServer();
        app.Run();
	}

}
	