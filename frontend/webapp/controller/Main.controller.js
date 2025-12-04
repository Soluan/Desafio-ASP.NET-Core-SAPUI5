sap.ui.define([
    "sap/ui/core/mvc/Controller",
    "sap/ui/model/json/JSONModel",
    "sap/m/MessageToast"
], function(Controller, JSONModel, MessageToast) {
    return Controller.extend("myapp.controller.Main", {
        onInit: function() {
            this._model = new JSONModel({
                items: [],
                page: 1,
                pageSize: 10,
                total: 0,
                busy: false,
                titleFilter: ""
            });
            this.getView().setModel(this._model);
            this._debounceTimer = null;
            this.loadTodos();
        },
        // Rota base unificada para o back-end
        _apiBase: "/users",

        setBusy: function(v) { this._model.setProperty("/busy", v); },

        async loadTodos() {
            const page = this._model.getProperty("/page");
            const pageSize = this._model._model.getProperty("/pageSize");
            const title = this._model.getProperty("/titleFilter");
            this.setBusy(true);
            const params = new URLSearchParams();
            params.append("page", page);
            params.append("pageSize", pageSize);
            if (title) params.append("title", title);

            // GET /users?...
            const resp = await fetch(`${this._apiBase}?${params.toString()}`);

            if (!resp.ok) { MessageToast.show("Erro ao carregar tarefas"); this.setBusy(false); return; }
            const data = await resp.json();
            this._model.setProperty("/items", data.items);
            this._model.setProperty("/total", data.total);
            this.getView().byId("pageInfo").setText(`Page ${data.page} / total ${data.total}`);
            this.setBusy(false);
        },

        onSearch: function(oEvt) {
            const val = oEvt.getParameter("query");
            this._model.setProperty("/titleFilter", val);
            this._model.setProperty("/page", 1);
            this.loadTodos();
        },

        onLiveChange: function(oEvt) {
            const val = oEvt.getParameter("value");
            this._model.setProperty("/titleFilter", val);
            this._model.setProperty("/page", 1);
            clearTimeout(this._debounceTimer);
            this._debounceTimer = setTimeout(() => this.loadTodos(), 400);
        },

        onPrevPage: function() {
            let p = this._model.getProperty("/page");
            if (p > 1) {
                this._model.setProperty("/page", p - 1);
                this.loadTodos();
            }
        },

        onNextPage: function() {
            let p = this._model.getProperty("/page");
            this._model.setProperty("/page", p + 1);
            this.loadTodos();
        },

        onDetails: function(oEvt) {
            const ctx = oEvt.getSource().getParent().getBindingContext();
            const id = ctx.getProperty("id");
            this.getOwnerComponent().getRouter().navTo("detail", { id });
        },

        async onToggleCompleted(oEvt) {
            const checkbox = oEvt.getSource();
            const ctx = checkbox.getBindingContext();
            const id = ctx.getProperty("id");
            const newVal = checkbox.getProperty("selected");

            // PUT /users/{id}
            const resp = await fetch(`${this._apiBase}/${id}`, {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Completed: newVal })
            });

            if (resp.status === 400) {
                const err = await resp.json();
                MessageToast.show(err.message || "Regra de negócio violada");
                checkbox.setSelected(!newVal);
            } else if (!resp.ok) {
                MessageToast.show("Erro ao atualizar");
                checkbox.setSelected(!newVal);
            } else {
                MessageToast.show("Atualizado");
                this.loadTodos();
            }
        },

        async onSync() {
            this.setBusy(true);

            // POST /users/sync
            const resp = await fetch(`${this._apiBase}/sync`, { method: "POST" });

            if (!resp.ok) { MessageToast.show("Erro no sync"); } else {
                const json = await resp.json();
                MessageToast.show(`Sync: added ${json.added} skipped ${json.skipped}`);
                this.loadTodos();
            }
            this.setBusy(false);
        }
    });
});