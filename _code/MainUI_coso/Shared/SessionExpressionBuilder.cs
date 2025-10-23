using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Web;
using System.Web.Compilation;

namespace skmExpressionBuilders
{
    [ExpressionPrefix("Session"), ExpressionEditor("skmExpressionBuilders.Design.SessionExpressionEditor, skmExpressionBuilders")]
    public class SessionExpressionBuilder : BaseServerObjectExpressionBuilder
    {
        public static SessionExpressionBuilder Instance()
        {
            try
            {
                return new SessionExpressionBuilder();
            }
            catch
            {
                return null;
            }
        }

        protected override object GetValue(string key)
        {
            return HttpContext.Current.Session[key];
        }

        public override string SourceObjectName
        {
            get { return "Session"; }
        }    
    }
}
