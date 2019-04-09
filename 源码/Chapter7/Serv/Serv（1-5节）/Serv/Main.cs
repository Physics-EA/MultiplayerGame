using System;

namespace Serv
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			ServNet servNet = new ServNet();
			servNet.Start("127.0.0.1",1234);
			Console.ReadLine();
		}
	}
}
