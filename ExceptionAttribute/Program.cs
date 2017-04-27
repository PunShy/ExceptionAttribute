using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceptionAttribute
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var loanValidator = new LoanValidator();
                loanValidator.CheckLoanApplication();
            }
            catch (Exception ex)
            {
                string exceptionMessage = ExceptionHelper.GetDefaultDetailMessage(ex);
                Console.Write(exceptionMessage);
                Console.Read();
            }
        }
    }

    public class LoanValidator
    {
        [MethodDescription("嘗試1")]
        public void CheckLoanApplication()
        {
            kk1();            
        }

        [MethodDescription("嘗試2")]
        private void kk1()
        {
            kk2();
            throw new Exception("kk1 error");
        }

        [MethodDescription("嘗試3")]
        private void kk2()
        {
            kk3();
            throw new Exception("kk2 error");
        }

        [MethodDescription("嘗試4")]
        private void kk3()
        {
            kk4();
            throw new Exception("kk3 error");
        }

        [MethodDescription("嘗試5")]
        private void kk4()
        {
            throw new Exception("kk4 error");
        }

    }

    #region Method用Attribute抓Exception訊息

    public class MethodDescription : Attribute, IExceptionMethod
    {
        public MethodDescription(string description)
        {
            this.Description = description;
        }
        public string Description { get; set; }
    }

    public interface IExceptionMethod
    {
        string Description { get; set; }
    }

    public class ExceptionHelper
    {
        /// <summary>
        /// 支援更多自訂 Attribute 的方法描述
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetDetailMessage<T>(Exception ex) where T: IExceptionMethod
        {
            StringBuilder messageBuilder = new StringBuilder();

            int step = 1;

            var attributes = GetStackTraceSteps<T>(ex);
            if (attributes != null && attributes.Count > 0)
                messageBuilder.AppendLine(string.Format
                ("Sorry there is a problem while processing step {0}:", attributes.Count));
            foreach (var attribute in attributes)
            {
                messageBuilder.Append(string.Format
                ("Step {0}: {1}", step.ToString(), attribute.Description));
                messageBuilder.AppendLine();
                step++;
            }
            messageBuilder.AppendLine();

            string formatedMessage = string.Format("{0}Error Description : {1}",
                                                    messageBuilder.ToString(),
                                                    ex.Message
                                                    );
            return formatedMessage;
        }
        /// <summary>
        /// 預設自訂 Attribute 的方法描述
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static string GetDefaultDetailMessage(Exception ex)
        {
            return GetDetailMessage<MethodDescription>(ex);
        }

        /// <summary>
        /// Extrace the custom attribute details from the stacktrace
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ex"></param>
        /// <returns></returns>
        public static List<T> GetStackTraceSteps<T>(Exception ex)
        {
            List<T> traceSteps = new List<T>();
            Type attributeType = typeof(T);
            StackTrace st = new StackTrace(ex);
            if (st != null && st.FrameCount > 0)
            {
                for (int index = st.FrameCount - 1; index >= 0; index--)
                {
                    var attribute = st.GetFrame(index).
                        GetMethod().GetCustomAttributes(attributeType, false).FirstOrDefault();
                    if (attribute != null)
                    {
                        traceSteps.Add((T)attribute);
                    }
                }
            }
            return traceSteps;
        }
    }
    #endregion
}
