using PasswordCrackerCentralized.model;
using PasswordCrackerCentralized.util;
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
		bool _passwordRequest = true;
		internal void connect(string host, int port)
		{
			TcpClient client = new TcpClient(host, port);

			Stream ns = client.GetStream();

			StreamReader sr = new StreamReader(ns);
			StreamWriter sw = new StreamWriter(ns);

			sw.AutoFlush = true;

			while (_passwordRequest)
			{
                sw.WriteLine("password");

                string passwordResponse = sr.ReadLine();

                List<string> passwords = JsonSerializer.Deserialize<List<string>>(passwordResponse);

                if (passwords == null || passwords.Count == 0)
                {
                    Console.WriteLine("No valid passwords received. Retrying...");
                    continue; // Prøver igen, hvis passwords er tom eller null
                }

                // Når vi har modtaget passwords, stopper vi denne while-løkke
                _passwordRequest = false;
            }

            //send request to the server
            sw.WriteLine("chunk");

            //read response from the server
            String response = sr.ReadLine();

            List<String> chunk = JsonSerializer.Deserialize<List<String>>(response);
            //Now you have recieved the chunk of 100000 words from the server 

            if (chunk == null || chunk.Count == 0)
            {
                throw new Exception("Failed to receive a valid chunk from the server.");
            }
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

        private string ReplaceSpecialCharacters(string input)
        {
            return input.Replace("oe", "ø")
                        .Replace("ae", "æ")
                        .Replace("aa", "å");
        }

        private String CheckWordWithVariations(String dictionaryEntry, String userInfos)
		{
            List<UserInfoClearText> result = new List<UserInfoClearText>(); //might be empty

            dictionaryEntry = ReplaceSpecialCharacters(dictionaryEntry);

            return "";
		}
	}
}
