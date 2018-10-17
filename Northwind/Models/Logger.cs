using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Northwind.Models
{
    public class Logger
    {
        // Write to log file defined in web.config
        public static void Log(string message)
        {
            Trace.WriteLine(message);
            Trace.Flush();
        }

        // Write to log file defined in web.config
        public static void Log(Exception ex, string message)
        {
            var annotatedException = new Exception(message, ex);
            Trace.WriteLine(ex.ToString());
            Trace.Flush();
        }

        /// <summary>
        /// Log error to Elmah
        /// </summary>
        public static void LogError(Exception ex, string contextualMessage = null)
        {
            /*
            try
            {
                // log error to Elmah
                if (contextualMessage != null)
                {
                    // log exception with contextual information that's visible when 
                    // clicking on the error in the Elmah log
                    var annotatedException = new Exception(contextualMessage, ex);
                    if (Elmah.ErrorSignal.FromCurrentContext() != null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(annotatedException, HttpContext.Current);
                    }
                    else
                    {
                        Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(annotatedException));
                    }
                }
                else
                {
                    if (Elmah.ErrorSignal.FromCurrentContext() != null)
                    {
                        Elmah.ErrorSignal.FromCurrentContext().Raise(ex, HttpContext.Current);
                    }
                    else
                    {
                        Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    }
                }
            }
            catch (Exception)
            {
                // uh oh! just keep going
            }
            */
       }

    }
}