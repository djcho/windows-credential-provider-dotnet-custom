namespace Penta.EeWin.Cp.Base
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class Log
    {
        public static void LogText(string text)
        {
            try
            {
                File.AppendAllText(@"C:\users\djcho\desktop\CSharpCredentialProvider.log", text + Environment.NewLine);
            }
            catch (System.Exception)
            {
                try
                {
                    Console.WriteLine("(Skipped logging)");
                }
                catch (System.Exception)
                {
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void LogMethodCall()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(1);

            var methodBase = sf.GetMethod();
            LogText(methodBase.DeclaringType?.Name + "::" + methodBase.Name);
        }
    }
}
