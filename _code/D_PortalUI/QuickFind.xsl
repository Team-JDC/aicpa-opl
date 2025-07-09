<?xml version="1.0" encoding="UTF-8" ?>
<stylesheet version="1.0" xmlns="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="/">
<xsl:apply-templates/>
</xsl:template>
<xsl:template match="Taxonomies">
<xsl:apply-templates/>
</xsl:template>
<xsl:template match="Taxonomy">
<Tree>
<Node Image="taxonomy.gif" Text={@Title}>
<xsl:apply-templates/>
</Node>
</Tree>
</xsl:template>
<xsl:template match="TaxonomyValue">
<Node Image="category.gif" Text={@Title}>
<xsl:apply-templates/>
</Node>
</xsl:template>
</stylesheet>

  