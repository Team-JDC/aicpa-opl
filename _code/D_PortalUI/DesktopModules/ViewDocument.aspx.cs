using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;

namespace AICPA.Destroyer.UI.Portal {

    public partial class ViewDocument : System.Web.UI.Page {

        int documentId = -1;

        //*******************************************************
        //
        // The Page_Load event handler on this Page is used to
        // obtain obtain the contents of a document from the 
        // Documents table, construct an HTTP Response of the
        // correct type for the document, and then stream the 
        // document contents to the response.  It uses the 
        // AICPA.Destroyer.UI.Portal.DocumentDB() data component to encapsulate 
        // the data access functionality.
        //
        //*******************************************************

        protected void Page_Load(object sender, System.EventArgs e) {

            if (Request.Params["DocumentId"] != null) {
                documentId = Int32.Parse(Request.Params["DocumentId"]);
            }

            if (documentId != -1) {
        
                // Obtain Document Data from Documents table
                AICPA.Destroyer.UI.Portal.DocumentDB documents = new AICPA.Destroyer.UI.Portal.DocumentDB();
            
                SqlDataReader dBContent = documents.GetDocumentContent(documentId);
                dBContent.Read();

                // Serve up the file by name
                Response.AppendHeader("content-disposition","filename=" + (String)dBContent["FileName"]);          
            
                // set the content type for the Response to that of the 
                // document to display.  For example. "application/msword"
                Response.ContentType = (String) dBContent["ContentType"];
            
                // output the actual document contents to the response output stream
                Response.OutputStream.Write((byte[]) dBContent["Content"], 0, (int) dBContent["ContentSize"]);

                // end the response
                Response.End();
            }
        }

        public ViewDocument() {
            Page.Init += new System.EventHandler(Page_Init);
        }

        protected void Page_Init(object sender, EventArgs e) {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
        }

		#region Web Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {    
        }
		#endregion
    }
}
