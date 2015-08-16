using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using NS_ValCommon;
using OTFontFile;

namespace OTFontFileVal {

    /// <summary>
    /// Collect the parameters to the validator in a unified way.
    /// </summary>
    public class ValidatorParameters
    {
        private string [] m_allTables = TableManager.GetKnownOTTableTypes();
        public List<string> tablesToTest = new List<string>();
        public bool doRastBW = false;
        public bool doRastGray = false;
        public bool doRastClearType = false;
        public bool doRastCTCompWidth = false;
        public bool doRastCTVert = false;
        public bool doRastCTBGR = false;
        public bool doRastCTFractWidth = false;
        public int  xRes = 96;
        public int  yRes = 96;
        public RastTestTransform xform = new RastTestTransform(); // Later
        public List<int> sizes = new List<int>();
        
        public ValidatorParameters()
        {
            SetAllTables();
            SetDefaultSizes();
        }

        public void SetNoRasterTesting()
        {
            doRastBW = false;
            doRastGray = false;
            doRastClearType = false;
            doRastCTCompWidth = false;
            doRastCTVert = false;
            doRastCTBGR = false;
            doRastCTFractWidth = false;
        }

        public void SetRasterTesting()
        {
            doRastBW = true;
            doRastGray = true;
            doRastClearType = true;
            doRastCTCompWidth = true;
            doRastCTVert = true;
            doRastCTBGR = true;
            doRastCTFractWidth = true;
        }

        private void SetDefaultSizes()
        {
            for ( int i = 4; i <= 72; i++ ) {
                sizes.Add( i );
            }
            sizes.Add( 80 );
            sizes.Add( 88 );
            sizes.Add( 96 );
            sizes.Add( 102 );
            sizes.Add( 110 );
            sizes.Add( 118 );
            sizes.Add( 126 );
        }
            
        public void AddTable( string table )
        {
            if ( !tablesToTest.Contains( table ) ) {
                tablesToTest.Add( table );
            }
        }

        public bool IsTestingTable( string table )
        {
            return tablesToTest.Contains( table );
        }

        public int RemoveTableFromList( string table )
        {
            int i;
            for ( i = 0; tablesToTest.Remove( table ); i++ );
            return i;
        }

        public void SetAllTables()
        {
            tablesToTest.Clear();
            for ( int k = 0; k < m_allTables.Length; k++ ) {
                tablesToTest.Add( m_allTables[k] );
            }
        }

        public void ClearTables()
        {
            tablesToTest.Clear();
        }

        public void SetupValidator( Validator v )
        {
            for ( int k = 0; k < m_allTables.Length; k++ ) {
                string table = m_allTables[k];
                bool perform = tablesToTest.Contains( table );
                v.SetTablePerformTest( table, perform );
            }
            v.SetRastPerformTest( doRastBW, 
                                  doRastGray, 
                                  doRastClearType,
                                  doRastCTCompWidth,
                                  doRastCTVert, 
                                  doRastCTBGR, 
                                  doRastCTFractWidth );
            v.SetRastTestParams( xRes, yRes, sizes.ToArray(), xform );
            
        }
    }
}