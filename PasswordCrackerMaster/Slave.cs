using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PasswordCrackerMaster
{
	internal class Slave
	{
		internal void connect(string host, int port)
		{
			TcpClient client = new TcpClient(host, port);

			Stream ns = client.GetStream();

			StreamReader sr = new StreamReader(ns);
			StreamWriter sw = new StreamWriter(ns);

			sw.AutoFlush = true;

			//send request to the server
			sw.WriteLine("chunk");

			//read response from the server
			String response = sr.ReadLine();

			List<String> chunk = JsonSerializer.Deserialize<List<String>>(response);

			//Now you have recieved the chunk of 100000 words from the server 
			//start caracking
			RunCracker(chunk);

			//print whatever you got from server
			Console.WriteLine(chunk);

			Console.ReadKey();
		}

		private void RunCracker(List<string> chunk)
		{
			//you can start your stop wach here if you will
			//Console.WriteLine(chunk[0]);
			CheckWordWithVariations(chunk[0], "Vibeke:EQ91A67FOpjss4uW8kV570lnSa0=");
		}

		private String CheckWordWithVariations(String dictionaryEntry, String userInfos)
		{



			return "";
		}
	}
}
