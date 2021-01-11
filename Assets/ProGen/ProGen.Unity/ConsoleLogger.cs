using System.Diagnostics;
using System.Reflection;

namespace ProGen.Unity
{
    public static class ConsoleLogger
    {
        public static void Debug(string msg)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            MethodBase method = sf.GetMethod();
            string className = method.DeclaringType.Name;
            string namespce = method.DeclaringType.Namespace;

            UnityEngine.Debug.Log(namespce + "." + className + "." + method.Name + ": " + msg);
        }
    }
}