using System;
using System.Configuration;

namespace ZWT.DbLib
{
	/// <summary>
	/// Summary description for PortalDALFactory.
	/// </summary>
	public abstract class DataLibFactory
	{
		public static Type dalType; 
		public static IDataLib CreateDAL()
		{
			//Type type = Type.GetType(ConfigurationSettings.AppSettings["DALType"]);
			//string assemblyName = ConfigurationSettings.AppSettings["DALAssemblyName"];
			if (dalType == null)
			{
                //string typeName = ConfigurationSettings.AppSettings["DataLibType"];
                //dalType = Type.GetType(typeName);
                string typeName = "ZWT.DbLib.SqlDataLib";
                dalType = Type.GetType(typeName);
			}
			return (IDataLib)Activator.CreateInstance(dalType);
		}//CreateDAL
	}//PortalDALFactory
}//namespace
