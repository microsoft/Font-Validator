using System;
using System.Text;
using System.Collections;

namespace NS_ValCommon
{
    public class DIActionContainer
    {
        /*
         *        MEMBERS
         */
        protected DIAction dia;

        /*
         *        PROPERTIES 
         */
        public DIAction DIA
        {
            get {return this.dia;}
            set {this.dia=value;}
        }

        /*
         *        CONVERSIONS
         */
        public static implicit operator DIAction(DIActionContainer diac)
        {
            return diac.DIA;
        }

        /*
         *        CONSTRUCTORS
         */
        public DIActionContainer(params DIAction[] dias)
        {
            if (dias!=null)
            {
                foreach (DIAction dia in dias)
                {
                    if (dia!=null)
                    {
                        if (this.dia==null)
                        {
                            this.dia=dia;
                        }
                        else
                        {
                            this.dia=System.Delegate.Combine(this.dia,dia) as DIAction;
                        }
                    }    
                }
            }
        }    

        /*
         *        METHODS
         */
        public bool AddDeleg(DIAction diaToAdd)
        {
            if (diaToAdd==null)
                return true;
            if (this.dia==null)
            {
                this.dia=diaToAdd;
            }
            else
            {
                this.dia=(MulticastDelegate.Combine(this.dia,diaToAdd)) as DIAction;
            }
            return true;
        }
        
        public bool AddDeleg(object obj, string nameDIA)
        {
            DIAction dia=DIActionBuilder.DIA(obj,nameDIA);
            return (this.AddDeleg(dia));
        }

        public bool RemoveDeleg(object objToRemove)
        {
            if ((objToRemove==null)||(this.dia==null))
                return false;                  
            ArrayList diasToRemove=null;
            Delegate[] delegs=this.dia.GetInvocationList();            
            foreach (Delegate deleg in delegs)
            {
                object obj=deleg.Target;
                if (Object.ReferenceEquals(obj,objToRemove))
                {
                    if (diasToRemove==null)
                    {
                        diasToRemove=new ArrayList();
                    }
                    diasToRemove.Add(deleg);
                }
            }
            if (diasToRemove==null)
                return false;
            bool res=true;
            foreach (Delegate diaToRemove in diasToRemove)
            {
                res=res && this.RemoveDeleg(diaToRemove);
            }
            return res;
        }

        public bool RemoveDeleg(DIAction diaToRemove)
        {
            if (diaToRemove==null)
                return false;
            if (this.dia==null)
                return false;
            this.dia=(MulticastDelegate.Remove(this.dia,diaToRemove)) as DIAction;
            return true;
        }
        
        // wrapper - reports errors
        public void SendInfo(ValInfoBasic info)
        {
            if (this.dia!=null)
            {
                dia(info);
            }
        }    
    
        public void SendInfoOneByOne(ValInfoBasic info, out string strErrMessage)
        {
            strErrMessage=null;
            StringBuilder sb=null;
            if (this.dia==null)
                return;
            Delegate[] delegs=this.dia.GetInvocationList();            
            foreach (Delegate deleg in delegs)
            {
                try
                {
                    DIAction dia=(DIAction)deleg;
                    dia(info);
                }
                catch
                {
                    if (sb==null)
                    {
                        sb=new StringBuilder();
                    }
                    sb.Append(deleg.Target.GetType());
                    sb.Append("\r\n");
                }
            }
            strErrMessage=sb.ToString();
        }
    }
}
