using System.Web.UI.WebControls;

namespace D_ODPPortalUI.Controls
{
    /// <summary>
    /// Removes auto-generated style="border-width:0px;" from ImageButton so that a border can be applied through CSS
    /// </summary>
    public class BorderlessImageButton : ImageButton
    {
        public override Unit BorderWidth
        {
            get
            {
                if (base.BorderWidth.IsEmpty)
                {
                    return Unit.Pixel(0);
                }
                else
                {
                    return base.BorderWidth;
                }
            }
            set { base.BorderWidth = value; }
        }
    }
}