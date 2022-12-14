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
                    Guid accountID;
                    Entity accountEntity = (Entity)context.InputParameters["Target"];
                                        
                    if(accountEntity.LogicalName.Equals("account") && accountEntity != null)
                    {
                        accountID = accountEntity.GetAttributeValue<Guid>("accountid");
                        string groupCode = accountEntity.GetAttributeValue<string>("ds_groupcode");
                        if (accountEntity.Attributes.Contains("ds_groupcode"))
                        {
                            string fetchXml = "<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
  "<entity name='contact'>" +
    "<attribute name='fullname' />" +
    "<attribute name='contactid' />" +
    "<order attribute='fullname' descending='false' />" +
    "<filter type='and'>" +
      "<condition attribute='parentcustomerid' operator='eq' value='{" + accountID + "}' />" +
    "</filter>" +
  "</entity>" +
"</fetch>";

                                                     FetchExpression fe = new FetchExpression(fetchXml);
                            EntityCollection ecContacts = service.RetrieveMultiple(fe);
                            int count = ecContacts.Entities.Count;
                            foreach(var contact in ecContacts.Entities)
                            {
                                contact["ds_groupcode"] = groupCode;
                                service.Update(contact);
                            }
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
