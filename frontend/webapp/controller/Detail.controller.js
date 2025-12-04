sap.ui.define([
  "sap/ui/core/mvc/Controller",
  "sap/ui/model/json/JSONModel",
  "sap/m/MessageToast"
], function(Controller, JSONModel, MessageToast) {
  return Controller.extend("myapp.controller.Detail", {
    onInit: function() {
      this.getOwnerComponent().getRouter().getRoute("detail").attachPatternMatched(this._onRouteMatched, this);
      this._model = new JSONModel({ todo: {} });
      this.getView().setModel(this._model);
    },
    async _onRouteMatched(oEvt) {
      const id = oEvt.getParameter("arguments").id;
      try {
        const resp = await fetch(`/api/todos/${id}`);
        if (!resp.ok) {
          MessageToast.show("Não encontrado");
          return;
        }
        const json = await resp.json();
        this._model.setProperty("/todo", json);
      } catch (e) {
        MessageToast.show("Erro ao carregar detalhes");
      }
    }
  });
});