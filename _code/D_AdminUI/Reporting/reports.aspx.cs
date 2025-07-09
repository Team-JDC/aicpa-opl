using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using AICPA.Destroyer.Content.Book;
using AICPA.Destroyer.Content.Document;

namespace D_AdminUI.Reporting
{
	/// <summary>
	/// Summary description for reports.
	/// </summary>
	public partial class reports : reporting_db
	{
		protected System.Web.UI.WebControls.DataGrid dd_userBooksLabel;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			int reportType = Convert.ToInt16(Request.QueryString["reportType"]);
			
			switch(reportType)
			{
				case 0:
						this.loadTopBooks();
					break;
				case 1:
						this.loadTopUsers(1);
					break;
				case 2:
						this.searchedWords(true);
					break;
				case 3:
						this.searchedWords(false);
					break;
				case 4:
						this.errorSummaryReport();
					break;
				case 5:
						this.errorDetailReport("500");
					break;
				case 6:
						this.errorDetailReport("404");
					break;
				case 7:
						this.loadTopUsers(2);
					break;
			}
		}


		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.user_access.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.user_access_ItemCommand);
			this.exam_access.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.exam_access_ItemCommand);
			this.top_documents.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.top_documents_ItemCommand);

		}
		#endregion

		#region reports

		#region mainReports

		private void loadTopUsers(int userType)
		{
			if(userType == 1)
			{
				this.user_access.DataSource = this.getTopUsersList(30,1);
				this.user_access.DataBind();
				this.titleLabel.Text = "Top 25 C2B Users";
				this.titleLabel.Visible = true;
			}
			else
			{
				this.exam_access.DataSource = this.getTopUsersList(30,2);
				this.exam_access.DataBind();
				this.titleLabel.Text = "Top 25 Exam Candidates Users";
				this.titleLabel.Visible = true;
			}
		}

		private void loadTopBooks()
		{
			this.top_documents.DataSource = this.getTopBooksList(30);
			this.top_documents.DataBind();
			this.top_documents.Visible = true;
			this.titleLabel.Text = "Books Report";
			this.titleLabel.Visible = true;
		}

		private void searchedWords(bool found)
		{
			if(found)
			{
				this.searchFound.DataSource = this.getSearchKeys(30,found);
				this.searchFound.DataBind();
				this.searchFound.Visible = true;
				this.titleLabel.Text = "Searched Keywords with results";
				this.titleLabel.Visible = true;
			}
			else
			{
				this.searchNotFound.DataSource = this.getSearchKeys(30,found);
				this.searchNotFound.DataBind();
				this.searchNotFound.Visible = true;
				this.titleLabel.Text = "Searched Keywords without results";
				this.titleLabel.Visible = true;
			}
		}

		private void errorSummaryReport()
		{
			this.errorSummary.DataSource = this.getErrorSummary(30);
			this.errorSummary.DataBind();
			this.errorSummary.Visible = true;
			this.titleLabel.Text = "Errors Summary";
			this.titleLabel.Visible = true;
		}

		#endregion mainReports
		#region drilldowns

		private void userAccessedBooks(string userId)
		{
			this.dd_userBooks.DataSource = this.getUserAccessedBooks(30,userId);
			this.dd_userBooks.DataBind();
			this.titleLabel.Visible = true;
			this.titleLabel.Text = "Books Visited by <b>"+userId+"</b>";
			this.goBkBtn.Visible = true;
		}

		private void userAccessHistory(string userId)
		{
			this.dd_userHist.DataSource = this.getUserAccessHistory(30,userId);
			this.dd_userHist.DataBind();
			this.dd_userHist.Visible = true;
			this.titleLabel.Visible = true;
			this.titleLabel.Text = "Visits Log for User: <b>"+userId+"</b>";
			this.goBkBtn.Visible = true;
		}

		private void bookUserCount(string description)
		{
			this.dd_bookUsers.DataSource = this.getBookUserList(30,description);
			this.dd_bookUsers.DataBind();
			this.dd_bookUsers.Visible = true;
			this.titleLabel.Text = "Book Users";
			this.titleLabel.Visible = true;
			this.goBkBtn.Visible = true;
		}

		private void errorDetailReport(string errorType)
		{
			this.errorDetail.DataSource = this.getErrorDetail(30,errorType);
			this.errorDetail.DataBind();
			this.errorDetail.Visible = true;
			if(errorType == "500")
			{
				this.titleLabel.Text = "System Errors Summary";
			}
			else if(errorType == "404")
			{
				this.titleLabel.Text = "Navagation Errors Summary";
			}
			this.titleLabel.Visible = true;
		}


		private void bookDocumentReport(string bookName,string book)
		{
			this.bookDocument.DataSource = this.getBookDocumentVisits(30,bookName);
			this.bookDocument.DataBind();
			this.bookDocument.Visible = true;
			this.titleLabel.Text = book + " documents.";
			this.titleLabel.Visible = true;
			this.goBkBtn.Visible = true;
		}

		#endregion drilldowns
		#region reportsHandlers

		private void user_access_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			switch(e.CommandName)
			{
				case "getUserBooks":
					this.hideAllControls();
					this.userAccessedBooks(e.Item.Cells[4].Text);
					this.dd_userBooks.Visible = true;
				break;
				case "getUserHist":
					this.hideAllControls();
					this.userAccessHistory(e.Item.Cells[4].Text);
					this.dd_userBooks.Visible = true;
					break;
				case "getBookUsers":
					this.hideAllControls();
					this.bookUserCount(e.Item.Cells[4].Text);
					break;
			}
		}

		private void exam_access_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			switch(e.CommandName)
			{
				case "getUserBooks":
					this.hideAllControls();
					this.userAccessedBooks(e.Item.Cells[5].Text);
					this.dd_userBooks.Visible = true;
					break;
				case "getUserHist":
					this.hideAllControls();
					this.userAccessHistory(e.Item.Cells[5].Text);
					this.dd_userBooks.Visible = true;
					break;
				case "getBookUsers":
					this.hideAllControls();
					this.bookUserCount(e.Item.Cells[5].Text);
					break;
			}
		}

		private void top_documents_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			switch(e.CommandName)
			{
				case "getBookUsers":
					this.hideAllControls();
					this.bookUserCount(e.Item.Cells[4].Text);
					break;
				case "getBookDocuments":
					this.hideAllControls();
					this.bookDocumentReport(e.Item.Cells[4].Text,e.Item.Cells[5].Text);
					break;
			}
		}

		private void hideAllControls()
		{
			this.user_access.Visible = false;
			this.exam_access.Visible = false;
			this.top_documents.Visible = false;
			this.dd_userBooks.Visible = false;
			this.dd_userHist.Visible = false;
			this.dd_bookUsers.Visible = false;
			this.searchFound.Visible = false;
			this.searchNotFound.Visible = false;
			this.titleLabel.Visible = false;
			this.goBkBtn.Visible = false;
			this.errorSummary.Visible = false;
			this.errorDetail.Visible = false;
			this.bookDocument.Visible = false;
		}


		#endregion reportsHandlers

		#endregion reports
	}
}
