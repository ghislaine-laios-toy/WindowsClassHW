using Lesson3Com;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lesson3ComClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(DemoCom.Add("1B864194-0C62-45EB-BB8D-1CEF19501524", "", 1, 2));
            Console.WriteLine(DemoCom.Multi("F5F9A628-4195-4A8D-931D-12B3E0F8B55D", "", 2, 3));
            Console.ReadLine();
        }
    }

    class DemoCom
    {
        public static ITransaction CreateTransaction(string _guid, string connectString)
        {
            ITransaction iTransaction = null;
            Guid guid = new Guid(_guid);
            Type transactionType = Type.GetTypeFromCLSID(guid);
            object transaction = Activator.CreateInstance(transactionType);
            iTransaction = transaction as ITransaction;
            iTransaction.Connect(connectString);
            return iTransaction;
        }

        public static string Add(string guid, string connectionStr, int a, int b)
        {
            ITransaction transaction = CreateTransaction(guid, connectionStr);
            return transaction.Add(a, b);
        }

        public static string Multi(string guid, string connectionStr, int a, int b)
        {
            ITransaction transaction = CreateTransaction(guid, connectionStr);
            return transaction.Multi(a, b);

        }
    }
}
