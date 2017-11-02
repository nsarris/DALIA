using System;
using System.Linq;
using System.Data.Common;
using Dalia;
using Dalia.DI;
using Dalia.DI.Unity;
using Dalia.Linq2db;

namespace Dalia.UI.Unity
{
    public class UnityLinqToDBResolverRegistrator : IUnityDaliaResolverRegistrator
    {
        public void RegisterTypes(UnityDaliaResolverBoostraper boostraper)
        {
            var contextTypes = AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic)
                 .SelectMany(a => a.GetTypes().Where(x => !x.IsAbstract && x.IsClass && x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ILinqToDBDataContext<>))))
                 .ToList();

            foreach (var contextType in contextTypes)
            {
                Type dataConnectionType = contextType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ILinqToDBDataContext<>)).FirstOrDefault().GetGenericArguments()[0];

                boostraper.RegisterDaliaType(dataConnectionType, new[] { typeof(IDataSource), typeof(DbConnection) }, (c, ds, registrationName) =>
                {
                    var con = c.Resolve(ds.DbConnectionType, registrationName);
                    return Activator.CreateInstance(dataConnectionType, new object[] { ds, con });
                });

                boostraper.RegisterDaliaType(contextType, new[] { dataConnectionType }, (c, ds, registrationName) =>
                {
                    var dc = c.Resolve(dataConnectionType, registrationName);
                    return Activator.CreateInstance(contextType, new object[] { dc });
                });
            }
        }
    }





}

