using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AICPA.Destroyer.Shared;

namespace MainUI.Shared
{
    public class FormatOption
    {
        public FormatOption()
        {
        }

        public FormatOption(int formatId, int docId)
        {
            FormatId = formatId;
            FormatName = Enum.GetName(typeof(ContentType), (ContentType)formatId);
            FormatShortName = EnumHelper.GetDescription((ContentType)formatId); 
            DocumentId = docId;
        }

        public FormatOption(ContentType contentType, int docId)
        {
            FormatId = (int)contentType;
            FormatName = Enum.GetName(typeof(ContentType), contentType);
            FormatShortName = EnumHelper.GetDescription(contentType); 
            DocumentId = docId;
        }

        public int DocumentId { get; set; }
        public int FormatId { get; set; }
        public string FormatName { get; set; }
        /// <summary>
        /// This is the name of the document type
        /// </summary>
        public string FormatShortName { get; set; }
    }
}