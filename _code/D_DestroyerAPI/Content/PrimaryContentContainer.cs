#region

using System;
using AICPA.Destroyer.Content.Site;
using AICPA.Destroyer.Shared;

#endregion

namespace AICPA.Destroyer.Content
{
    /// <summary>
    /// 	
    /// </summary>
    public static class PrimaryContentContainer
    {
        /// <summary>
        /// Constructs the container.	
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="id">The id.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static IPrimaryContentContainer ConstructContainer(ISite site, int id, NodeType type)
        {
            IPrimaryContentContainer container;

            switch (type)
            {
                case NodeType.SiteFolder:
                    container = new SiteFolder(site, id);
                    break;

                case NodeType.Book:
                    container = new Book.Book(site, id);
                    break;

                case NodeType.Document:
                    container = new Document.Document(id);
                    break;

                case NodeType.DocumentAnchor:
                    container = new Document.DocumentAnchor(id);
                    break;

                /*
                 *  MKM 05/28/2010: There is currently no use case for retrieving content for NodeTypes not
                 *  listed above (Site, Format, BookFolder).
                 */
                default:
                    throw new Exception("Could not consturct IPrimaryContentContainer for type " + type);
            }

            return container;
        }

        /// <summary>
        /// Constructs the container.	
        /// </summary>
        /// <param name="site">The site.</param>
        /// <param name="id">The id.</param>
        /// <param name="nodeType">Type of the node.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static IPrimaryContentContainer ConstructContainer(ISite site, int id, string nodeType)
        {
            return ConstructContainer(site, id, (NodeType) Enum.Parse(typeof (NodeType), nodeType));
        }
    }
}