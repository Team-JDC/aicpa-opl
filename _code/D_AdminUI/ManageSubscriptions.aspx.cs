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
using AICPA.Destroyer.Content.Subscription;
using AICPA.Destroyer.Content.Book;
using Telerik.WebControls;

namespace D_AdminUI
{
	/// <summary>
	/// Summary description for ManageSubscriptions.
	/// </summary>
	public partial class ManageSubscriptions : System.Web.UI.Page
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if(!Page.IsPostBack)
			{
				this.BuildSubscriptionTree();
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
			this.SubscriptionTree.NodeContextClick += new Telerik.WebControls.RadTreeView.RadTreeViewEventHandler(this.SubscriptionTree_OnNodeContextClick);

		}
		#endregion

		#region Properties
		private SubscriptionCollection activeSubscriptionCollection;
		private SubscriptionCollection ActiveSubscriptionCollection
		{
			get
			{
				if (activeSubscriptionCollection == null)
				{
					activeSubscriptionCollection = new SubscriptionCollection();
				}
				return activeSubscriptionCollection;
			}
		}
		protected string CurrentContextMenuSubscriptionCode
		{
			get 
			{
				object o = ViewState["currentContextMenuSubscriptionCode"];
				if (o == null) 
				{
					return null;
				}
				return (string)o;
			}

			set 
			{
				ViewState["currentContextMenuSubscriptionCode"] = value;
			}
		}
		#endregion Properties

		#region Constants 
		private const string CONTEXT_ADDSUBSCRIPTION = "AddSubscription";
		private const string CONTEXT_EDITSUBSCRIPTION = "EditSubscription";
		private const string CONTEXT_DELETESUBSCRIPTION = "DeleteSubscription";
		private const string CONTEXT_ADDBOOK = "AddBook";
		private const string CONTEXT_REMOVEBOOK = "RemoveBook";
		private const string TREE_CONTEXTMENUCONTENTFILE = "xmlFiles/treeContentMenu.xml";
		private const string TREE_ROOT_IMAGE = "images/TreeIcons/Icons/oInboxF.gif";
		private const string TREE_ROOT_CONTEXT_MENU = "SubscriptionRootMenu";
		private const string TREE_SUBSCRIPTION_PREFIX = "S-";
		private const string TREE_SUBSCRIPTION_IMAGE = "images/TreeIcons/Icons/ball_blueS.gif";
		private const string TREE_SUBSCRIPTION_CONTEXT_MENU = "SubscriptionSubscriptionMenu";
		private const string TREE_BOOK_IMAGE = "images/TreeIcons/Icons/book.gif";
		private const string TREE_BOOK_CONTEXT_MENU = "SubscriptionBookMenu";
		private const string TREE_ROOT_ID = "root_node";
		private const string NODE_CATEGORY_ROOT = "RootNode";
		private const string NODE_CATEGORY_SUBSCRIPTION = "SubscriptionNode";
		private const string NODE_CATEGORY_BOOK = "BookNode";
		#endregion Constants

		#region Methods
		#region Tree Building and Unbuilding Methods
		private void BuildSubscriptionTree()
		{
			SubscriptionTree.ContextMenuContentFile = TREE_CONTEXTMENUCONTENTFILE;
			SubscriptionTree.BeforeClientContextClick = "setPos();";
			RadTreeNode rootNode = new RadTreeNode("Subscriptions");
			rootNode.Image = TREE_ROOT_IMAGE;
			rootNode.ID = TREE_ROOT_ID;
			rootNode.Category = NODE_CATEGORY_ROOT;
			rootNode.ContextMenuName = TREE_ROOT_CONTEXT_MENU;
			rootNode.Expanded = true;
			SubscriptionTree.AddNode(rootNode);			
			foreach(Subscription subscription in this.ActiveSubscriptionCollection)
			{
				RadTreeNode subscriptionNode = this.AddSubscriptionToRootNode(subscription.Title, subscription.Description, subscription.Code);
				foreach(string bookName in subscription.BookNames)
				{
					this.AddBookToSubscriptionNode(subscriptionNode, bookName);					
				}
			}
			SubscriptionTree.ExpandAllNodes();
			SubscriptionTree.Sort = RadTreeViewSort.Ascending;
		}

		private void AddBookToSubscriptionNode(RadTreeNode subscriptionNode, string bookName)
		{
			RadTreeNode newBookNode = new RadTreeNode(bookName);
			newBookNode.Value = bookName;
			newBookNode.Image = TREE_BOOK_IMAGE;
			newBookNode.ContextMenuName = TREE_BOOK_CONTEXT_MENU;
			newBookNode.Category = NODE_CATEGORY_BOOK;
			subscriptionNode.AddNode(newBookNode);
		}

		private RadTreeNode AddSubscriptionToRootNode(string title, string description, string code)
		{
			RadTreeNode subscriptionNode = new RadTreeNode(title);
			subscriptionNode.Category = NODE_CATEGORY_SUBSCRIPTION;
			subscriptionNode.ToolTip = description;
			subscriptionNode.Value = code;
			subscriptionNode.ID = TREE_SUBSCRIPTION_PREFIX + code;
			subscriptionNode.Image = TREE_SUBSCRIPTION_IMAGE;
			subscriptionNode.ContextMenuName = TREE_SUBSCRIPTION_CONTEXT_MENU;

			RadTreeNode rootNode = SubscriptionTree.FindNodeById(TREE_ROOT_ID);
			rootNode.AddNode(subscriptionNode);

			return subscriptionNode;
		}

		#endregion Tree Building and Unbuilding Methods

		private void SubscriptionTree_OnNodeContextClick(object o, Telerik.WebControls.RadTreeNodeEventArgs e)
		{
			if (e.NodeClicked.Category == NODE_CATEGORY_SUBSCRIPTION)
			{
				this.CurrentContextMenuSubscriptionCode = e.NodeClicked.Value;
			}
			else 
			{
				this.CurrentContextMenuSubscriptionCode = null;
			}
			switch(e.ContextMenuItemID)
			{
				case CONTEXT_ADDSUBSCRIPTION :
					this.SubscriptionCode.Enabled = true;
					this.HideComponents();
					this.labelSubscriptionTitle.Visible = true;
					this.labelSubscriptionDescription.Visible = true;
					this.SubscriptionCode.Visible = true;
					this.labelSubscriptionCode.Visible = true;
					this.SubscriptionDescription.Visible = true;
					this.SubscriptionTitle.Visible = true;
					this.saveButton.Visible = true;
					this.cancelButton.Visible = true;
					this.SubscriptionCode.Text = "";
					this.SubscriptionDescription.Text = "";
					this.SubscriptionTitle.Text = "";

					this.jsLabel.Text = "<script>showmenu("+ this.xPos.Value +","+ this.yPos.Value +");</script>";
					this.jsLabel.Visible = true;
					break;
				case CONTEXT_EDITSUBSCRIPTION :
					this.SubscriptionCode.Enabled = false;
					this.HideComponents();
					this.labelSubscriptionTitle.Visible = true;
					this.labelSubscriptionDescription.Visible = true;
					this.SubscriptionCode.Visible = true;
					this.labelSubscriptionCode.Visible = true;
					this.SubscriptionDescription.Visible = true;
					this.SubscriptionTitle.Visible = true;
					this.saveButton.Visible = true;
					this.cancelButton.Visible = true;

					this.SubscriptionCode.Text = e.NodeClicked.Value;
					this.SubscriptionDescription.Text = e.NodeClicked.ToolTip;
					this.SubscriptionTitle.Text = e.NodeClicked.Text;

					this.jsLabel.Text = "<script>showmenu("+ this.xPos.Value +","+ this.yPos.Value +");</script>";
					this.jsLabel.Visible = true;
					break;
				case CONTEXT_DELETESUBSCRIPTION:
					this.ActiveSubscriptionCollection.Remove(this.ActiveSubscriptionCollection[this.CurrentContextMenuSubscriptionCode]);
					this.ActiveSubscriptionCollection.Save();
					e.NodeClicked.Remove();
					break;
				case CONTEXT_ADDBOOK:
					this.HideComponents();
					this.BookDB.Visible = true;
					this.addBookButton.Visible = true;
					this.cancelButton.Visible = true;
					this.BindBook();

					this.jsLabel.Text = "<script>showmenu("+ this.xPos.Value +","+ this.yPos.Value +");</script>";
					this.jsLabel.Visible = true;
					break;
				case CONTEXT_REMOVEBOOK:
					Subscription editSubscription = (Subscription)this.ActiveSubscriptionCollection[e.NodeClicked.Parent.Value];
					editSubscription.RemoveBook(e.NodeClicked.Value);
					editSubscription.Save();
					e.NodeClicked.Remove();
					this.HideComponents();
					break;
			}
			
		}
		protected void addBookButton_Click(object sender, System.EventArgs e)
		{
			Subscription editSubscription = (Subscription)this.ActiveSubscriptionCollection[this.CurrentContextMenuSubscriptionCode];
			
			try
			{
				editSubscription.AddBook(this.BookDB.SelectedValue);
				editSubscription.Save();
				RadTreeNode subscriptionNode = SubscriptionTree.FindNodeById(TREE_SUBSCRIPTION_PREFIX+this.CurrentContextMenuSubscriptionCode);
				this.AddBookToSubscriptionNode(subscriptionNode, this.BookDB.SelectedValue);
				this.HidePopUp();
			}
			catch(AICPA.Destroyer.Shared.BusinessRuleException error)
			{
				this.jsLabel.Text = string.Format("<script>hidemenu();alert('{0}');</script>",error.Message.Replace("'", "\\'"));
			}
		}
		protected void saveButton_Click(object sender, System.EventArgs e)
		{
			if(this.CurrentContextMenuSubscriptionCode == null)
			{
				Subscription newSubscription = new Subscription(this.SubscriptionCode.Text, new string[]{});
				newSubscription.Title = this.SubscriptionTitle.Text;
				newSubscription.Description = this.SubscriptionDescription.Text;
				this.ActiveSubscriptionCollection.Add(newSubscription);
				this.ActiveSubscriptionCollection.Save();
				this.AddSubscriptionToRootNode(this.SubscriptionTitle.Text, this.SubscriptionDescription.Text, this.SubscriptionCode.Text);
				this.SubscriptionTree.ExpandAllNodes();
			}
			else
			{
				Subscription editSubscription = (Subscription)this.ActiveSubscriptionCollection[this.CurrentContextMenuSubscriptionCode];
				editSubscription.Description = this.SubscriptionDescription.Text;
				editSubscription.Title = this.SubscriptionTitle.Text;
				editSubscription.Save();
				RadTreeNode subscriptionNode = SubscriptionTree.FindNodeById(TREE_SUBSCRIPTION_PREFIX + this.CurrentContextMenuSubscriptionCode);
				subscriptionNode.ToolTip = this.SubscriptionDescription.Text;
				subscriptionNode.Value = this.SubscriptionCode.Text;
				subscriptionNode.Text = this.SubscriptionTitle.Text;
				subscriptionNode.ID = TREE_SUBSCRIPTION_PREFIX + this.SubscriptionCode.Text;
			}
			this.HidePopUp();

		}
		protected void cancelButton_Click(object sender, System.EventArgs e)
		{
			this.HidePopUp();
		}

		private void HideComponents()
		{
			this.saveButton.Visible = false;
			this.addBookButton.Visible = false;
			this.cancelButton.Visible = false;
			this.labelSubscriptionDescription.Visible = false;
			this.labelSubscriptionTitle.Visible = false;
			this.BookDB.Visible = false;
			this.SubscriptionCode.Visible = false;
			this.SubscriptionDescription.Visible = false;
			this.SubscriptionTitle.Visible = false;
			this.SubscriptionCode.Visible = false;
			this.labelSubscriptionCode.Visible = false;
			this.labelNoBooksToAdd.Visible = false;
		}
		private void HidePopUp()
		{
			this.jsLabel.Text = "<script>hidemenu();</script>";
			this.jsLabel.Visible = true;
			HideComponents();
		}
		private void BindBook()
		{		
			// Only bind to books that the subscription doesn't already have.
			ArrayList subscriptionBooks = new ArrayList();
			foreach(string book in this.ActiveSubscriptionCollection[this.CurrentContextMenuSubscriptionCode].BookNames)
			{
				subscriptionBooks.Add(book);
			}
			BookCollection bookCollection = new BookCollection(true, false);
			ArrayList booksNotInSubscription = new ArrayList();
			foreach(Book book in bookCollection)
			{
				if(!subscriptionBooks.Contains(book.Name))
				{
					booksNotInSubscription.Add(book.Name);
				}
			}
			// only show the books if there are books to show.
			if(booksNotInSubscription.Count > 0)
			{
				BookDB.DataSource = booksNotInSubscription;
				BookDB.DataBind();
				BookDB.Visible=true;
			}
			else
			{
				BookDB.Visible=false;
				this.addBookButton.Visible = false;
				this.labelNoBooksToAdd.Visible = true;
			}			
		}
		#endregion Methods

	}
}
