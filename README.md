# Embedded Integration Bridge

<div align="center">

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-13.0-239120?style=for-the-badge&logo=csharp&logoColor=white)
![OPC UA](https://img.shields.io/badge/OPC%20UA-Industrial-FF6600?style=for-the-badge)

**High-performance OPC UA client for industrial automation data integration**

[Technologies](#🛠️-technologies) • [Getting Started](#🚀-getting-started)

</div>

---

## Overview

The **Embedded Integration Bridge** is the **industrial communication layer** of the system. Its primary responsibility is to connect **OPC UA servers (PLCs / TIA Portal)** with the **Back-end API**, translating industrial protocol events into HTTP-based updates.

This service isolates industrial complexity from the rest of the stack, enabling a clean and maintainable architecture.

---

### 🏭 System Architecture - Industrial Integration

```
┌──────────────┐     OPC UA     ┌──────────────┐         HTTP         ┌──────────────┐
│  Factory IO  │ ◄────────────► │  TIA Portal  │ ◄──────────────────► │    Bridge    │
│  (3D Sim)    │                │     (PLC)    │                      │  (OPC UA +   │
└──────────────┘                └──────────────┘                      │   HTTP)      │
                                                                      └──────┬───────┘
                                                                             │
                                                                        HTTP │
                                                                             ▼
┌──────────────┐    SignalR     ┌──────────────────────────────────────────────────┐
│    React     │ ◄────────────► │              Backend Server                      │
│   Frontend   │     HTTP       │  ┌─────────┐ ┌─────────┐ ┌──────────────────┐    │
└──────────────┘                │  │   API   │ │ SignalR │ │  Memory Cache    │    │
                                │  │ (REST)  │ │  Hub    │ │  (Repository)    │    │
                                │  └─────────┘ └─────────┘ └──────────────────┘    │
                                └──────────────────────────────────────────────────┘
```

---

## Responsibilities

- Connect to OPC UA servers
- Monitor subscribed nodes
- Detect value changes
- Forward updates to the backend via HTTP
- Handle reconnections and fault tolerance

---

## 🛠️ Technologies

| Technology | Purpose |
|-----------|---------|
| .NET | Runtime |
| OPC UA Client | Industrial protocol |
| HTTP Client | Backend integration |

---

## Data Flow

1. PLC exposes variables via OPC UA
2. Bridge subscribes to configured nodes
3. Value change detected
4. HTTP request sent to backend
5. Backend broadcasts update via SignalR

---

## Configuration

Configured using `appsettings.json`:

```json
{
  "OpcUa": {
    "ServerUrl": "opc.tcp://localhost:4840",
    "SessionTimeout": 60000
  },
  "ApiClient": {
    "BaseUrl": "http://localhost:5000/api"
  }
}
```

---

## 🚀 Getting Started

### Prerequisites

- OPC UA Server (TIA Portal / PLC)
- Backend running

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/igor-fuchs/Embedded-Integration-Bridge.git
   cd Embedded-Integration-Bridge/Bridge
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the solution**
   ```bash
   dotnet build
   ```

4. **Run the worker**
   ```bash
   dotnet run --project src/Worker
   ```

5. **Worker running**
   
   The worker will connect to the OPC UA server and start monitoring nodes.

---

## Role in the Ecosystem

The Bridge is the **only component that understands OPC UA**:

- No OPC UA logic exists in the backend or frontend
- All industrial concerns are encapsulated here

This design guarantees **protocol isolation**, **security**, and **scalability**.

---

## 📄 License

This project is for educational and demonstration purposes.

## 👤 Author

**Igor Fuchs**

- GitHub: [@igor-fuchs](https://github.com/igor-fuchs)
- LinkedIn: [Igor Fuchs Pereira](www.linkedin.com/in/igor-fuchs-pereira)

---

<div align="center">

**Part of the Embedded Integration project ecosystem**

[Backend Repository](https://github.com/igor-fuchs/Embedded-Integration-Backend) • [Bridge Repository](https://github.com/igor-fuchs/Embedded-Integration-Bridge)

</div>

