using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace D365Plugin
{
    public class AccountUpdate : IPlugin
    {
        IOrganizationService service;
        IExecutionContext context;
        ITracingService trace;
        Entity currentEntity;

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {
                context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                service = ((IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory))).CreateOrganizationService(context.UserId);
                trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

                if(context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity accountEntity = (Entity)context.InputParameters["Target"];
                    if(accountEntity.LogicalName.Equals("account"))
                    {
                        if(accountEntity.Attributes.Contains(""))
                    }
                };
            }
            catch (InvalidPluginExecutionException IPE)
            {
                trace.Trace(IPE.Message.ToString());
            }
        }
    }
}
