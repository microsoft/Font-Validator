using System;

namespace NS_GMath
{
    public class InfoConnect
    {
        /*
         *        MEMBERS
         */
        bool isConnect;
        bool isTangent;
        /*
         *        PROPERTIES
         */
        public bool IsConnect
        {
            get { return this.isConnect; }
            set { this.isConnect=value; }
        }
        public bool IsTangent
        {
            get { return this.isTangent; }
            set { this.isTangent=value; }
        }
        /*
         *        CONSTRUCTORS
         */
        public InfoConnect(bool isConnect, bool isTangent)
        {
            this.isConnect=isConnect;
            this.isTangent=isTangent;
        }
        public InfoConnect(InfoConnect ic)
        {
            this.isConnect=ic.IsConnect;
            this.isTangent=ic.IsTangent;
        }
    }
}