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
                        if (accountEntity.Attributes.Contains("ds_GroupCode"))
                        {
                            string fetchXml = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>"+
  "<entity name='contact'>"+
    "< attribute name='contactid' />"+
    "< order attribute='fullname' descending='false' />"+
    "< link-entity name='account' from='accountid' to='parentcustomerid' link-type='inner' alias='ac'>"+
      "< filter type='and'>"+
        "< condition attribute='accountid' operator='eq' uiname='Paracherry' uitype='account' value='{'" + accountEntity + "'}' />"+
      "</filter>" +
    "</link-entity>" +
  "</entity>"+
"</fetch>";
                            EntityCollection ecContacts = service.RetrieveMultiple(new FetchExpression(fetchXml));
                        }
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
