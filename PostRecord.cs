using System;
using System.Net.Http;
using System.Text;
using Microsoft.Xrm.Sdk;
using System.Net.Http.Headers;
using PluginCallApi.Model;
using System.Text.Json;
using Microsoft.Xrm.Sdk.Query;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace PluginCallApi
{

    public class PostRecord : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            // Obtain the tracing service
            ITracingService tracingService =
            (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // getting the pipeline context.
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory serviceFactory =
            (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // The InputParameters collection contains all the data passed in the message request.
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    Entity entity = context.InputParameters["Target"] as Entity;
                    Entity vendasEntity = service.Retrieve("mat_vendas", entity.Id, new ColumnSet(new string[] { "mat_id", "statuscode", "mat_item" }));

                    Object itemObj;
                    EntityReference item;
                    vendasEntity.Attributes.TryGetValue("mat_item", out itemObj);

                    item = (EntityReference)itemObj;
                    Entity itemEntity = service.Retrieve("mat_item", item.Id, new ColumnSet(new string[] { "mat_name", "mat_descricao", "mat_preco" }));

                    //Data
                    string vendasId = vendasEntity.Attributes["mat_id"].ToString();
                    string status = string.Empty;
                    switch (vendasEntity.GetAttributeValue<OptionSetValue>("statuscode").Value)
                    {
                        case 1:
                            status = "Active";
                            return;
                        case 860960001:
                            status = "Aguardando Aprovação";
                            break;
                        case 860960002:
                            status = "Venda Aprovada";
                            return;
                        case 860960003:
                            status = "Venda Recusada";
                            return;
                        default:
                            return;
                    }
                    string nome = itemEntity.GetAttributeValue<string>("mat_name");
                    string descricao = itemEntity.GetAttributeValue<string>("mat_descricao");
                    string preco = itemEntity.GetAttributeValue<Money>("mat_preco").Value.ToString();

                    Record record = new Record(vendasId, nome, descricao, preco, status);
                    tracingService.Trace($"Record: {vendasId}, {nome}, {descricao}, {preco}, {status}");

                    //Serialize Data
                    var json = JsonSerializer.Serialize(record);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                //Call API
                    string URL = "https://challenge04vendas.azurewebsites.net/api/VendasHttpTrigger";
                    string apikey = "-OuPMMBqIAwnQAGpnCPm3jCPXOm6l3nAwtRdX49xIp87AzFuecdgEA==";
                    string urlParameters = $"?code={apikey}";

                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(URL);

                    // Add an Accept header for JSON format.
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    // List data response.
                    HttpResponseMessage response = client.PostAsync(URL + urlParameters, content).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content.ReadAsStringAsync().Result;

                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        };

                        tracingService.Trace(responseContent.ToString());

                    }
                    else
                    {
                        tracingService.Trace("Error: " + response.StatusCode);
                    }
                }
            }
            catch (InvalidPluginExecutionException ex)
            {
                tracingService.Trace("Error: " + ex.ToString());
            }

        }
    }
}