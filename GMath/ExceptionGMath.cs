using System;
using System.Text;

namespace NS_GMath
{
    public class ExceptionGMath : System.Exception
    {
        /*
         *        CONSTRUCTORS
         */
        string strMessage;

        private ExceptionGMath()
        {
        }
        public ExceptionGMath(string nameClass, string nameMethod, string strDetails)
        {
            StringBuilder sb=new StringBuilder("Exception GMath:");
            if (nameClass!=null)
            {
                sb.Append(" class: "+nameClass);
            }
            if (nameMethod!=null)
            {
                sb.Append(" method: "+nameMethod);
            }
            if (strDetails!=null)
            {
                sb.Append(" details: "+strDetails);
            }
            this.strMessage=sb.ToString();
        }
        override public string Message
        {
            get { return this.strMessage; }
        }

        /*
         *        PROPERTIES
         */

    }
}