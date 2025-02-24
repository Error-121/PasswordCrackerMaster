// See https://aka.ms/new-console-template for more information
using PasswordCrackerMaster;
using System.Net;

Console.WriteLine("Hello, World!");

namespace PasswordCrackerMaster
{
	class Program
	{
		static void Main(string[] args)
		{

			Master master = new Master();

			master.Listen(IPAddress.Loopback, 6789);
		}
	}
}

namespace PasswordCrackerSlave
{
	class Program
	{
		static void Main(string[] args)
		{

			Slave slave = new Slave();

			slave.connect("localhost", 6789);


		}
	}

}