using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Collections;

namespace LogUtility
{
    public class LogHelper
    {
        public void Log(string content)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "Log";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            string filePath = dir + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";

            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                content = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + content;
                sw.WriteLine(content);
                sw.Flush();
                sw.Close();
            }
        }

        public void LogException(Exception ex)
        {
            Log(GetFullErrorMessage(ex));
        }

        public string GetFullErrorMessage(Exception ex)
        {
            var exStack = new System.Collections.ArrayList();
            for (Exception e = ex; e != null; e = e.InnerException)
            {
                exStack.Add(e);
            }
            var sb = new System.Text.StringBuilder();
            for (int i = exStack.Count - 1; i >= 0; i--)
            {
                if (i < exStack.Count - 1)
                {
                    sb.Append("\r\n");
                }

                Exception e = (Exception)exStack[i];

                sb.Append("[" + exStack[i].GetType().Name);

                // Display the error code if there is one
                if ((e is System.Runtime.InteropServices.ExternalException) && ((System.Runtime.InteropServices.ExternalException)e).ErrorCode != 0)
                {
                    sb.Append(" (0x" + (((System.Runtime.InteropServices.ExternalException)e).ErrorCode).ToString("x") + ")");
                }

                // Display the message if there is one 
                if (e.Message != null && e.Message.Length > 0)
                {
                    sb.Append(": " + e.Message);
                }

                sb.Append("]\r\n");

                // Display context if there is any
                if (e.Data != null)
                {
                    sb.AppendLine("Data:");
                    foreach (var key in e.Data.Keys)
                    {
                        var obj = e.Data[key];
                        sb.AppendLine(string.Format("\t{0} = {1}", key, (obj == null ? "<null>" : obj.ToString())));
                    }
                    sb.Append("\r\n");
                }

                // display the stack trace
                sb.AppendLine("Stack Trace:");
                sb.Append(e.StackTrace);
                sb.Append("\r\n");
            }
            return sb.ToString();
        }
    }
}