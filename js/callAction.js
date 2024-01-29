var Challenge = window.Challenge || {};
(function () {
    // Define some global variables
    var myUniqueId = "_myUniqueId"; // Define an ID for the notification
    var currentUserName = Xrm.Utility.getGlobalContext().userSettings.userName; // get current user name
    var message = currentUserName + ", a venda foi enviada ao cliente. Aguarde a aprovação.";
    var messageError = currentUserName + ", seu envio falhou.";

    // const { ServiceBusClient } = require("@azure/service-bus");
    // const connectionString = "Endpoint=sb://challenge04.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=6tEd2bAHcel2jv7wY/hI482cJ2bhohPAH+ASbDKANxU="
    // const queueName = "vendas"

    // create a Service Bus client using the connection string to the Service Bus namespace
    // const sbClient = new ServiceBusClient(connectionString);
    // createSender() can also be used to create a sender for a topic.
    // const sender = sbClient.createSender(queueName);
    // Code to run in the form OnSave event 
    // this.formOnSave = async function (executionContext) {

    this.formOnSave = function (executionContext) {

        try {

            var formContext = executionContext;
            formContext.getAttribute("statuscode").setValue(860960001);
            formContext.data.entity.save({ saveMode: 1 });

            // try {
            //     let batch = await sender.createMessageBatch();
            //     batch.tryAddMessage({ body: "Albert Einstein" });
            //     await sender.sendMessages(batch);
            //     console.log(`Sent a batch of messages to the queue: ${queueName}`);
            //     await sender.close();
            // }
            // finally {
            //     await sbClient.close();
            //     formContext.getAttribute("statuscode").setValue(860960001);
            //     formContext.data.entity.save({ saveMode: 1 });
            // }

        }
        catch (e) {
            console.error("inner", e.message);
            Xrm.Navigation.openAlertDialog(messageError + " (Erro: " + e.message + ")");
            throw e;
        }
        finally {
            Xrm.Navigation.openAlertDialog(message);
            console.log("finally");
        }

    }


}).call(Challenge);