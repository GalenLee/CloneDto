using System;
using System.Security.Cryptography;
using System.Text;

namespace HashRelated
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var md5 = MD5.Create())
            {
                var value = "Joe";
                var bytes = Encoding.UTF8.GetBytes(value);    
                var hash = md5.ComputeHash(bytes);            
                var hashString = BitConverter.ToString(hash);
                Console.WriteLine(hashString);
                Console.WriteLine();
                var result = BitConverter.ToUInt64(hash, 0);  
                Console.WriteLine(result);
                Console.WriteLine();
                var limited = result % 10;
                Console.WriteLine(limited);
                Console.WriteLine();
      
                var a = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
                var b = (long)(BitConverter.ToUInt64(hash, 0) % 10);
                Console.WriteLine(b);
                Console.ReadLine();
            }
        }
    }
}
