using System;
using System.Text;

namespace NS_Glyph
{
    public class ExceptionGlyph : System.Exception
    {
        /*
         *        CONSTRUCTORS
         */
        string strMessage;

        private ExceptionGlyph()
        {
        }
        public ExceptionGlyph(string nameClass, string nameMethod, string strDetails)
        {
            StringBuilder sb=new StringBuilder("Exception Glyph:");
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