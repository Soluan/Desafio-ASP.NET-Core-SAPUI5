# 📝Fullstack Project (ASP.NET Core & SAPUI5)

Este repositório contém uma aplicação Fullstack de gerenciamento de tarefas, utilizando **ASP.NET Core Web API** para o backend e **SAPUI5/OpenUI5** para o frontend.

---

## 🏗️ Estrutura do Projeto

A solução está organizada em uma estrutura limpa de mono-repositório:

| Diretório | Conteúdo | Tecnologia |
| :--- | :--- | :--- |
| **`backend/`** | O projeto principal da Web API e a lógica de serviço. | ASP.NET Core WebAPI |
| **`backend.Tests/`** | Testes de unidade e integração para o backend. | xUnit |
| **`frontend/`** | O código-fonte da aplicação SAPUI5, incluindo `Views`, `Controllers` e `Component.js`. | SAPUI5 / OpenUI5 |

---

## 🛠️ Pré-requisitos

Para rodar o projeto localmente, você precisará de:

* **Backend:** [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0) (ou superior).
* **Frontend:** [Node.js](https://nodejs.org/) e **npm** (para o `ui5-cli` e o middleware de proxy).

---

## ▶️ Como Rodar (Setup Local)

O projeto utiliza o **`ui5 serve`** com um proxy configurado para comunicação entre o frontend (Porta 8080) e o backend (Porta 5000).

### 1. ⚙️ Iniciar o Backend (API)

O backend deve ser iniciado primeiro. Ele usará o IP de *loopback* (`127.0.0.1`) para garantir a estabilidade da comunicação via proxy.

1.  Navegue até o diretório do backend:
    ```bash
    cd backend
    ```

2.  Restaure e execute o projeto, forçando o endereço que o proxy usará:
    ```bash
    dotnet restore
    dotnet run --urls "[http://127.0.0.1:5000](http://127.0.0.1:5000)"
    ```
    * **Observação:** A API estará disponível em **`http://127.0.0.1:5000`**. O **CORS (AllowAll)** está ativado para facilitar o desenvolvimento.

### 2. 🖥️ Iniciar o Frontend (SAPUI5 Dev Server)

O servidor de desenvolvimento do UI5 irá carregar a aplicação e configurar o proxy.

1.  Navegue até o diretório do frontend:
    ```bash
    cd ../frontend
    ```

2.  Instale as dependências do Node.js (incluindo o middleware de proxy):
    ```bash
    npm install
    ```

3.  Inicie o servidor de desenvolvimento do UI5:
    ```bash
    ui5 serve -o webapp/index.html
    ```
    * O aplicativo será aberto no seu navegador, geralmente em **`http://localhost:8080`**.
    * O **`ui5.yaml`** está configurado para rotear todas as chamadas para `/users` para o backend na porta 5000.

### 3. ✅ Rodar os Testes

Para garantir a integridade do backend, execute os testes:

1.  Navegue até a pasta de testes:
    ```bash
    cd backend.Tests
    ```
2.  Execute os testes:
    ```bash
    dotnet test
    ```

---

## 🔗 Configuração de Comunicação

| Componente | Endereço de Execução | Rota no Código (Front) | Rota no Servidor (Back) |
| :--- | :--- | :--- | :--- |
| **Frontend UI5** | `http://localhost:8080` | `this._apiBase: "/users"` | N/A |
| **Backend API** | `http://127.0.0.1:5000` | N/A | `[Route("users")]` |
| **Proxy (`ui5.yaml`)** | N/A | N/A | `http://localhost:8080/users` → `http://127.0.0.1:5000/users` |
