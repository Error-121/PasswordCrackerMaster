using PasswordCrackerCentralized.model;
using PasswordCrackerCentralized.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PasswordCrackerMaster
{
	public class Slave
	{
		bool _passwordRequest = true;
        private readonly HashAlgorithm _messageDigest;
        private List<string> passwords;
        private List<string> chunk;

        public Slave()
        {
        }

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

                passwords = JsonSerializer.Deserialize<List<string>>(passwordResponse);

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

            chunk = JsonSerializer.Deserialize<List<String>>(response);
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

        public void RunCracking()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            var userInfos =
                passwords;
            Console.WriteLine("passwd opeend");

            List<UserInfoClearText> result = new List<UserInfoClearText>();

            foreach (var dictionaryEntry in chunk)
            {
                IEnumerable<UserInfoClearText> partialResult = CheckWordWithVariations(dictionaryEntry, userInfos);
                result.AddRange(partialResult);
            }

            stopwatch.Stop();
            Console.WriteLine(string.Join(", ", result));
            Console.WriteLine("Out of {0} password {1} was found ", userInfos.Count, result.Count);
            Console.WriteLine();
            Console.WriteLine("Time elapsed: {0}", stopwatch.Elapsed);
        }

        private string ReplaceSpecialCharacters(string input)
        {
            return input.Replace("oe", "ø")
                        .Replace("ae", "æ")
                        .Replace("aa", "å");
        }

        private String CheckWordWithVariations( dictionaryEntry, List<UserInfo> userInfos)
		{
            List<UserInfoClearText> result = new List<UserInfoClearText>(); //might be empty

            dictionaryEntry = ReplaceSpecialCharacters(dictionaryEntry);

            String possiblePassword = dictionaryEntry;
            IEnumerable<UserInfoClearText> partialResult = CheckSingleWord(userInfos, possiblePassword);
            result.AddRange(partialResult);

            String possiblePasswordUpperCase = dictionaryEntry.ToUpper();
            IEnumerable<UserInfoClearText> partialResultUpperCase = CheckSingleWord(userInfos, possiblePasswordUpperCase);
            result.AddRange(partialResultUpperCase);

            String possiblePasswordCapitalized = StringUtilities.Capitalize(dictionaryEntry);
            IEnumerable<UserInfoClearText> partialResultCapitalized = CheckSingleWord(userInfos, possiblePasswordCapitalized);
            result.AddRange(partialResultCapitalized);

            String possiblePasswordReverse = StringUtilities.Reverse(dictionaryEntry);
            IEnumerable<UserInfoClearText> partialResultReverse = CheckSingleWord(userInfos, possiblePasswordReverse);
            result.AddRange(partialResultReverse);

            for (int i = 0; i < 100; i++)
            {
                String possiblePasswordEndDigit = dictionaryEntry + i;
                IEnumerable<UserInfoClearText> partialResultEndDigit = CheckSingleWord(userInfos, possiblePasswordEndDigit);
                result.AddRange(partialResultEndDigit);
            }

            for (int i = 0; i < 100; i++)
            {
                String possiblePasswordStartDigit = i + dictionaryEntry;
                IEnumerable<UserInfoClearText> partialResultStartDigit = CheckSingleWord(userInfos, possiblePasswordStartDigit);
                result.AddRange(partialResultStartDigit);
            }

            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    String possiblePasswordStartEndDigit = i + dictionaryEntry + j;
                    IEnumerable<UserInfoClearText> partialResultStartEndDigit = CheckSingleWord(userInfos, possiblePasswordStartEndDigit);
                    result.AddRange(partialResultStartEndDigit);
                }
            }

            return result;
        }

        private IEnumerable<UserInfoClearText> CheckSingleWord(IEnumerable<UserInfo> userInfos, String possiblePassword)
        {
            char[] charArray = possiblePassword.ToCharArray();
            byte[] passwordAsBytes = Array.ConvertAll(charArray, PasswordFileHandler.GetConverter());

            byte[] encryptedPassword = _messageDigest.ComputeHash(passwordAsBytes);
            //string encryptedPasswordBase64 = System.Convert.ToBase64String(encryptedPassword);

            List<UserInfoClearText> results = new List<UserInfoClearText>();

            foreach (UserInfo userInfo in userInfos)
            {
                if (CompareBytes(userInfo.EntryptedPassword, encryptedPassword))  //compares byte arrays
                {
                    results.Add(new UserInfoClearText(userInfo.Username, possiblePassword));
                    Console.WriteLine(userInfo.Username + " " + possiblePassword);
                }
            }
            return results;
        }

        /// <summary>
        /// Compares to byte arrays. Encrypted words are byte arrays
        /// </summary>
        /// <param name="firstArray"></param>
        /// <param name="secondArray"></param>
        /// <returns></returns>
        private static bool CompareBytes(IList<byte> firstArray, IList<byte> secondArray)
        {
            //if (secondArray == null)
            //{
            //    throw new ArgumentNullException("firstArray");
            //}
            //if (secondArray == null)
            //{
            //    throw new ArgumentNullException("secondArray");
            //}
            if (firstArray.Count != secondArray.Count)
            {
                return false;
            }
            for (int i = 0; i < firstArray.Count; i++)
            {
                if (firstArray[i] != secondArray[i])
                    return false;
            }
            return true;
        }
    }
}
