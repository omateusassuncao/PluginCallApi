var Challenge = window.Challenge || {};

(function () {

    var currentUserName = Xrm.Utility.getGlobalContext().userSettings.userName; // get current user name
    var messageError = currentUserName + ", seu envio falhou.";


    this.formOnChange = async function (executionContext) {

        try {

            var formContext = executionContext.getFormContext();;

            if(formContext.getAttribute("mat_inventario").getValue() != null){    

                var mat_inventario_ref = formContext.getAttribute("mat_inventario").getValue()[0].id;
                Xrm.WebApi.retrieveRecord("mat_inventario", mat_inventario_ref, "?$select=mat_id,mat_quantidae,mat_Item&$expand=mat_Item($select=mat_itemid,mat_name)").then(
                    function success(result) {

                        if(result.mat_quantidae == 0)
                        {
                            Xrm.Navigation.openAlertDialog("Esse inventário está com quantidade zerada.");   
                            formContext.getAttribute("mat_inventario").setValue(null);
                        }
                        else
                        {
                            var toLookup = new Array();
                            toLookup[0] = new Object();
                            toLookup[0].id = result.mat_Item.mat_itemid;
                            toLookup[0].entityType = "mat_item";
                            toLookup[0].name = result.mat_Item.mat_name;
    
                            formContext.getAttribute("mat_item").setValue(toLookup);
                        }
                    },
                    function (error) {
                        console.log(error.message);
                    }
                );
            }
            else{
                formContext.getAttribute("mat_item").setValue(null);
            }

        }
        catch (e) {
            console.error("inner", e.message);
            throw e;
        }
        finally {
            console.log("finally");
        }
    }

}).call(Challenge);