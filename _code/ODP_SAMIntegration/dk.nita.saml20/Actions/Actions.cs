using System;
using System.Collections.Generic;
using dk.nita.saml20.config;

namespace dk.nita.saml20.Actions
{
    /// <summary>
    /// 
    /// </summary>
    public class Actions
    {
        /// <summary>
        /// Gets the default actions. 
        /// </summary>
        /// <returns></returns>
        public static List<IAction> GetDefaultActions()
        {
            var actions = new List<IAction>();
            actions.Add(new SamlPrincipalAction());
            actions.Add(new RedirectAction());
            return actions;
        }

        /// <summary>
        /// Gets the actions.
        /// </summary>
        /// <returns></returns>
        public static List<IAction> GetActions()
        {
            List<IAction> actions = GetDefaultActions();
            var config = ConfigurationReader.GetConfig<FederationConfig>();

            foreach (ActionConfigAbstract ac in config.Actions.ActionList)
            {
                if (ac is ActionConfigClear)
                    actions.Clear();
                else if (ac is ActionConfigRemove)
                {
                    actions.RemoveAll(delegate(IAction a) { return a.Name == ac.Name; });
                }
                else if (ac is ActionConfigAdd)
                {
                    var addAction = (ActionConfigAdd) ac;
                    var add = (IAction) Activator.CreateInstance(Type.GetType(addAction.Type));
                    actions.Add(add);
                }
            }

            return actions;
        }
    }
}