
# 💸 SignalR Backend - Transações Bancárias com .NET

Este projeto é um backend desenvolvido em **.NET 8** com **SignalR**, voltado para simular **operações bancárias em tempo real** como depósitos e pagamentos**.

---

## 📦 Tecnologias Utilizadas

- ✅ .NET 8
- ✅ SignalR (real-time communication)
- ✅ MongoDB (armazenamento de contas e transações)
- ✅ DDD (Domain-Driven Design)
- ✅ C#
- ✅ WebSocket

---

## 📁 Estrutura do Projeto

```
signalr-backend/
│
├── Domain/                # Camada de domínio (entidades, regras de negócio)
│   └── Entities/
│       ├── BankAccount.cs
│       └── Transaction.cs
│
│
├── Infrastructure/        # Camada de infraestrutura (MongoDB)
│   └── Data/
│       ├── MongoDbAccountService.cs
│       └── MongoDbTransactionService.cs
│
├── Web/                   # Camada de apresentação (SignalR Hub)
│   └── Hubs/
│       └── TransactionWebSocket.cs
│
├── appsettings.json
├── Program.cs
└── README.md
```

---

## 🚀 Como Executar

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- MongoDB local ou em nuvem
- Configurar a variável `ConnectionStrings:MongoDB` no `appsettings.json`
- Iniciar o front-end na porta 3000 [signalr-frontend](https://github.com/pedrinhoas7/signalr-frontend)

### Passos

```bash
# Restaurar dependências
dotnet restore

# Rodar a aplicação
dotnet run
```

---

## 🧪 Funcionalidades

- [x] Obtenção de dados bancários em tempo real
- [x] Depósitos e pagamentos via WebSocket
- [x] Histórico de transações
- [x] Banco de dados MongoDB

---

## 🔄 Comunicação via SignalR

### Endpoint
```
/ws/transactions?document={cpf}&email={email}
```

### Métodos do Hub
- `GetAccountDetails()` → Retorna saldo e extrato
- `ProcessTransaction(tipo, valor, descricao)` → Executa depósito ou pagamento

### Eventos Recebidos pelo Cliente
- `ReceiveAccountDetails(saldo, extrato[])`
- `Error(mensagem)`

---

## 🧱 Padrões Adotados

- **DDD**: Domínio separado da lógica de infraestrutura
- **Camadas bem definidas**: Domain, Application, Infrastructure e Web
- **MongoDB Repository Pattern**: acesso desacoplado ao banco

---

## 👨‍💻 Autor

Desenvolvido com 💙 por Pedro Henrique
