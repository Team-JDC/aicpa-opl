using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace D_ODPPortalUI
{
    public partial class Footer : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowToolBar(false);
                mvwFooter.SetActiveView(vwHome);
            }
        }

        protected void lbtTabHome_Click(object sender, EventArgs e)
        {
            SwitchToTab(vwHome, "divTabHome");
        }

        protected void lbtTabDocuments_Click(object sender, EventArgs e)
        {
            SwitchToTab(vwDocuments, "divTabDocuments");
            AddScriptToMultiView("LoadMyDocuments();", "key2");
        }

        protected void lbtTabTools_Click(object sender, EventArgs e)
        {
            SwitchToTab(vwTools, "divTabTools");
        }

        protected void lbtTabPreferences_Click(object sender, EventArgs e)
        {
            SwitchToTab(vwPreferences, "divTabPreferences");
        }

        private void SwitchToTab(View view, string tabDiv)
        {
            mvwFooter.SetActiveView(view);

            if (pnlToolbar.Visible)
            {
                ShowTab(tabDiv, true);
            }
            else
            {
                ShowTab(tabDiv, false);
                ShowToolBar(true);
            }
        }

        private void ShowTab(string tabId, bool fadeIn)
        {
            string script;

            if (fadeIn)
            {
                script = string.Format("$('#{0}').hide().fadeIn();", tabId);
            }
            else
            {
                script = string.Format("$('#{0}').show();", tabId);
            }

            AddScriptToMultiView(script);
        }

        private void AddScriptToMultiView(string script)
        {
            AddScriptToMultiView(script, "multiview_ajax_key");
        }

        private void AddScriptToMultiView(string script, string key)
        {
            ScriptManager.RegisterStartupScript(mvwFooter, typeof (MultiView), key, script, true);
        }

        protected void ibtShowToolBar_Click(object sender, EventArgs e)
        {
            ShowToolBar(true);
        }

        private void ShowToolBar(bool slideIn)
        {
            ToggleShowHideToolbar(true);

            if (slideIn)
            {
                SlideInToolbar();
            }
        }

        private void SlideInToolbar()
        {
            string script = "$('#panel').hide().slideToggle('slow');";
            AddScriptToMultiView(script);
        }

        protected void ibtHideToolBar_Click(object sender, EventArgs e)
        {
            ToggleShowHideToolbar(false);
        }

        private void ToggleShowHideToolbar(bool shouldShow)
        {
            pnlToolbar.Visible = shouldShow;
            ibtShowToolBar.Visible = !shouldShow;
            ibtHideToolBar.Visible = shouldShow;
        }
    }
}