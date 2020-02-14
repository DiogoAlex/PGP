using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace UpdateRecord
{
    public class Class1 : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Obtain the execution context from the service provider.  
            IPluginExecutionContext context = (IPluginExecutionContext)
                serviceProvider.GetService(typeof(IPluginExecutionContext));

            // The InputParameters collection contains all the data passed in the message request.  
            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {

                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    if (context.Depth <= 1)
                    {
                        // Plug-in business logic goes here.  
                        Entity entity = (Entity)context.InputParameters["Target"];

                        if (entity.LogicalName == "new_test")
                        {
                            tracingService.Trace("Will update test record now");
                            //setting string
                            entity["new_string1"] = "testplugin";

                            //setting lookup
                            entity["new_lookup1"] = new EntityReference("account", new Guid("997380d6-4420-ea11-a812-000d3a86bcc1"));

                            //setting only the date of right now, excluding time
                            entity["new_onlydate1"] = DateTime.Now.Date;

                            //Setting money
                            entity["new_currency1"] = new Money(new decimal(99.99));

                            //setting option set
                            entity["new_optionset1"] = new OptionSetValue(100000000);

                            service.Update(entity);
                        }
                    }
                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException("An error occurred in FollowUpPlugin.", ex);
                }

                catch (Exception ex)
                {
                    tracingService.Trace("FollowUpPlugin: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}
