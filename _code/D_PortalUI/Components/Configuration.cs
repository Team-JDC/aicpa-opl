using System;
using System.Configuration;
using System.Web;
using System.Data;
using System.Web.Caching;
using System.Data.SqlClient;
using System.Collections;
using AICPA.Destroyer.UI.Portal;

namespace AICPA.Destroyer.UI.Portal 
{

	//*********************************************************************
	//
	// Configuration Class
	//
	// Class that encapsulates all data logic necessary to add/query/delete
	// tab configuration settings, module configuration settings and module 
	// definition configuration settings from the PortalCfg.xml file.
	//
	//*********************************************************************
	public class Configuration
	{

		//
		// PORTAL
		//

		//*********************************************************************
		//
		// UpdatePortalInfo() Method <a name="UpdatePortalInfo"></a>
		//
		// The UpdatePortalInfo method updates the name and access settings for the portal.
		// These settings are stored in the Xml file PortalCfg.xml.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public void UpdatePortalInfo (int portalId, String portalName, bool alwaysShow) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Get first record of the "Global" element 
			SiteConfiguration.GlobalRow globalRow = siteSettings.Global.FindByPortalId(portalId);

			// Update the values
			globalRow.PortalId = portalId;
			globalRow.PortalName = portalName;
			globalRow.AlwaysShowEditButton = alwaysShow;

			// Save the changes 
			SaveSiteSettings();
		}
		
		//
		// TABS
		//

		//*********************************************************************
		//
		// AddTab Method <a name="AddTab"></a>
		//
		// The AddTab method adds a new tab to the portal.  These settings are 
		// stored in the Xml file PortalCfg.xml.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public int AddTab (int portalId, String tabName, int tabOrder) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Create a new TabRow from the Tab table
			SiteConfiguration.TabRow newRow = siteSettings.Tab.NewTabRow();

			// Set the properties on the new row
			newRow.TabName = tabName;
			newRow.TabOrder = tabOrder;
			newRow.MobileTabName = String.Empty;
			newRow.ShowMobile = true;
			newRow.AccessRoles = "All Users;";

			// Add the new TabRow to the Tab table
			siteSettings.Tab.AddTabRow(newRow);

			// Save the changes 
			SaveSiteSettings();

			// Return the new TabID
			return newRow.TabId;
		}
        
        
		//*********************************************************************
		//
		// UpdateTab Method <a name="UpdateTab"></a>
		//
		// The UpdateTab method updates the settings for the specified tab. 
		// These settings are stored in the Xml file PortalCfg.xml.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public void UpdateTab (int portalId, int tabId, String tabName, int tabOrder, String authorizedRoles, String mobileTabName, bool showMobile) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Find the appropriate tab in the Tab table and set the properties
			SiteConfiguration.TabRow tabRow = siteSettings.Tab.FindByTabId(tabId);

			tabRow.TabName = tabName;
			tabRow.TabOrder = tabOrder;
			tabRow.AccessRoles = authorizedRoles;
			tabRow.MobileTabName = mobileTabName;
			tabRow.ShowMobile = showMobile;

			// Save the changes 
			SaveSiteSettings();
		}
		
		//*********************************************************************
		//
		// UpdateTabOrder Method <a name="UpdateTabOrder"></a>
		//
		// The UpdateTabOrder method changes the position of the tab with respect
		// to other tabs in the portal.  These settings are stored in the Xml 
		// file PortalCfg.xml.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public void UpdateTabOrder (int tabId, int tabOrder) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Find the appropriate tab in the Tab table and set the property
			SiteConfiguration.TabRow tabRow = siteSettings.Tab.FindByTabId(tabId);

			tabRow.TabOrder = tabOrder;

			// Save the changes 
			SaveSiteSettings();
		}
		
		//*********************************************************************
		//
		// DeleteTab Method <a name="DeleteTab"></a>
		//
		// The DeleteTab method deletes the selected tab and its modules from 
		// the settings which are stored in the Xml file PortalCfg.xml.  This 
		// method also deletes any data from the database associated with all 
		// modules within this tab.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//	  + <a href="DeleteModule.htm" style="color:green">DeleteModule stored procedure</a>
		//
		//*********************************************************************
		public void DeleteTab(int tabId) 
		{
			//
			// Delete the Tab in the XML file
			//

			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Find the appropriate tab in the Tab table
			SiteConfiguration.TabDataTable tabTable = siteSettings.Tab;
			SiteConfiguration.TabRow tabRow = siteSettings.Tab.FindByTabId(tabId);

			//
			// Delete information in the Database relating to each Module being deleted
			//

			// Create Instance of Connection and Command Object
			//SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["connectionString"]);
            // updated 1/20/10 djf
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand myCommand = new SqlCommand("Portal_DeleteModule", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			myConnection.Open();

			foreach(SiteConfiguration.ModuleRow moduleRow in tabRow.GetModuleRows())
			{
				myCommand.Parameters.Clear();
				parameterModuleID.Value = moduleRow.ModuleId;
				myCommand.Parameters.Add(parameterModuleID);

				// Open the database connection and execute the command
				myCommand.ExecuteNonQuery();
			}

			// Close the connection
			myConnection.Close();

			// Finish removing the Tab row from the Xml file
			tabTable.RemoveTabRow(tabRow);

			// Save the changes 
			SaveSiteSettings();
		}
       
		//
		// MODULES
		//

		//*********************************************************************
		//
		// UpdateModuleOrder Method  <a name="UpdateModuleOrder"></a>
		//
		// The UpdateModuleOrder method updates the order in which the modules
		// in a tab are displayed.  These settings are stored in the Xml file 
		// PortalCfg.xml.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public void UpdateModuleOrder (int ModuleId, int ModuleOrder, String pane) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Find the appropriate Module in the Module table and update the properties
			SiteConfiguration.ModuleRow moduleRow = siteSettings.Module.FindByModuleId(ModuleId);

			moduleRow.ModuleOrder = ModuleOrder;
			moduleRow.PaneName = pane;

			// Save the changes 
			SaveSiteSettings();
		}
        
        
		//*********************************************************************
		//
		// AddModule Method  <a name="AddModule"></a>
		//
		// The AddModule method adds Portal Settings for a new Module within
		// a Tab.  These settings are stored in the Xml file PortalCfg.xml.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public int AddModule(int tabId, int moduleOrder, String paneName, String title, int moduleDefId, int cacheTime, String editRoles, bool showMobile) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Create a new ModuleRow from the Module table
			SiteConfiguration.ModuleRow newModule = siteSettings.Module.NewModuleRow();

			// Set the properties on the new Module
			newModule.ModuleDefId = moduleDefId;
			newModule.ModuleOrder = moduleOrder;
			newModule.ModuleTitle = title;
			newModule.PaneName = paneName;
			newModule.EditRoles = editRoles;
			newModule.CacheTimeout = cacheTime;
			newModule.ShowMobile = showMobile;
			newModule.TabRow = siteSettings.Tab.FindByTabId(tabId);

			// Add the new ModuleRow to the Module table
			siteSettings.Module.AddModuleRow(newModule);

			// Save the changes
			SaveSiteSettings();

			// Return the new Module ID
			return newModule.ModuleId;
		}

        
		//*********************************************************************
		//
		// UpdateModule Method  <a name="UpdateModule"></a>
		//
		// The UpdateModule method updates the Portal Settings for an existing 
		// Module within a Tab.  These settings are stored in the Xml file
		// PortalCfg.xml.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public int UpdateModule(int moduleId, int moduleOrder, String paneName, String title, int cacheTime, String editRoles, bool showMobile) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Find the appropriate Module in the Module table and update the properties
			SiteConfiguration.ModuleRow moduleRow = siteSettings.Module.FindByModuleId(moduleId);

			moduleRow.ModuleOrder = moduleOrder;
			moduleRow.ModuleTitle = title;
			moduleRow.PaneName = paneName;
			moduleRow.CacheTimeout = cacheTime;
			moduleRow.EditRoles = editRoles;
			moduleRow.ShowMobile = showMobile;

			// Save the changes 
			SaveSiteSettings();

			// Return the existing Module ID
			return moduleId;
		}

		//*********************************************************************
		//
		// DeleteModule Method  <a name="DeleteModule"></a>
		//
		// The DeleteModule method deletes a specified Module from the settings
		// stored in the Xml file PortalCfg.xml.  This method also deletes any 
		// data from the database associated with this module.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//	  + <a href="DeleteModule.htm" style="color:green">DeleteModule stored procedure</a>
		//
		//*********************************************************************
		public void DeleteModule(int moduleId) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			//
			// Delete information in the Database relating to Module being deleted
			//

			// Create Instance of Connection and Command Object
			//SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["connectionString"]);
            // updated 1/20/10 djf
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand myCommand = new SqlCommand("Portal_DeleteModule", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			myConnection.Open();

			parameterModuleID.Value = moduleId;
			myCommand.Parameters.Add(parameterModuleID);

			// Open the database connection and execute the command
			myCommand.ExecuteNonQuery();
			myConnection.Close();

			// Finish removing Module
			siteSettings.Module.RemoveModuleRow(siteSettings.Module.FindByModuleId(moduleId));

			// Save the changes 
			SaveSiteSettings();
		}


		//*********************************************************************
		//
		// UpdateModuleSetting Method  <a name="UpdateModuleSetting"></a>
		//
		// The UpdateModuleSetting Method updates a single module setting 
		// in the configuration file.  If the value passed in is String.Empty,
		// the Setting element is deleted if it exists.  If not, either a 
		// matching Setting element is updated, or a new Setting element is 
		// created.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public void UpdateModuleSetting(int moduleId, String key, String val) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Find the appropriate Module in the Module table
			SiteConfiguration.ModuleRow moduleRow = siteSettings.Module.FindByModuleId(moduleId);

			// Find the first (only) settings element
			SiteConfiguration.SettingsRow settingsRow;

			if(moduleRow.GetSettingsRows().Length > 0 )
			{
				settingsRow = moduleRow.GetSettingsRows()[0];
			}
			else
			{
				// Add new settings element
				settingsRow = siteSettings.Settings.NewSettingsRow();

				// Set the parent relationship
				settingsRow.ModuleRow = moduleRow;

				siteSettings.Settings.AddSettingsRow(settingsRow);
			}

			// Find the child setting elements
			SiteConfiguration.SettingRow settingRow;

			SiteConfiguration.SettingRow[] settingRows = settingsRow.GetSettingRows();

			if(settingRows.Length == 0)
			{
				// If there are no Setting elements at all, add one with the new name and value,
				// but only if the value is not empty
				if(val != String.Empty)
				{
					settingRow = siteSettings.Setting.NewSettingRow();

					// Set the parent relationship and data
					settingRow.SettingsRow = settingsRow;
					settingRow.Name = key;
					settingRow.Setting_Text = val;

					siteSettings.Setting.AddSettingRow(settingRow);
				}
			}
			else
			{
				// Update existing setting element if it matches
				bool found = false;
				Int32 i;

				// Find which row matches the input parameter "key" and update the
				// value.  If the value is String.Empty, however, delete the row.
				for(i=0; i < settingRows.Length; i++)
				{
					if(settingRows[i].Name == key)
					{
						if(val == String.Empty)
						{
							// Delete the row
							siteSettings.Setting.RemoveSettingRow(settingRows[i]);
						}
						else
						{
							// Update the value
							settingRows[i].Setting_Text = val;
						}

						found = true;
					}
				}
				
				if(found == false)
				{
					// Setting elements exist, however, there is no matching Setting element.
					// Add one with new name and value, but only if the value is not empty
					if(val != String.Empty)
					{
						settingRow = siteSettings.Setting.NewSettingRow();

						// Set the parent relationship and data
						settingRow.SettingsRow = settingsRow;
						settingRow.Name = key;
						settingRow.Setting_Text = val;

						siteSettings.Setting.AddSettingRow(settingRow);
					}
				}
			}

			// Save the changes 
			SaveSiteSettings();
		}

		//*********************************************************************
		//
		// GetModuleSettings Method  <a name="GetModuleSettings"></a>
		//
		// The GetModuleSettings Method returns a hashtable of custom,
		// module-specific settings from the configuration file.  This method is
		// used by some user control modules (Xml, Image, etc) to access misc
		// settings.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public static Hashtable GetModuleSettings(int moduleId) 
		{
			// Create a new Hashtable
			Hashtable _settingsHT = new Hashtable();

			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Find the appropriate Module in the Module table
			SiteConfiguration.ModuleRow moduleRow = siteSettings.Module.FindByModuleId(moduleId);

			// Find the first (only) settings element
			if(moduleRow.GetSettingsRows().Length > 0)
			{
				SiteConfiguration.SettingsRow settingsRow = moduleRow.GetSettingsRows()[0];

				if(settingsRow != null)
				{
					// Find the child setting elements and add to the hashtable
					foreach(SiteConfiguration.SettingRow sRow in settingsRow.GetSettingRows())
					{
						_settingsHT[sRow.Name] = sRow.Setting_Text;
					}
				}
			}

			return _settingsHT;
		}

		//
		// MODULE DEFINITIONS
		//

		//*********************************************************************
		//
		// GetModuleDefinitions() Method <a name="GetModuleDefinitions"></a>
		//
		// The GetModuleDefinitions method returns a list of all module type 
		// definitions for the portal.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public DataRow[] GetModuleDefinitions(int portalId) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Find the appropriate Module in the Module table
			return siteSettings.ModuleDefinition.Select();
		}

		//*********************************************************************
		//
		// AddModuleDefinition() Method <a name="AddModuleDefinition"></a>
		//
		// The AddModuleDefinition add the definition for a new module type
		// to the portal.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public int AddModuleDefinition(int portalId, String name, String desktopSrc, String mobileSrc) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Create new ModuleDefinitionRow
			SiteConfiguration.ModuleDefinitionRow newModuleDef = siteSettings.ModuleDefinition.NewModuleDefinitionRow();

			// Set the parameter values
			newModuleDef.FriendlyName = name;
			newModuleDef.DesktopSourceFile = desktopSrc;
			newModuleDef.MobileSourceFile = mobileSrc;

			// Add the new ModuleDefinitionRow to the ModuleDefinition table
			siteSettings.ModuleDefinition.AddModuleDefinitionRow(newModuleDef);

			// Save the changes
			SaveSiteSettings();

			// Return the new ModuleDefID
			return newModuleDef.ModuleDefId;
		}

		//*********************************************************************
		//
		// DeleteModuleDefinition() Method <a name="DeleteModuleDefinition"></a>
		//
		// The DeleteModuleDefinition method deletes the specified module type 
		// definition from the portal.  Each module which is related to the
		// ModuleDefinition is deleted from each tab in the configuration
		// file, and all data relating to each module is deleted from the
		// database.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//    + <a href="DeleteModule.htm" style="color:green">DeleteModule Stored Procedure</a>
		//
		//*********************************************************************
		public void DeleteModuleDefinition(int defId) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			//
			// Delete information in the Database relating to each Module being deleted
			//

			// Create Instance of Connection and Command Object
			//SqlConnection myConnection = new SqlConnection(ConfigurationSettings.AppSettings["connectionString"]);
            // updated 1/20/10 djf
            SqlConnection myConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString);
            SqlCommand myCommand = new SqlCommand("Portal_DeleteModule", myConnection);

			// Mark the Command as a SPROC
			myCommand.CommandType = CommandType.StoredProcedure;

			// Add Parameters to SPROC
			SqlParameter parameterModuleID = new SqlParameter("@ModuleID", SqlDbType.Int, 4);
			myConnection.Open();

			foreach(SiteConfiguration.ModuleRow moduleRow in siteSettings.Module.Select())
			{
				if(moduleRow.ModuleDefId == defId)
				{
					myCommand.Parameters.Clear();
					parameterModuleID.Value = moduleRow.ModuleId;
					myCommand.Parameters.Add(parameterModuleID);

					// Delete the xml module associated with the ModuleDef
					// in the configuration file
					siteSettings.Module.RemoveModuleRow(moduleRow);

					// Open the database connection and execute the command
					myCommand.ExecuteNonQuery();
				}
			}

			myConnection.Close();

			// Finish removing Module Definition
			siteSettings.ModuleDefinition.RemoveModuleDefinitionRow(siteSettings.ModuleDefinition.FindByModuleDefId(defId));

			// Save the changes 
			SaveSiteSettings();
		}
       
		//*********************************************************************
		//
		// UpdateModuleDefinition() Method <a name="UpdateModuleDefinition"></a>
		//
		// The UpdateModuleDefinition method updates the settings for the 
		// specified module type definition.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public void UpdateModuleDefinition(int defId, String name, String desktopSrc, String mobileSrc) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Find the appropriate Module in the Module table and update the properties
			SiteConfiguration.ModuleDefinitionRow modDefRow = siteSettings.ModuleDefinition.FindByModuleDefId(defId);

			modDefRow.FriendlyName = name;
			modDefRow.DesktopSourceFile = desktopSrc;
			modDefRow.MobileSourceFile = mobileSrc;

			// Save the changes 
			SaveSiteSettings();
		}

		//*********************************************************************
		//
		// GetSingleModuleDefinition Method
		//
		// The GetSingleModuleDefinition method returns a ModuleDefinitionRow
		// object containing details about a specific module definition in the
		// configuration file.
		//
		// Other relevant sources:
		//    + <a href="#SaveSiteSettings" style="color:green">SaveSiteSettings() method</a>
		//	  + <a href="PortalCfg.xml" style="color:green">PortalCfg.xml</a>
		//
		//*********************************************************************
		public SiteConfiguration.ModuleDefinitionRow GetSingleModuleDefinition(int defId) 
		{
			// Obtain SiteSettings from Current Context
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Items["SiteSettings"];

			// Find the appropriate Module in the Module table
			return siteSettings.ModuleDefinition.FindByModuleDefId(defId);
		}

		//*********************************************************************
		//
		// GetSiteSettings Static Method
		//
		// The Configuration.GetSiteSettings Method returns a typed
		// dataset of the all of the site configuration settings from the
		// XML configuration file.  This method is used in Global.asax to
		// push the settings into the current HttpContext, so that all of the 
		// pages, content modules and classes throughout the rest of the request
		// may access them.
		//
		// The SiteConfiguration object is cached using the ASP.NET Cache API,
		// with a file-change dependency on the XML configuration file.  Normallly,
		// this method just returns a copy of the object in the cache.  When the
		// configuration is updated and changes are saved to the the XML file,
		// the SiteConfiguration object is evicted from the cache.  The next time 
		// this method runs, it will read from the XML file again and insert a
		// fresh copy of the SiteConfiguration into the cache.
		//
		//*********************************************************************
		public static SiteConfiguration GetSiteSettings()
		{
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Cache["SiteSettings"];

			// If the SiteConfiguration isn't cached, load it from the XML file and add it into the cache.
			if(siteSettings == null)
			{
				// Create the dataset
				siteSettings = new SiteConfiguration();

				// Retrieve the location of the XML configuration file
				//string configFile = HttpContext.Current.Server.MapPath(ConfigurationSettings.AppSettings["configFile"]);
                //modified 1/20/10 djf
                string configFile = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["configFile"]);

				// Set the AutoIncrement property to true for easier adding of rows
				siteSettings.Tab.TabIdColumn.AutoIncrement = true;
				siteSettings.Module.ModuleIdColumn.AutoIncrement = true;
				siteSettings.ModuleDefinition.ModuleDefIdColumn.AutoIncrement = true;

				// Load the XML data into the DataSet
				siteSettings.ReadXml(configFile);
		
				// Store the dataset in the cache
				HttpContext.Current.Cache.Insert("SiteSettings", siteSettings, new CacheDependency(configFile));
			}

			return siteSettings;
		}

		//*********************************************************************
		//
		// SaveSiteSettings Method <a name="SaveSiteSettings"></a>
		//
		// The Configuration.SaveSiteSettings overwrites the the XML file with the
		// settings in the SiteConfiguration object in context.  The object will in 
		// turn be evicted from the cache and be reloaded from the XML file the next
		// time GetSiteSettings() is called.
		//
		//*********************************************************************
		public void SaveSiteSettings()
		{
			// Obtain SiteSettings from the Cache
			SiteConfiguration siteSettings = (SiteConfiguration) HttpContext.Current.Cache["SiteSettings"];

			// Check the object
			if(siteSettings == null)
			{
				// If SaveSiteSettings() is called once, the cache is cleared.  If it is
				// then called again before Global.Application_BeginRequest is called, 
				// which reloads the cache, the siteSettings object will be Null 
				siteSettings = GetSiteSettings();
			}
			//string configFile = HttpContext.Current.Server.MapPath(ConfigurationSettings.AppSettings["configFile"]);
            //modified 1/20/10 djf
            string configFile = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["configFile"]);

			// Object is evicted from the Cache here.  
			siteSettings.WriteXml(configFile);
		}
	}

	//*********************************************************************
	//
	// PortalSettings Class
	//
	// This class encapsulates all of the settings for the Portal, as well
	// as the configuration settings required to execute the current tab
	// view within the portal.
	//
	//*********************************************************************

	public class PortalSettings 
	{

		public int          PortalId;
		public String       PortalName;
		public bool         AlwaysShowEditButton;
		public ArrayList    DesktopTabs = new ArrayList();
		public ArrayList    MobileTabs = new ArrayList();
		public TabSettings  ActiveTab = new TabSettings();

		//*********************************************************************
		//
		// PortalSettings Constructor
		//
		// The PortalSettings Constructor encapsulates all of the logic
		// necessary to obtain configuration settings necessary to render
		// a Portal Tab view for a given request.
		//
		// These Portal Settings are stored within PortalCFG.xml, and are
		// fetched below by calling config.GetSiteSettings().
		// The method config.GetSiteSettings() fills the SiteConfiguration
		// class, derived from a DataSet, which PortalSettings accesses.
		//       
		//*********************************************************************

		public PortalSettings(int tabIndex, int tabId) 
		{
			// Get the configuration data
			SiteConfiguration siteSettings = Configuration.GetSiteSettings();

			// Read the Desktop Tab Information, and sort by Tab Order
			foreach(SiteConfiguration.TabRow tRow in siteSettings.Tab.Select("", "TabOrder"))
			{
				TabStripDetails tabDetails = new TabStripDetails();

				tabDetails.TabId = tRow.TabId;
				tabDetails.TabName = tRow.TabName;
				tabDetails.TabOrder = tRow.TabOrder;
				tabDetails.AuthorizedRoles = tRow.AccessRoles;
				tabDetails.ToolTip = tRow.ToolTip;

				this.DesktopTabs.Add(tabDetails);
			}

			// If the PortalSettings.ActiveTab property is set to 0, change it to  
			// the TabID of the first tab in the DesktopTabs collection
			if(this.ActiveTab.TabId == 0)
				this.ActiveTab.TabId = ((TabStripDetails)this.DesktopTabs[0]).TabId;


			// Read the Mobile Tab Information, and sort by Tab Order
			foreach(SiteConfiguration.TabRow mRow in siteSettings.Tab.Select("ShowMobile='true'", "TabOrder"))
			{
				TabStripDetails tabDetails = new TabStripDetails();

				tabDetails.TabId = mRow.TabId;
				tabDetails.TabName = mRow.MobileTabName;
				tabDetails.AuthorizedRoles = mRow.AccessRoles;
				
				this.MobileTabs.Add(tabDetails);
			}

			// Read the Module Information for the current (Active) tab
			SiteConfiguration.TabRow activeTab = siteSettings.Tab.FindByTabId(tabId);

			// Get Modules for this Tab based on the Data Relation
			foreach(SiteConfiguration.ModuleRow moduleRow in activeTab.GetModuleRows())
			{
				ModuleSettings moduleSettings = new ModuleSettings();

				moduleSettings.ModuleTitle = moduleRow.ModuleTitle;
				moduleSettings.ModuleId = moduleRow.ModuleId;
				moduleSettings.ModuleDefId = moduleRow.ModuleDefId;
				moduleSettings.ModuleOrder = moduleRow.ModuleOrder;
				moduleSettings.TabId = tabId;
				moduleSettings.PaneName = moduleRow.PaneName;
				moduleSettings.AuthorizedEditRoles = moduleRow.EditRoles;
				moduleSettings.CacheTime = moduleRow.CacheTimeout;
				moduleSettings.ShowMobile = moduleRow.ShowMobile;

				// ModuleDefinition data
				SiteConfiguration.ModuleDefinitionRow modDefRow = siteSettings.ModuleDefinition.FindByModuleDefId(moduleSettings.ModuleDefId);

				moduleSettings.DesktopSrc = modDefRow.DesktopSourceFile;
				moduleSettings.MobileSrc = modDefRow.MobileSourceFile;
				
				this.ActiveTab.Modules.Add(moduleSettings);
			}

			// Sort the modules in order of ModuleOrder
			this.ActiveTab.Modules.Sort();

			// Get the first row in the Global table
			SiteConfiguration.GlobalRow globalSettings = (SiteConfiguration.GlobalRow) siteSettings.Global.Rows[0];

			// Read Portal global settings 
			this.PortalId = globalSettings.PortalId;
			this.PortalName = globalSettings.PortalName;
			this.AlwaysShowEditButton = globalSettings.AlwaysShowEditButton;
			this.ActiveTab.TabIndex = tabIndex;
			this.ActiveTab.TabId = tabId;
			this.ActiveTab.TabOrder = activeTab.TabOrder;
			this.ActiveTab.MobileTabName = activeTab.MobileTabName;
			this.ActiveTab.AuthorizedRoles = activeTab.AccessRoles;
			this.ActiveTab.TabName = activeTab.TabName;
			this.ActiveTab.ShowMobile = activeTab.ShowMobile;
		}
	}

	//*********************************************************************
	//
	// TabStripDetails Class
	//
	// Class that encapsulates the just the tabstrip details -- TabName, TabId and TabOrder 
	// -- for a specific Tab in the Portal
	//
	//*********************************************************************

	public class TabStripDetails 
	{

		public int          TabId;
		public String       TabName;
		public int          TabOrder;
		public String       AuthorizedRoles;
		public String		ToolTip;
	}

	//*********************************************************************
	//
	// TabSettings Class
	//
	// Class that encapsulates the detailed settings for a specific Tab 
	// in the Portal
	//
	//*********************************************************************

	public class TabSettings 
	{
                           
		public int          TabIndex;
		public int          TabId;
		public String       TabName;
		public int          TabOrder;
		public String       MobileTabName;
		public String       AuthorizedRoles;
		public bool         ShowMobile;
		public ArrayList    Modules = new ArrayList();
	}

	//*********************************************************************
	//
	// ModuleSettings Class
	//
	// Class that encapsulates the detailed settings for a specific Tab 
	// in the Portal.  ModuleSettings implements the IComparable interface 
	// so that an ArrayList of ModuleSettings objects may be sorted by
	// ModuleOrder, using the ArrayList's Sort() method.
	//
	//*********************************************************************

	public class ModuleSettings : IComparable 
	{

		public int          ModuleId;
		public int          ModuleDefId;
		public int          TabId;
		public int          CacheTime;
		public int          ModuleOrder;
		public String       PaneName;
		public String       ModuleTitle;
		public String       AuthorizedEditRoles;
		public bool         ShowMobile;
		public String       DesktopSrc;
		public String       MobileSrc;


		public int CompareTo(object value) 
		{

			if (value == null) return 1;

			int compareOrder = ((ModuleSettings)value).ModuleOrder;
            
			if (this.ModuleOrder == compareOrder) return 0;
			if (this.ModuleOrder < compareOrder) return -1;
			if (this.ModuleOrder > compareOrder) return 1;
			return 0;
		}
	}

	//*********************************************************************
	//
	// ModuleItem Class
	//
	// This class encapsulates the basic attributes of a Module, and is used
	// by the administration pages when manipulating modules.  ModuleItem implements 
	// the IComparable interface so that an ArrayList of ModuleItems may be sorted
	// by ModuleOrder, using the ArrayList's Sort() method.
	//
	//*********************************************************************

	public class ModuleItem : IComparable 
	{

		private int      _moduleOrder;
		private String   _title;
		private String   _pane;
		private int      _id;
		private int      _defId;

		public int ModuleOrder 
		{

			get 
			{
				return _moduleOrder;
			}
			set 
			{
				_moduleOrder = value;
			}
		}    

		public String ModuleTitle 
		{

			get 
			{
				return _title;
			}
			set 
			{
				_title = value;
			}
		}

		public String PaneName 
		{

			get 
			{
				return _pane;
			}
			set 
			{
				_pane = value;
			}
		}
        
		public int ModuleId 
		{

			get 
			{
				return _id;
			}
			set 
			{
				_id = value;
			}
		}  
  
		public int ModuleDefId 
		{

			get 
			{
				return _defId;
			}
			set 
			{
				_defId = value;
			}
		} 
   
		public int CompareTo(object value) 
		{

			if (value == null) return 1;

			int compareOrder = ((ModuleItem)value).ModuleOrder;
            
			if (this.ModuleOrder == compareOrder) return 0;
			if (this.ModuleOrder < compareOrder) return -1;
			if (this.ModuleOrder > compareOrder) return 1;
			return 0;
		}
	}
    
	//*********************************************************************
	//
	// TabItem Class
	//
	// This class encapsulates the basic attributes of a Tab, and is used
	// by the administration pages when manipulating tabs.  TabItem implements 
	// the IComparable interface so that an ArrayList of TabItems may be sorted
	// by TabOrder, using the ArrayList's Sort() method.
	//
	//*********************************************************************

	public class TabItem : IComparable 
	{

		private int      _tabOrder;
		private String   _name;
		private int      _id;

		public int TabOrder 
		{

			get 
			{
				return _tabOrder;
			}
			set 
			{
				_tabOrder = value;
			}
		}    

		public String TabName 
		{

			get 
			{
				return _name;
			}
			set 
			{
				_name = value;
			}
		}

		public int TabId 
		{

			get 
			{
				return _id;
			}
			set 
			{
				_id = value;
			}
		}  
  
		public int CompareTo(object value) 
		{

			if (value == null) return 1;

			int compareOrder = ((TabItem)value).TabOrder;
            
			if (this.TabOrder == compareOrder) return 0;
			if (this.TabOrder < compareOrder) return -1;
			if (this.TabOrder > compareOrder) return 1;
			return 0;
		}
	}
}