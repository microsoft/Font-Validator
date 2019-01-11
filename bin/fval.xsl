<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

	<xsl:template match="/">
		<HTML>
		<HEAD>
		<script language="JavaScript">
		function cbInfo_onclick()
		{
			display_row(document.FORM1.cbInfo, "class_I");
		}
		function cbPass_onclick()
		{
			display_row(document.FORM1.cbPass, "class_P");
		}
		function cbWarn_onclick()
		{
			display_row(document.FORM1.cbWarn, "class_W");
		}
		function cbErr_onclick()
		{
			display_row(document.FORM1.cbErr, "class_E");
		}
		function display_table(table_id)
		{
			table_id.style.display = (table_id.style.display == "none" ) ? "" : "none";
		}
		function display_row(cb_id, row_classname)
		{
			rowColl = document.all.tags("TR");
			for (i=0; i!=rowColl.length; i++) {
				if (rowColl(i).className == row_classname)
				{
					if (cb_id.checked)
					{
						rowColl(i).style.display = "";
					}
					else
					{
						rowColl(i).style.display = "none";
					}
				}
			}
		}
		</script>
		<STYLE>
			body 	{font-family: Trebuchet MS;
				font-size: 10pt;
				}
			TABLE 	{font-family: Trebuchet MS;
				font-size: 10pt;
				}
			H2	{font-family: Trebuchet MS;
				font-weight:bold;
				font-size: 14pt;
				color: #000000;
				}
			TR	{background-color: #f0f0f0;
				}
			TR.caption {background-color: #cccccc}
			TR.class_A {background-color: #000000; color: #ffff00}
			TR.class_E {background-color: #ff9999}
			TR.class_W {background-color: #ffff99}
			TR.class_P {background-color: #99ff99}
			TR.class_I {background-color: #ccffff}
		</STYLE>
		</HEAD>
		<BODY>
		
		<FORM NAME="FORM1">
		<H2>
			Font Validator Report -
			<xsl:value-of select="FontValidatorReport/FontFile/@FileName"/>
		</H2>

		<TABLE WIDTH="450"><TR>
		<TD BGCOLOR="#ffffff" WIDTH="50">
		Show 
		</TD>

		<TD BGCOLOR="#ccffff" WIDTH="100">
		  <INPUT id='cbInfo' type='checkbox' name='cbInfo'
			 onclick='cbInfo_onclick()' checked="1" />
		Info
		</TD>
		
		<TD BGCOLOR="#99ff99" WIDTH="100">
		  <INPUT id='cbPass' type='checkbox' name='cbPass'
			 onclick='cbPass_onclick()' checked="1" />
		Pass
		</TD>
		
		<TD BGCOLOR="#ffff99" WIDTH="100">
		  <INPUT id='cbWarn' type='checkbox' name='cbWarn'
			 onclick='cbWarn_onclick()' checked="1"/>
		Warning
		</TD>
		
		<TD BGCOLOR="#ff9999" WIDTH="100">
		<INPUT id='cbErr' type='checkbox' name='cbErr'
		       onclick='cbErr_onclick()' checked="1"/>
		Error 
		</TD>

		</TR></TABLE>
		
		</FORM>


		<TABLE WIDTH="100%">
		<TR>
		<TD WIDTH="15%"><I>Run DateTime: </I></TD>
		<TD><xsl:value-of select="FontValidatorReport/@RunDateTime"/></TD>
		</TR>
		<TR>
		<TD WIDTH="15%"><I>Machine Name: </I></TD>
		<TD><xsl:value-of select="FontValidatorReport/@MachineName"/></TD>
		</TR>
		<TR>
		<TD WIDTH="15%"><I>Font file: </I></TD>
		<TD><xsl:value-of select="FontValidatorReport/FontFile/@FileNameAndPath"/></TD>
		</TR>
		</TABLE>
		
		<TABLE WIDTH="100%">
		<TR CLASS="caption">
		<TD WIDTH="10%"><I>Error code</I></TD>
		<TD WIDTH="50%"><I>Message</I></TD>
		<TD WIDTH="40%"><I>Details</I></TD>
		</TR>
		<xsl:apply-templates select="FontValidatorReport/FontFile/Report"/><BR/>
		</TABLE>
		
		<xsl:for-each select="FontValidatorReport/FontFile/Font">
		
			<HR></HR><BR/>
			
			<P><B><I>Index: </I><xsl:value-of select="@FontIndex"/>, <xsl:value-of select="FontInfo/@FontName"/>, <xsl:value-of select="FontInfo/@FontVersion"/>, <xsl:value-of select="FontInfo/@FontDate"/></B></P>
			<BR/>

			<TABLE WIDTH="100%">
			<TR CLASS="caption">
			<TD WIDTH="25%"><I>Table Tag</I></TD>
			<TD WIDTH="25%"><I>Table Offset</I></TD>
			<TD WIDTH="25%"><I>Table Length</I></TD>
			<TD WIDTH="25%"><I>Table Checksum</I></TD>
			</TR>
			<xsl:for-each select="TableEntry">
				<TR>
				<TD>
				
				<A HREF='#{@CodeFriendlyTag}_{parent::node()/@FontIndex}'>
				  <xsl:value-of select="@Tag"/>
				</A>
				</TD>
				<TD><xsl:value-of select="@Offset"/></TD>
				<TD><xsl:value-of select="@Length"/></TD>
				<TD><xsl:value-of select="@Checksum"/></TD>
				</TR>
			</xsl:for-each>
			</TABLE>
			<BR/>
		
			<B>General Font Information</B>
			<TABLE WIDTH="100%">
			<TR CLASS="caption">
			<TD WIDTH="10%"><I>Error code</I></TD>
			<TD WIDTH="50%"><I>Message</I></TD>
			<TD WIDTH="40%"><I>Details</I></TD>
			</TR>
			<xsl:apply-templates select="Report"/><BR/>
			</TABLE><BR/>
			
			<xsl:for-each select="TableEntry">
				<BR/>
				<INPUT
				    id='cb_{@CodeFriendlyTag}_{parent::node()/@FontIndex}'
				    type='checkbox'
				    name='cb_{@CodeFriendlyTag}_{parent::node()/@FontIndex}'
				    onclick='display_table(table_{@CodeFriendlyTag}_{parent::node()/@FontIndex})'
				checked='1'/>
				<B>
				<A
				NAME='{@CodeFriendlyTag}_{parent::node()/@FontIndex}'>
				  <xsl:value-of select="@Tag"/>
				</A>
				</B><BR/>
				<TABLE
				WIDTH='100%' id='table_{@CodeFriendlyTag}_{parent::node()/@FontIndex}'>
				  <TR CLASS="caption">
				    <TD WIDTH="10%"><I>Error code</I></TD>
				    <TD WIDTH="50%"><I>Message</I></TD>
				    <TD WIDTH="40%"><I>Details</I></TD>
				  </TR>
				  <xsl:apply-templates select="Report"/><BR/>
				</TABLE>
				<BR/>
			</xsl:for-each>
			<xsl:for-each select="RasterizationTest_BW">
				<BR/>
				<INPUT
				id='cb_{@CodeFriendlyTag}_{parent::node()/@FontIndex}' type='checkbox' name='cb_{CodeFriendlyTag}_{parent::node()/@FontIndex}' onclick='display_table(table_{@CodeFriendlyTag}_{parent::node()/@FontIndex})'
				checked='1'/>
				<B>Rasterization Test, BW</B><BR/>
				<TABLE
				WIDTH='100%' id='table_{@CodeFriendlyTag}_{parent::node()/@FontIndex}'>
				<TR CLASS="caption">
				<TD WIDTH="10%"><I>Error code</I></TD>
				<TD WIDTH="50%"><I>Message</I></TD>
				<TD WIDTH="40%"><I>Details</I></TD>
				</TR>
				<xsl:apply-templates select="Report"/><BR/>
				</TABLE>
				<BR/>
			</xsl:for-each>
			<xsl:for-each select="RasterizationTest_Grayscale">
				<BR/>
				<INPUT
				id='cb_{@CodeFriendlyTag}_{parent::node()/@FontIndex}' type='checkbox' name='cb_{@CodeFriendlyTag}_{parent::node()/@FontIndex}' onclick='display_table(table_{@CodeFriendlyTag}_{parent::node()/@FontIndex})'
				checked='1'/>
				<B>Rasterization Test, Grayscale</B><BR/>
				<TABLE
				WIDTH='100%' id='table_{@CodeFriendlyTag}_{parent::node()/@FontIndex}'>
				<TR CLASS="caption">
				<TD WIDTH="10%"><I>Error code</I></TD>
				<TD WIDTH="50%"><I>Message</I></TD>
				<TD WIDTH="40%"><I>Details</I></TD>
				</TR>
				<xsl:apply-templates select="Report"/><BR/>
				</TABLE>
				<BR/>
			</xsl:for-each>
			<xsl:for-each select="RasterizationTest_ClearType">
				<BR/>
				<INPUT
				id='cb_{@CodeFriendlyTag}_{parent::node()/@FontIndex}' type='checkbox' name='cb_{@CodeFriendlyTag}_{parent::node()/@FontIndex}' onclick='display_table(table_{@CodeFriendlyTag}_{parent::node()/@FontIndex})'
				checked='1'/>
				<B>Rasterization Test, ClearType</B><BR/>
				<TABLE
				WIDTH='100%' id='table_{@CodeFriendlyTag}_{parent::node()/@FontIndex}'>
				<TR CLASS="caption">
				<TD WIDTH="10%"><I>Error code</I></TD>
				<TD WIDTH="50%"><I>Message</I></TD>
				<TD WIDTH="40%"><I>Details</I></TD>
				</TR>
				<xsl:apply-templates select="Report"/><BR/>
				</TABLE>
				<BR/>
			</xsl:for-each>
			
		</xsl:for-each>

		<BR/>
		<P>
			Note: The Font Validator's helpfile contains detailed information about each error message.
			The latest OpenType specification is available at the
			<A TARGET="TOP" HREF="http://www.microsoft.com/typography">Microsoft Typography website</A>.
		</P>


		</BODY>
		</HTML>
	</xsl:template>
	
	
	<xsl:template match = "Report">
	
		<TR class='class_{@ErrorType}'>
		<TD VALIGN="TOP"> <xsl:value-of select="@ErrorCode"/> </TD>
		<TD VALIGN="TOP"> <xsl:value-of select="@Message"/> </TD>
		<TD VALIGN="TOP"> <xsl:value-of select="@Details"/> </TD>
		</TR>
		
	</xsl:template>
	
	
</xsl:stylesheet>
