using System;
using System.Collections;
using System.Diagnostics;

using OTFontFile;
using NS_ValCommon;

namespace OTFontFileVal
{
    public class Validator : DIActionContainer, I_TestReader
    {
        /*
         *        CONSTRUCTOR
         */

        public Validator()
            : base(null)
        {
            m_hashTestsToPerform = new Hashtable();
            ReadTestNames();
            m_bCancelFlag = false;

            // default rasterization values
            m_rast_XResolution = 72;
            m_rast_YResolution = 72;
            m_rast_pointsizes = new int[68]; for (int i=0; i<68; i++) m_rast_pointsizes[i] = i+4;
            m_rast_RastTestTransform = new RastTestTransform();;
        }

        /*
         *        Test Reader Functions
         */

        private void ReadTestNames()
        {
            /*
            System.Reflection.Assembly a = System.Reflection.Assembly.GetAssembly(this.GetType());
            System.IO.Stream stream = a.GetManifestResourceStream("OTFontFile.ValTests.resources");
            System.Resources.IResourceReader reader = new System.Resources.ResourceReader(stream);
            System.Collections.IDictionaryEnumerator en = reader.GetEnumerator();
      
            while (en.MoveNext()) 
            {
                m_hashTestsToPerform.Add(en.Key, true);
            }

            reader.Close();
            stream.Close();
            */

            // previously, each individual test name was read from the ValTests.resx file
            // now, only the table name is used

            string [] sTableTypes = TableManager.GetKnownOTTableTypes();
            for (int i=0; i<sTableTypes.Length; i++)
            {
                m_hashTestsToPerform.Add(sTableTypes[i], true);
            }
        }


        // Careful, but the name is never used.
        // This method exists to fulfill the NS_ValCommon.I_TestReader interface
        // We never call it, though.
        public bool PerformTest( string sTestName ) {
            return PerformTest( T.T_NULL ); 
        }

        public bool PerformTest( T eTestName )
        {
            bool bPerform = true;

            if (CancelFlag)
            {
                bPerform = false;
            }

            return bPerform;
        }

        /// <summary>Return <c>true</c> iff table has not been deselected
        /// and user has not cancelled.
        /// </summary>
        public bool TestTable(OTTag tagTable)
        {
            bool bTest = true;  // if the table hasn't been deselected, then it will default to getting tested

            if (m_hashTestsToPerform.ContainsKey((string)tagTable))
            {
                bTest = (bool)m_hashTestsToPerform[(string)tagTable];
            }

            if (CancelFlag)
            {
                bTest = false;
            }

            return bTest;
        }

//        This routine does not appear to be called.
//        public SortedList GetTests()
//        {
//            SortedList sl = new SortedList();
//
//            System.Reflection.Assembly a = System.Reflection.Assembly.GetAssembly(GetType());
//            System.IO.Stream stream = a.GetManifestResourceStream("OTFontFile.ValTests.resources");
//            System.Resources.IResourceReader reader = new System.Resources.ResourceReader(stream);
//            System.Collections.IDictionaryEnumerator en = reader.GetEnumerator();
//
//            while (en.MoveNext()) 
//            {
//                sl.Add(en.Key, en.Value);
//            }
//
//            reader.Close();
//            stream.Close();
//
//            return sl;
//        }
//
//        This routine does not appear to be called.
//        public string GetTestDescription(string sTest)
//        {
//            System.Reflection.Assembly a = System.Reflection.Assembly.GetAssembly(this.GetType());
//            System.Resources.ResourceManager rm = new System.Resources.ResourceManager("OTFontFileVal.ValTests", a);
//            return rm.GetString(sTest);
//        }



        /// <summary>Action for checking or unchecking "perform table test"
        /// in UI.
        /// </summary>
        public void SetTablePerformTest(string sTable, bool bPerform)
        {
            if (m_hashTestsToPerform.ContainsKey(sTable))
            {
                m_hashTestsToPerform[sTable] = bPerform;
            }
            else
            {
                Debug.Assert(false, "table name not found", sTable);
            }
        }

        /// <summary>Set boolean parameters to be used in deciding which
        /// rasterization tests to perform.
        /// </summary>
        public void SetRastPerformTest( bool bPerform_BW, 
                                        bool bPerform_Grayscale, 
                                        bool bPerform_Cleartype,
                                        bool bCTFlag_CompWidth, 
                                        bool bCTFlag_Vert, 
                                        bool bCTFlag_BGR, 
                                        bool bCTFlag_FractWidth )
        {
            m_bPerformRastTest_BW = bPerform_BW;
            m_bPerformRastTest_Grayscale = bPerform_Grayscale;
            m_bPerformRastTest_Cleartype = bPerform_Cleartype;
            
            m_bCTFlag_CompWidth = bCTFlag_CompWidth;
            m_bCTFlag_Vert = bCTFlag_Vert;
            m_bCTFlag_BGR  = bCTFlag_BGR;
            m_bCTFlag_FractWidth = bCTFlag_FractWidth;
        }

        /// <summary>Return true is we are to perform BW tests, and user has
        /// not cancelled.
        /// </summary>
        public bool PeformRastTest_BW()
        {
            bool bPerform = m_bPerformRastTest_BW;

            if (CancelFlag)
            {
                bPerform = false;
            }

            return bPerform;
        }

        /// <summary>Return true is we are to perform grayscale tests, 
        /// and user has not cancelled.
        /// </summary>
        public bool PeformRastTest_Grayscale()
        {
            bool bPerform = m_bPerformRastTest_Grayscale;

            if (CancelFlag)
            {
                bPerform = false;
            }

            return bPerform;
        }

        /// <summary>Return true is we are to perform ClearType tests, 
        /// and user has not cancelled.
        /// </summary>
        public bool PeformRastTest_Cleartype()
        {
            bool bPerform = m_bPerformRastTest_Cleartype;

            if (CancelFlag)
            {
                bPerform = false;
            }

            return bPerform;
        }

        /// <summary>Return ClearType testing flags set via
        /// <c>SetRastPerformTest</c>
        /// </summary>
        public uint GetCleartypeFlags()
        {
            // flags values are defined in the rasterizer, see fscaler.h

            uint flags = 0;
            if (m_bCTFlag_CompWidth)
                flags |= 0x0002;
            if (m_bCTFlag_Vert)
                flags |= 0x0004;
            if (m_bCTFlag_BGR)
                flags |= 0x0008;
            if (m_bCTFlag_FractWidth)
                flags |= 0x0010;

            return flags;
        }

        /// <summary>Reset the set of tests to perform.</summary>
        public void Clear()
        {
            this.m_hashTestsToPerform.Clear();
        }

        /*
         *        Info Action Functions
         */

    
        /*
        // Declare a delegate for a method that receives a ValidationInfo reference and returns void.
        public delegate void ValInfoDelegate(ValidationInfo vi);
        */
        
        /// <summary><c>Progress.Worker</c> calls this and 
        /// sets the delegate to be <c>Progress.ValidatorCallback</c>
        /// </summary>
        /// 
        public void SetValInfoDelegate(DIAction vid)
        {
            base.dia = vid;
        }


        /// <summary>event types enumeration</summary>
        public enum EventTypes
        {
            FileBegin,
            FileEnd,
            FontBegin,
            FontParsed,
            FontEnd,
            TableBegin,
            TableProgress,
            TableEnd,
            RastTestBegin_BW,
            RastTestProgress_BW,
            RastTestEnd_BW,
            RastTestBegin_Grayscale,
            RastTestProgress_Grayscale,
            RastTestEnd_Grayscale,
            RastTestBegin_ClearType,
            RastTestProgress_ClearType,
            RastTestEnd_ClearType,
        }

        /// <summary>delegate for a method that will receive events</summary>
        public delegate void OnValidateEvent(EventTypes e, object oParam);


        /// <summary><c>Progress.Worker</c> calls this and 
        /// sets the delegate to be <c>Progress.OnValidateEvent</c>
        /// </summary>
        /// 
        public void SetOnValidateEvent(OnValidateEvent ove)
        {
            m_OnValidateEvent = ove;
        }

        /// <summary>Notify observer of FileBegin and FileEnd events</summary>
        public void OnFileValidationEvent(OTFile f, bool bBegin)
        {
            if (m_OnValidateEvent != null)
            {
                if (bBegin)
                {
                    m_OnValidateEvent(EventTypes.FileBegin, f);
                }
                else
                {
                    m_OnValidateEvent(EventTypes.FileEnd, f);
                }
            }
        }

        /// <summary>Notify observer of FontBegin and FontEnd events</summary>
        public void OnFontValidationEvent(uint fontIndex, bool bBegin)
        {
            if (m_OnValidateEvent != null)
            {
                if (bBegin)
                {
                    m_OnValidateEvent(EventTypes.FontBegin, fontIndex);
                }
                else
                {
                    m_OnValidateEvent(EventTypes.FontEnd, fontIndex);
                }
            }
        }

        /// <summary>Notify observer of FontParsed event</summary>
        public void OnFontParsedEvent(OTFont f)
        {
            m_OnValidateEvent(EventTypes.FontParsed, f);
        }

        /// <summary>Notify observer of TableBegin and TableEnd events</summary>
        public void OnTableValidationEvent(DirectoryEntry de, bool bBegin)
        {
            if (m_OnValidateEvent != null)
            {
                if (bBegin)
                {
                    m_OnValidateEvent(EventTypes.TableBegin, de);
                }
                else
                {
                    m_OnValidateEvent(EventTypes.TableEnd, de);
                }
            }
        }

        /// <summary>Notify observer of TableProgress event</summary>
        public void OnTableProgress(string s)
        {
            if (m_OnValidateEvent != null)
            {
                m_OnValidateEvent(EventTypes.TableProgress, s);
            }
        }

        /// <summary>Notify observer of RastTestBegin(End)_BW events</summary>
        public void OnRastTestValidationEvent_BW(bool bBegin)
        {
            if (m_OnValidateEvent != null)
            {
                if (bBegin)
                {
                    m_OnValidateEvent(EventTypes.RastTestBegin_BW, null);
                }
                else
                {
                    m_OnValidateEvent(EventTypes.RastTestEnd_BW, null);
                }
            }
        }

        /// <summary>Notify observer of RastTestProgress_BW events</summary>
        public void OnRastTestProgress_BW(string s)
        {
            if (m_OnValidateEvent != null)
            {
                m_OnValidateEvent(EventTypes.RastTestProgress_BW, s);
            }
        }

        /// <summary>Notify observer of RastTestBegin_GrayScale
        /// and RastTestEnd_GrayScale events</summary>
        public void OnRastTestValidationEvent_Grayscale(bool bBegin)
        {
            if (m_OnValidateEvent != null)
            {
                if (bBegin)
                {
                    m_OnValidateEvent(EventTypes.RastTestBegin_Grayscale, null);
                }
                else
                {
                    m_OnValidateEvent(EventTypes.RastTestEnd_Grayscale, null);
                }
            }
        }

        /// <summary>Notify observer of RastTestProgress_GrayScale
        /// event</summary>
        public void OnRastTestProgress_Grayscale(string s)
        {
            if (m_OnValidateEvent != null)
            {
                m_OnValidateEvent(EventTypes.RastTestProgress_Grayscale, s);
            }
        }

        /// <summary>Notify observer of RastTestBegin_Cleartype
        /// and RastTestEnd_Cleartype events</summary>
        public void OnRastTestValidationEvent_Cleartype(bool bBegin)
        {
            if (m_OnValidateEvent != null)
            {
                if (bBegin)
                {
                    m_OnValidateEvent(EventTypes.RastTestBegin_ClearType, null);
                }
                else
                {
                    m_OnValidateEvent(EventTypes.RastTestEnd_ClearType, null);
                }
            }
        }

        /// <summary>Notify observer of RastTestProgress_Cleartype
        /// event</summary>
        public void OnRastTestProgress_Cleartype(string s)
        {
            if (m_OnValidateEvent != null)
            {
                m_OnValidateEvent(EventTypes.RastTestProgress_ClearType, s);
            }
        }

        /// <summary>Called when there is a caught exception.</summary>
        private void ApplicationError(string sTestName, string stringName, OTTag tag, string stringDetails)
        {
            if (this.dia != null)
            {
                ValidationInfo vi = new ValidationInfo(
                    ValidationInfo.ValInfoType.AppError, 
                    stringName, 
                    tag, 
                    stringDetails,
                    sTestName);
                SendInfo(vi);
            }
        }
        
        public void ApplicationError( T eTestName, E eStringName, 
                                      OTTag tag, string stringDetails )
        {
            ApplicationError( ToTestName(eTestName),
                              eStringName.ToString(),
                              tag, stringDetails );
        }
        
        private string ToTestName( T enumValue )
        {
            if ( T.T_NULL == enumValue ) {
                return null;
            } else {
                return enumValue.ToString();
            }
        }

        /// <summary>The main method for adding an Error message.</summary>
        private void Error(string sTestName, string stringName, OTTag tag, string stringDetails)
        {
            if (this.dia != null)
            {
                ValidationInfo vi = new ValidationInfo(
                    ValidationInfo.ValInfoType.Error, 
                    stringName, 
                    tag, 
                    stringDetails,
                    sTestName);
                SendInfo(vi);
            }
        }

        /// <summary>Error with no <c>sDetails</c></summary>
        private void Error(string sTestName, string stringName, OTTag tag)
        {
            Error(sTestName, stringName, tag, null);
        }

        /// <summary>Error with no <c>sTestName</c> and no <c>sDetails</c>
        /// </summary>
        private void Error(string StringName, OTTag tag)
        {
            Error(null, StringName, tag, null);
        }

        public void Error( T eTestName, E eStringName, OTTag tag, string stringDetails) 
        {
            Error( ToTestName(eTestName), 
                   eStringName.ToString(),
                   tag, 
                   stringDetails );
        }
        public void Error( T eTestName, E eStringName, OTTag tag) {
            Error( ToTestName(eTestName), eStringName.ToString(), tag );
        }
        public void Error( E eStringName, OTTag tag) {
            Error( eStringName.ToString(), tag );
        }

        /// <summary>The main method for adding a Warning message.</summary>
        private void Warning(string sTestName, string stringName, OTTag tag, string stringDetails)
        {
            if (this.dia != null)
            {
                ValidationInfo vi = new ValidationInfo(
                    ValidationInfo.ValInfoType.Warning, 
                    stringName, 
                    tag, 
                    stringDetails,
                    sTestName);
                SendInfo(vi);
            }
        }

        /// <summary>Warning with no <c>sDetails</c></summary>
        private void Warning(string sTestName, string stringName, OTTag tag)
        {
            Warning(sTestName, stringName, tag, null);
        }

        /// <summary>Warning with no <c>sTestName</c> and no <c>sDetails</c> 
        /// </summary>
        private void Warning(string StringName, OTTag tag)
        {
            Warning(null, StringName, tag, null);
        }

        public void Warning( T      eTestName, 
                             W      eStringName, 
                             OTTag tag, 
                             string stringDetails) {
            Warning( ToTestName(eTestName), 
                     eStringName.ToString(), 
                     tag, stringDetails );
        }
        public void Warning( T eTestName, W eStringName, OTTag tag) {
            Warning( ToTestName(eTestName), eStringName.ToString(), tag );
        }
        public void Warning( W eStringName, OTTag tag) {
            Warning( eStringName.ToString(), tag );
        }

        /// <summary>The main method for adding an Pass message.</summary>
        private void Pass(string sTestName, string stringName, OTTag tag, string stringDetails)
        {
            if (this.dia != null)
            {
                ValidationInfo vi = new ValidationInfo(
                    ValidationInfo.ValInfoType.Pass, stringName, tag, stringDetails, sTestName);
                SendInfo(vi);
            }
        }
        
        /// <summary>Pass with no <c>sDetails</c> string</summary>
        private void Pass(string sTestName, string stringName, OTTag tag)
        {
            Pass(sTestName, stringName, tag, null);
        }

        /// <summary>Pass with no <c>sTestName</c> and no 
        /// <c>sDetails</c> string</summary>
        private void Pass(string StringName, OTTag tag)
        {
            Pass(null, StringName, tag, null);
        }

        public void Pass( T eTestName, 
                          P eStringName, 
                          OTTag tag, 
                          string stringDetails ) 
        {
            Pass( ToTestName(eTestName), 
                  eStringName.ToString(), 
                  tag, 
                  stringDetails );
        }
        public void Pass( T eTestName, P eStringName, OTTag tag)
        {
            Pass( ToTestName(eTestName), eStringName.ToString(), tag );
        }
        public void Pass( P eStringName, OTTag tag) 
        {
            Pass( eStringName.ToString(), tag );
        }
        

        /// <summary>The main method for adding an Info message.</summary>
        private void Info(string sTestName, string stringName, OTTag tag, string sDetails)
        {
            if (this.dia != null)
            {
                ValidationInfo vi = new ValidationInfo(
                    ValidationInfo.ValInfoType.Info, stringName, tag, sDetails, sTestName);
                SendInfo(vi);
            }
        }

        /// <summary>Info with no <c>sDetails</c> string</summary>
        private void Info(string sTestName, string stringName, OTTag tag)
        {
            Info(sTestName, stringName, tag, null);
        }

        /// <summary>Info with no <c>sTestName</c> and no <c>sDetails</c> 
        /// string
        /// </summary>
        private void Info(string StringName, OTTag tag)
        {
            Info(null, StringName, tag, null);
        }

        public void Info( T eTestName, 
                          I eStringName, 
                          OTTag tag, 
                          string sDetails) {
            Info( ToTestName(eTestName), eStringName.ToString(), tag, sDetails);
        }
        public void Info( T eTestName, I eStringName, OTTag tag) {
            Info( ToTestName(eTestName), eStringName.ToString(), tag );
        }
        public void Info( I eStringName, OTTag tag) {
            Info( eStringName.ToString(), tag );
        }

        /// <summary>Sneaky way of getting a string in the output as an
        /// info message.
        /// </summary>
        public void DebugMsg(string s, OTTag tag)
        {
            Info("DEBUG: "+s, tag);
        }


        /// <summary>Synchronized access to m_bCancelFlag</summary>
        public bool CancelFlag
        {
            get
            {
                lock(this)
                {
                    return m_bCancelFlag;
                }
            }
            set
            {
                lock(this)
                {
                    m_bCancelFlag = value;
                }
            }
        }

        /// <summary>Set rasterization test parameters</summary>
        public void SetRastTestParams(int resX, int resY, int [] pointsizes, RastTestTransform rtt)
        {
            m_rast_XResolution = resX;
            m_rast_YResolution = resY;
            m_rast_pointsizes = pointsizes;
            m_rast_RastTestTransform = rtt;
        }

        /// <summary>Accessor for <c>m_rast_XResolution</c></summary>
        public int GetRastTestXRes()
        {
            return m_rast_XResolution;
        }

        /// <summary>Accessor for <c>m_rast_YResolution</c></summary>
        public int GetRastTestYRes()
        {
            return m_rast_YResolution;
        }

        /// <summary>Accessor for <c>m_rast_pointsizes</c></summary>
        public int [] GetRastTestPointSizes()
        {
            return m_rast_pointsizes;
        }

        /// <summary>Accessor for <c>m_rast_RastTestTransform</c></summary>
        public RastTestTransform GetRastTestTransform()
        {
            return m_rast_RastTestTransform;
        }

        public void OnRastTestError(String sStringName, String sDetails)
        {
            if (sStringName[6] == 'W')
            {
                Warning(null, sStringName, null, sDetails);
            }
            else
            {
                Error(null, sStringName, null, sDetails);
            }
        }

        /*
         *        MEMBERS
         */

        Hashtable m_hashTestsToPerform;
        bool m_bCancelFlag;
        OnValidateEvent m_OnValidateEvent;

        // rasterization test members

        bool m_bPerformRastTest_BW;
        bool m_bPerformRastTest_Grayscale;
        bool m_bPerformRastTest_Cleartype;
        
        bool m_bCTFlag_CompWidth;
        bool m_bCTFlag_Vert;
        bool m_bCTFlag_BGR;
        bool m_bCTFlag_FractWidth;

        int m_rast_XResolution;
        int m_rast_YResolution;
        int [] m_rast_pointsizes;
        RastTestTransform m_rast_RastTestTransform;

    }
}

