using System;
using System.Diagnostics;
using System.Xml;

using OTFontFile;
using NS_ValCommon;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for ValidationInfo.
    /// </summary>
    public class ValidationInfo : ValInfoBasic
    {

        /*
         *        CONSTRUCTORS
         */
    /*
        public ValidationInfo(ValInfoType VIType, string StringName, OTTag tag, string stringTestName):
            base(VIType, StringName, null, "OTFontFile.ValStrings", "OTFontFile", tag, stringTestName)
        {
        }
    */

        /// <summary>Hardwires <c>OTFontFileVal.ValStrings</c> as
        /// the <c>nameFileErrs</c> and <c>OTFontFileVal</c> as
        /// the <c>nameAsmFileErrs</c> in the underlying
        /// <c>ValInfoBasic</c>.
        /// </summary>
        public ValidationInfo( ValInfoType VIType, 
                               string StringName, 
                               OTTag tag, 
                               string StringDetails, 
                               string stringTestName):
            base(VIType, StringName, StringDetails, 
                 "OTFontFileVal.ValStrings", "OTFontFileVal", 
                 tag, stringTestName)
        {
        }
        
        /// <summary>Does not appear to be called.</summary>
        public ValidationInfo(ValInfoType type, 
                              string stringName,
                              string stringValueUser,
                              string nameFileErrs,
                              string nameAsmFileErrs,
                              OTTag tagPrincipal,
                              string stringTestName):
            base(type,stringName,stringValueUser,nameFileErrs,
                 nameAsmFileErrs,tagPrincipal, stringTestName)
        {
        }

        /// <summary>Copy constructor</summary>
        public ValidationInfo(ValInfoBasic viBasic):
            base(viBasic)
        {
        }

        /*
         *        METHODS  -    wrappers for methods of ValInfoBasic (can be 
         *                    removed...)    
         */
        public ValInfoType GetValInfoType() {return this.m_Type;}
        public void SetValInfoType(ValInfoType mt) {this.m_Type=mt;}

        public OTTag GetOTTag() {return this.m_OTTagPrincipal;}
        public void SetOTTag(OTTag tt) { this.m_OTTagPrincipal = tt;}
        public void SetOTTag(uint tag) { this.m_OTTagPrincipal = tag;}

        /// <summary>In the special case that <c>m_NameAsmFileErrs</c> is
        /// <c>OTFontFileVal</c>, which I believe it always is, then
        /// read the string corresponding to <c>m_StringName</c> of the form
        /// <c>W1404: The LineGap value is less than the recommended value</c>
        /// from the hardcoded resource file <c>OTFontFileVal.ValStrings</c>
        /// and return a string consisting of substring starting at character
        /// 7, that is, after discarding the initial "W1404: ".
        ///
        /// If the resource is not present return <c>null</c>.
        /// If <c>m_NameAsmFileErrs != "OTFontFileVal</c>, then
        /// just return the value of the <c>ErrorID</c> property of the 
        /// base class.
        /// <p/>
        /// This routine may be redundant since the <c>ErrorID</c> 
        /// property already does a resource lookup on the resource.
        /// </summary>
        public string GetString()
        {
            string s = null;

            if ((object)this.m_StringName != null)
            {
                // if string starts with "DEBUG" then just output the string
                if (this.m_StringName.Substring(0, 5) == "DEBUG")
                {
                    s = this.m_StringName;
                }
                else // else look it up in the resources
                {
                    if (this.m_NameAsmFileErrs=="OTFontFileVal")
                    {
                        System.Reflection.Assembly a = System.Reflection.Assembly.GetAssembly(this.GetType());
                        System.Resources.ResourceManager rm = new System.Resources.ResourceManager("OTFontFileVal.ValStrings", a);
                        string sErrorAndMessage = rm.GetString(m_StringName);
                        if (sErrorAndMessage.Length > 6 && sErrorAndMessage[5] == ':' && sErrorAndMessage[6] == ' ')
                        {
                            s = sErrorAndMessage.Substring(7);
                        }
                        else
                        {
                            s = null;
                        }
                    }
                    else
                    {
                        s = this.ValueName;
                    }
                    Debug.Assert(s != null, "Resource string not found", m_StringName);
                }
            }

            return s;
        }

        /// <summary>In the special case that <c>m_NameAsmFileErrs</c> is
        /// <c>OTFontFileVal</c>, which I believe it always is, then
        /// read the string corresponding to <c>m_StringName</c> of the form
        /// <c>W1404: The LineGap value is less than the recommended value</c>
        /// from the hardcoded resource file <c>OTFontFileVal.ValStrings</c>
        /// and return a string consisting of the first 5 characters.
        ///
        /// If the resource is not present return <c>null</c>.
        /// If <c>m_NameAsmFileErrs != "OTFontFileVal</c>, then
        /// just return the value of the <c>ErrorID</c> property of the 
        /// base class.
        /// <p/>
        /// This routine may be redundant since the <c>ErrorID</c> 
        /// property already does a resource lookup on the resource.
        /// </summary>
        public string GetErrorID()
        {


            string s = null;

            if ((object)this.m_StringName != null)
            {
                if (this.m_NameAsmFileErrs=="OTFontFileVal")
                {
                    System.Reflection.Assembly a = System.Reflection.Assembly.GetAssembly(this.GetType());
                    System.Resources.ResourceManager rm = new System.Resources.ResourceManager("OTFontFileVal.ValStrings", a);
                    string sErrorAndMessage = rm.GetString(m_StringName);
                    if (sErrorAndMessage.Length > 6 && sErrorAndMessage[5] == ':' && sErrorAndMessage[6] == ' ')
                    {
                        s = sErrorAndMessage.Substring(0,5);
                    }
                    else
                    {
                        s = null;
                    }
                }
                else
                {
                    s = ErrorID;
                }
            }

            return s;
        }

        public string GetDetails() {return this.m_StringValueUser;}
        public void SetDetails(string s) {this.m_StringValueUser = s;}

        string GetStringName() {return this.m_StringName;}
        void SetStringName(string StringName) {this.m_StringName = StringName;}
    }
}
