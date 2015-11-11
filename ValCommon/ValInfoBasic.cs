using System;
using System.Diagnostics;

using OTFontFile;

using Assembly=System.Reflection.Assembly;
using ResourceManager=System.Resources.ResourceManager;


namespace NS_ValCommon
{
    public class ValInfoBasic
    {
        // enums
        public enum ValInfoType
        {
            AppError,
            Error,
            Warning,
            Pass,
            Info
        }
        

        // members
        protected ValInfoType m_Type;
        protected string m_NameFileErrs;             // name of the resourse file to get a "human" optput
        protected string m_NameAsmFileErrs;             // display name of assembly where the resource file is defined 
        protected string m_StringName;               // "index" of the validation info 
        protected string m_StringValueUser;             // user-defined string of information
        protected string m_StringValueName;
        protected OTTag m_OTTagPrincipal;
        protected string m_StringTestName;           // name of the test that generated this validation info.  may be null
        protected string m_StringErrorID;
        //protected OTTag m_OTTagRelated;

        // constructors
        public ValInfoBasic()
        {
            this.m_Type=ValInfoType.Info;
            this.m_StringName=null;
            this.m_StringValueName=null;
            this.m_StringValueUser=null;
            this.m_NameFileErrs=null;
            this.m_NameAsmFileErrs=null;
            this.m_OTTagPrincipal=null;
            this.m_StringTestName=null;
            //this.m_OTTagRelated=null;
        }
        public ValInfoBasic(ValInfoBasic viBasic) 
        {
            Debug.Assert(viBasic!=null);
            this.m_Type=viBasic.TypeBasic;
            this.m_StringName=viBasic.Name;
            this.m_StringValueName=null;
            this.m_StringValueUser=viBasic.ValueUser;
            this.m_NameFileErrs=viBasic.NameFileErrs;
            this.m_NameAsmFileErrs=viBasic.NameAsmFileErrs;
            this.m_OTTagPrincipal=viBasic.TagPrincipal;
            this.m_StringTestName=viBasic.m_StringTestName;
            //this.m_OTTagRelated=viBasic.TagRelated;
        }
        public ValInfoBasic(ValInfoType type, 
            string stringName,
            string stringValueUser,
            string nameFileErrs,
            string nameAsmFileErrs,
            OTTag tagPrincipal,
            string stringTestName)
        {
            this.m_Type=type;
            this.m_StringName=stringName;
            this.m_StringValueName=null;
            this.m_StringValueUser=stringValueUser;
            this.m_NameFileErrs=nameFileErrs;
            this.m_NameAsmFileErrs=nameAsmFileErrs;
            this.m_OTTagPrincipal=tagPrincipal;
            this.m_StringTestName=stringTestName;
            //this.m_OTTagRelated=tagRelated;
        }

        // properties
        public ValInfoType TypeBasic
        {
            get { return this.m_Type; }
            set { this.m_Type=value; }
        }
        public string Name
        {
            get { return this.m_StringName; }
            set { this.m_StringName=value; }
        }
        virtual public string ValueUser
        {
            get { return this.m_StringValueUser; }
            set { this.m_StringValueUser=value; }
        }
        public string NameFileErrs
        {
            get { return this.m_NameFileErrs; }
            set { this.m_NameFileErrs=value; }
        }
        public string NameAsmFileErrs
        {
            get { return this.m_NameAsmFileErrs; }
            set { this.m_NameAsmFileErrs=value; }
        }
        public OTTag TagPrincipal
        {
            get { return this.m_OTTagPrincipal; }
            set { this.m_OTTagPrincipal=value; }
        }
        public string TestName
        {
            get { return this.m_StringTestName; }
            set { this.m_StringTestName=value; }
        }
        /*
        public OTTag TagRelated
        {
            get { return this.m_OTTagRelated; }
            set { this.m_OTTagRelated=value; }
        }
        */
        virtual public string ValueName
        {
            get 
            {
                if (this.m_StringValueName!=null)
                {
                    return this.m_StringValueName;
                }
                if (((object)this.Name==null)||
                    ((object)this.NameAsmFileErrs==null)||
                    ((object)this.NameFileErrs==null))
                {
                    return null;
                }
                
                /* 
                // WORKS !!!
                // NOTE: uses directly absolute name of the resourse file
                //         (does not make use of the assembly)
                System.Resources.ResourceReader rr=new System.Resources.ResourceReader("C:\\VAL_SHARP_NEW\\Glyph\\obj\\Debug\\NS_Glyph.GErrStrings.resources");
                System.Collections.IDictionaryEnumerator en=rr.GetEnumerator();
                while (en.MoveNext())
                {
                    object key=en.Key;
                    object val=en.Value;
                }
                */

                /*
                // WORKS if the resource belongs to the SAME ASSEMBLY that the error class
                Assembly asm=Assembly.GetAssembly(this.GetType());
                System.Resources.ResourceManager rm = new ResourceManager("NS_Glyph.GErrStrings", asm);
                string nameValue = rm.GetString(this.Name);
                */
                try 
                {
                    /* DO NOT REMOVE - interesting ...
                    Assembly asm;
                    string[] strNameRes;
                    Type type=Type.GetType("OTFontFile.ValidationInfo");
                    
                    asm=Assembly.GetCallingAssembly();   // Glyph.dll
                    strNameRes=asm.GetManifestResourceNames();
                    asm=Assembly.GetExecutingAssembly(); // ValidatorBasic.dll 

                    asm=Assembly.GetEntryAssembly();
                    strNameRes=asm.GetManifestResourceNames();
                    System.Reflection.AssemblyName[] strNameAsmRef=asm.GetReferencedAssemblies();
                    */            
        
                    // WORKS VERSION_1- begin
                    /*
                    Assembly asm=Assembly.Load("OTFontFile");
                    ResourceManager rm = new ResourceManager("OTFontFile.ValStrings", asm);
                    */
                    // WORKS VERSION_1- end
                
                    // WORKS VERSION_2 - begin
                    /*
                    ResourceManager rm = ResourceManager.CreateFileBasedResourceManager("OTFontFile.ValStrings","C:\\VAL_SHARP_NEW\\OTFontFile\\obj\\Debug",null);
                    */
                    // WORKS VERSION_2 - end
                
                    ResourceManager rm=new ResourceManager(this.NameFileErrs,Assembly.Load(this.NameAsmFileErrs));        
                    string sErrorAndMessage = rm.GetString(m_StringName);
                    if (sErrorAndMessage.Length > 6 && sErrorAndMessage[5] == ':' && sErrorAndMessage[6] == ' ')
                    {
                        this.m_StringValueName = sErrorAndMessage.Substring(7);
                    }
                    else
                    {
                        this.m_StringValueName = rm.GetString(this.Name);
                    }
                }
                catch
                {
                    this.m_StringValueName = null;
                }

                return this.m_StringValueName;
            

                //Assembly asm=Assembly.GetAssembly(this.GetType());        
                //ResourceManager rm=new ResourceManager(this.FileErrs,asm);                
                //return (rm.GetString(this.Name));
            }
        }

        public string ErrorID
        {

            get
            {
                if (m_StringErrorID == null)
                {
                    if ((object)this.m_StringName != null)
                    {
                        try
                        {
                            ResourceManager rm=new ResourceManager(this.NameFileErrs,Assembly.Load(this.NameAsmFileErrs));        
                            string sErrorAndMessage = rm.GetString(m_StringName);
                            if (sErrorAndMessage.Length > 6 && sErrorAndMessage[5] == ':' && sErrorAndMessage[6] == ' ')
                            {
                                m_StringErrorID = sErrorAndMessage.Substring(0,5);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Assert(false, e.Message);
                        }
                    }
                }

                return m_StringErrorID;
            }
        }


        // methods
        virtual public bool IsSame(object obj)                        
        {        
            ValInfoBasic info=obj as ValInfoBasic;
            if (info==null)
                return false;
            if (this.m_Type!=info.TypeBasic)
                return false;
            if (this.m_OTTagPrincipal!=info.TagPrincipal)
                return false;
            /*
            if (this.m_OTTagRelated!=info.TagRelated)
                return false;
                */
            if (String.Compare(this.m_StringName,info.Name)!=0)
                return false;
            if (String.Compare(this.m_NameFileErrs,info.NameFileErrs)!=0)
                return false;
            if (String.Compare(this.m_NameAsmFileErrs,info.NameAsmFileErrs)!=0)
                return false;
            return true;
        }
    }
}
