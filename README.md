# Fyora Sprint 3

Fyora Sprint 3 é uma aplicação **.NET 8 (C#)** em modo console para **gestão de usuários e logs de progresso**.
O projeto utiliza **Entity Framework Core (SQLite)** para persistência, possui **validações de duplicidade (nickname/email)** e recursos de **importação/exportação em JSON e TXT**.
A interface em **Console UI** foi aprimorada para fornecer menus claros, cores e feedback visual.

---

## Integrantes
### GUILHERME ROCHA BIANCHINI - RM97974
### NIKOLAS RODRIGUES MOURA DOS SANTOS - RM551566
### PEDRO HENRIQUE PEDROSA TAVARES - RM97877
### RODRIGO BRASILEIRO - RM98952
### THIAGO JARDIM DE OLIVEIRA - RM551624

---

## 📂 Estrutura do Projeto

```
Fyora-sprint3/
├── Program.cs             # Console UI (menus, navegação)
├── Data/
│   └── FyoraContext.cs    # DbContext (SQLite, EF Core)
├── Models/
│   ├── User.cs            # Entidade User (Nickname, Email, CreatedAt)
│   └── ProgressLog.cs     # Entidade ProgressLog (dias sem jogar, conquistas)
├── Repositories/
│   └── UserRepository.cs  # CRUD + validação de duplicidade
├── Services/
│   └── FileService.cs     # Import/Export JSON e TXT na raiz do projeto
├── Diagrams/              # Diagramas UML / C4 / ERD (SVG)
├── fyora_admin.db         # Banco SQLite (gerado automaticamente)
├── fyora_import.json      # Arquivo de importação (colocar na raiz)
├── fyora_export/          # Exportações JSON geradas pelo sistema
└── fyora_summary/         # Relatórios TXT gerados pelo sistema
```

---

## ⚙️ Tecnologias Utilizadas

* **.NET 8 / C#**
* **Entity Framework Core 8**
* **SQLite**
* **Console UI (interativo e colorido)**
* **JSON / TXT I/O**

---

## 📊 Diagramas (em `/Diagrams`)

> Os diagramas abaixo são renderizados a partir dos arquivos **SVG** da pasta `Diagrams/`.  
> Se a sua plataforma não lidar bem com nomes contendo espaços/acentos, renomeie os arquivos ou ajuste os paths.

### 1) C4 – System Context (Nível 1)
![C4 – System Context (Nível 1)](Diagrams/C4%20%E2%80%93%20System%20Context%20(N%C3%ADvel%201).svg "C4 – System Context (Nível 1)")

### 2) C4 – Containers/Componentes (Nível 2)
![C4 – Containers/Componentes (Nível 2)](Diagrams/C4%20%E2%80%93%20Containers_Componentes%20(N%C3%ADvel%202).drawio.svg "C4 – Containers/Componentes (Nível 2)")

### 3) ERD – Modelo de Dados (SQLite)
![ERD – Modelo de Dados (SQLite)](Diagrams/ERD%20%E2%80%93%20Modelo%20de%20Dados%20%28SQLite%29.svg "ERD – Modelo de Dados (SQLite)")

### 4) UML Sequence – Fluxo “Importar Usuários”
![UML Sequence – Fluxo “Importar Usuários”](Diagrams/UML%20Sequence%20%E2%80%93%20Fluxo%20%E2%80%9CImportar%20Usu%C3%A1rios%E2%80%9D.svg "UML Sequence – Fluxo “Importar Usuários”")

### 5) UML Deployment – Execução local
![UML Deployment – Execução local](Diagrams/UML%20Deployment%20%E2%80%93%20Execu%C3%A7%C3%A3o%20local.svg "UML Deployment – Execução local")

---

## 🚀 Funcionalidades

* **Gerenciamento de Usuários**
  * Adicionar usuários (com verificação de duplicidade de nickname/email)
  * Listar todos os usuários cadastrados
  * Ver detalhes de um usuário específico (incluindo histórico de progresso)
  * Atualizar nickname/email de usuários existentes
  * Excluir usuários (remoção em cascata dos logs)

* **Gerenciamento de Logs de Progresso**
  * Registrar quantidade de dias sem jogar
  * Anotar conquistas alcançadas
  * Associar cada log diretamente ao usuário escolhido

* **Importação / Exportação**
  * Exportar todos os usuários → `fyora_export/fyora_export.json`
  * Importar usuários de `fyora_import.json` (na raiz do projeto)
    * Insere apenas novos usuários, sem duplicar existentes
  * Gerar relatório TXT (resumo) → `fyora_summary/fyora_summary.txt`

---

## 📥 Pré-requisitos

* [Instalar .NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
* Opcional: [Instalar SQLite CLI](https://www.sqlite.org/download.html) para inspecionar o banco
* Editor recomendado: **Visual Studio** ou **VS Code**

---

## ▶️ Como Rodar o Projeto

1. **Clone o repositório:**
   ```bash
   git clone https://github.com/seuusuario/Fyora-sprint3.git
   cd Fyora-sprint3
   ```

2. **Restaure dependências:**
   ```bash
   dotnet restore
   ```

3. **Compile e rode a aplicação:**
   ```bash
   dotnet run
   ```

4. **Primeira execução:**
   * Será criado o arquivo `fyora_admin.db` (SQLite).
   * Serão criadas automaticamente as tabelas **Users** e **ProgressLogs**.

5. **Executando menus no Console:**
   * Navegue digitando o número correspondente à opção desejada.
   * Use ENTER para confirmar.
   * O sistema exibirá mensagens de sucesso (✔) ou avisos (⚠).

---

## 📋 Menu Principal

```
┌────────────────────────────────────────────┐
│ Fyora Admin Console                        │
└────────────────────────────────────────────┘
[1] Gerenciar Usuários
[2] Gerenciar Logs de Progresso
[3] Importar/Exportar Dados
[0] Sair
```

---

## 👤 Gerenciamento de Usuários

Menu de operações CRUD sobre usuários.
Cada usuário é identificado por **Nickname** e **Email** (ambos únicos e obrigatórios).

### Opções:

1. **Adicionar Usuário**
   * Solicita **Nickname** e **Email**.
   * Bloqueia duplicidade (mesmo que seja apenas diferença de maiúsculas/minúsculas).

2. **Listar todos os Usuários**
   * Exibe todos os registros com ID, Nickname e Email.

3. **Ver detalhes de um Usuário**
   * Solicita o **ID**.
   * Mostra Email e todos os logs de progresso vinculados.

4. **Atualizar Usuário**
   * Solicita o **ID**.
   * Permite alterar o **Nickname** e opcionalmente o **Email**.
   * Valida duplicidade antes de confirmar.

5. **Deletar Usuário**
   * Solicita o **ID**.
   * Remove o usuário e todos os seus logs (remoção em cascata).

---

## 📈 Gerenciamento de Logs de Progresso

Permite registrar atividades de cada usuário.

### Fluxo:

1. Solicita o **ID do Usuário**.
   * Se o usuário não existir, exibe aviso ⚠.
2. Solicita **dias sem jogar** (número inteiro ≥ 0).
3. Solicita uma **conquista** (exemplo: *"Completou desafio semanal"*).
4. Registra o log, associado ao usuário, com a data/hora do sistema.

---

## 📂 Importar / Exportar Dados

### 1. Exportar para JSON
* Exporta todos os usuários + logs para:
  ```
  fyora_export/fyora_export.json
  ```

### 2. Importar de JSON
* Lê dados do arquivo:
  ```
  fyora_import.json
  ```
* Arquivo deve estar na **raiz do projeto**.
* Estrutura esperada:
```json
[
  {
    "Nickname": "Alice",
    "Email": "alice@example.com",
    "CreatedAt": "2025-09-15T12:30:00",
    "ProgressLogs": [
      {
        "DaysWithoutGambling": 10,
        "Achievement": "Completou desafio semanal",
        "LogDate": "2025-09-15T12:30:00"
      }
    ]
  }
]
```
⚠️ Regras durante o import:
* Se Nickname ou Email já existirem, o usuário é ignorado.
* Se `CreatedAt` estiver vazio ou `default`, será preenchido com `DateTime.Now`.

### 3. Gerar Relatório TXT
* Cria um resumo com contagens:
```
--- Relatório Resumo Fyora Admin ---
Data do Relatório: 16/09/2025 15:20:00
Total de Usuários no Sistema: 5
Total de Logs de Progresso Registrados: 12
```
Salvo em:
```
fyora_summary/fyora_summary.txt
```

---

## 🔒 Regras de Validação

* **Nickname**
  * Obrigatório
  * Único
  * Até 120 caracteres
* **Email**
  * Obrigatório
  * Único
  * Até 200 caracteres
  * Formato válido de email
* **CreatedAt**
  * Obrigatório
  * Preenchido automaticamente no código ou com `CURRENT_TIMESTAMP` no banco
* **ProgressLog**
  * Dias sem jogar ≥ 0
  * Achievement opcional (até 300 caracteres)
  * LogDate preenchido automaticamente

---

## 🛠️ Desenvolvimento e Banco

* O banco é criado via `EnsureCreated()` quando a aplicação roda pela primeira vez.
* Para evoluir schema em ambientes reais, use **Migrations EF Core**:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```
* Para resetar completamente:
  * Apague `fyora_admin.db`
  * Rode `dotnet run` novamente (novo schema será criado)

---

## 📌 Roadmap Futuro

* Autenticação de administrador
* Testes unitários para UserRepository/FileService
* Exportação em CSV
* Interface Web (Blazor / ASP.NET Core MVC)

---

## 👨‍💻 Autor

* Projeto desenvolvido por **TPGN - TechPulse Global Network**
* Licença: **MIT**
