using System;

namespace NS_ValCommon
{
    public class DIActionOnCondBuilder
    {
        private DICond dic;
        private DIAction dia;


        public DIActionOnCondBuilder(DICond dic, DIAction dia)
        {
            this.dic=dic;
            this.dia=dia;
        }
        public void DIAFunc(ValInfoBasic info)
        {
            if (this.dic(info))
            {
                this.dia(info);
            }
        }
        public DIAction DIA
        {
            get 
            {
                if ((this.dic==null)||(this.dia==null))
                    return null;
                return DIActionBuilder.DIA(this,"DIAFunc");
            }
        }
    }
}