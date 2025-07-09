<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">       
	<xsl:template match="ndoc" mode="header-section">
		<style type="text/css"> 
			.tblhead
			{ 
				font-size:8pt;font-weight:bold;background-color:yellow; 
			}
			.tblitem
			{
				font-size:8pt; 
			}
			.tbltitle
			{
				font-size:8pt;font-weight:bold;
			}
		</style> 
	</xsl:template> 
	
	<xsl:template match="column" mode="slashdoc">
		<table style="font-size:8pt;border-color:#999999;border-style:solid;border-width:1px;border-collapse:collapse" 
			width="80%" cellspacing="0">
			<tr valign="top">
				<td colspan="2" style="border-bottom:1px solid #999999;background-color:yellow;font-size:10pt;">
					<xsl:apply-templates select="name" mode="slashdoc"/>
				</td>
			</tr>
			<tr valign="top">
				<td style="border-bottom:1px solid #999999;border-right:1px solid #999999;" width="110px"><b>Description:</b></td>
				<td style="border-bottom:1px solid #999999;">
					<xsl:apply-templates select="description" mode="slashdoc"/>
				</td>
			</tr>
			<xsl:if test="possibleValues">
				<tr valign="top">
					<td style="border-bottom:1px solid #999999;border-right:1px solid #999999;" width="110px"><b>Possible Values:</b></td>
					<td style="border-bottom:1px solid #999999;">
						<xsl:apply-templates select="possibleValues" mode="slashdoc"/>
					</td>
				</tr>
			</xsl:if>
			<xsl:if test="defaultValue">
				<tr valign="top">
					<td style="border-right:1px solid #999999;" width="110px"><b>Default Value:</b></td>
					<td><xsl:apply-templates select="defaultValue" mode="slashdoc"/></td>
				</tr>
			</xsl:if>
		</table>
		<br/>
	</xsl:template>
	
	<xsl:template match="name" mode="slashdoc">
		<b><xsl:value-of select="."/></b>
		<xsl:if test="@required='yes'"><span style="font-size:7pt;font-style:italic;"> (required)</span></xsl:if>
	</xsl:template>
	
</xsl:stylesheet>

