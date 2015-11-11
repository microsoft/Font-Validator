using System;
using System.Reflection;
using System.Diagnostics;

namespace NS_ValCommon
{
    public class DICondBuilder
    {
        object obj;
        Type typeObj;
        private object[] pars;
        MethodInfo infoMethod;

        private DICondBuilder()
        {
            this.obj=null;
            this.infoMethod=null;
            this.typeObj=null;
            this.infoMethod=null;
        }

        public DICondBuilder(Type typeObj, string nameMethod, params object[] pars)
            : this()

        {
            // for static methods
            this.typeObj=typeObj;
            this.pars=new object[pars.Length+1];
            
            Type[] types=new Type[pars.Length+1];
            types[0]=typeof(ValInfoBasic);
            for (int iPar=0; iPar<pars.Length; iPar++)
            {
                this.pars[iPar+1]=pars[iPar];
                types[iPar+1]=pars[iPar].GetType();
            }
            // for debug only - begin
            this.infoMethod=this.typeObj.GetMethod(nameMethod);
            System.Reflection.ParameterInfo[] infoPars= this.infoMethod.GetParameters();
            // for debug only - end

            this.infoMethod=this.typeObj.GetMethod(nameMethod,types);
            if (this.infoMethod!=null)
            {
                if (this.infoMethod.ReturnType!=typeof(bool))
                    this.infoMethod=null;
            }
            Debug.Assert(this.infoMethod!=null);            
        }


        public DICondBuilder(object obj, string nameMethod, params object[] pars)
            : this(obj.GetType(), nameMethod, pars)
        {
            // for instance methods
            this.obj=obj;
        }

        private bool DICFunc(ValInfoBasic info)
        {
            this.pars[0]=info;
            if (this.obj!=null)
            {
                return (bool)(this.infoMethod.Invoke(this.obj,this.pars));
            }
            else
            {
                return (bool)(this.infoMethod.Invoke(this.typeObj,this.pars));
            }
        }

        public DICond DIC()
        {
            //get
            //{
                if (this.infoMethod==null)
                    return null;
                return (Delegate.CreateDelegate(typeof(DICond),this,"DICFunc") as DICond);
            //}
        }

        static public DICond DIC(object obj, string nameDIC)
        {
            if ((obj==null) || (nameDIC==null))
            return null;
            return ((Delegate.CreateDelegate(typeof(DICond),obj,nameDIC)) as DICond);
        }
    }
}