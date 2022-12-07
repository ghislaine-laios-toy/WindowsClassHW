using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Lesson3Com
{
    [Guid("618D3557-18DE-420C-B812-5C1E730419E4")]
    [ComVisible(true)]
    public interface ITransaction
    {
        void Connect(string connectString);
        void Disconnect();
        string GetVersion();
        string Add(int a, int b);
        string Multi(int a, int b);

    }

    [Guid("1B864194-0C62-45EB-BB8D-1CEF19501524")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Description("Demo transaction 1")]
    public class DemoTransaction1 : ITransaction
    {
        public void Connect(string connectString)
        {
        }

        public void Disconnect()
        {
        }

        public string GetVersion()
        {
            return "0.0.1";
        }

        public string Add(int a, int b)
        {
            return $"{a}+{b}={a+b}";
        }

        public string Multi(int a, int b)
        {
            throw new NotImplementedException();
        }
    }

    [Guid("F5F9A628-4195-4A8D-931D-12B3E0F8B55D")]
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Description("Demo transaction 2")]
    public class DemoTransaction2 : ITransaction
    {
        public void Connect(string connectString)
        {
            
        }

        public void Disconnect()
        {

        }

        public string GetVersion()
        {
            return "0.0.1";
        }

        public string Add(int a, int b)
        {
            throw new NotImplementedException();
        }

        public string Multi(int a, int b)
        {
            return $"{a}*{b}={a * b}";
        }
    }
}
