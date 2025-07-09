/*
 * RadMultiPage
 */

function RadMultiPage(
	id,
	pageViews,
	selectedIndex
	)
{
	this.ID = id;
	this.PageViews = pageViews;
	this.SelectedIndex = selectedIndex;
	this.RadMultiPageSpecialTagName = "ca1bcf481c";
}

RadMultiPage.prototype.Init = function()
{
	if (null == this.PageViews) { return; }

	for (var i = 0; i < this.PageViews.length; i++)
	{
		this.PageViews[i].Parent = this;
		this.PageViews[i].Index = i;
	}
};

RadMultiPage.prototype.SelectPageById = function(id)
{
	var found = false;

	for (var i = 0; i < this.PageViews.length; i++)
	{
		if (id == this.PageViews[i].ID)
		{
			var showPage = document.getElementById(this.PageViews[i].ID);
			if (showPage)
			{
				if (this.SelectedIndex > -1)
				{
					document.getElementById(this.PageViews[this.SelectedIndex].ID).style.display = "none";
				}

				showPage.style.display = "";
				this.SelectedIndex = i;
				document.getElementById(this.RadMultiPageSpecialTagName + "_" + this.ID + "_Selected").value = i;
				found = true;
			}
			break;
		}
	}
};

RadMultiPage.prototype.SelectPageByIndex = function(index)
{
	if (index + 1 > this.PageViews.length) { return; }

	if (index > -1)
	{
		var showPage = document.getElementById(this.PageViews[index].ID);
		if (showPage)
		{
			if (this.SelectedIndex > -1)
			{
				document.getElementById(this.PageViews[this.SelectedIndex].ID).style.display = "none";
			}

			showPage.style.display = "";
			this.SelectedIndex = index;
			document.getElementById(this.RadMultiPageSpecialTagName + "_" + this.ID + "_Selected").value = index;
		}
	}
	else if (-1 == index)
	{
		if (this.SelectedIndex > -1)
		{
			document.getElementById(this.PageViews[this.SelectedIndex].ID).style.display = "none";
		}

		this.SelectedIndex = -1;
		document.getElementById(this.RadMultiPageSpecialTagName + "_" + this.ID + "_Selected").value = -1;
	}
};

/*
 * PageView
 */

function PageView(
	id
	)
{
	this.Parent = null;
	this.ID = id;
	this.Index = -1;
}