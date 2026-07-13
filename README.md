# AI Code Assistant

An intelligent AI-powered assistant designed to help developers explain, fix, and optimize their code. This project demonstrates the integration of Large Language Models (LLMs) into a developer workflow via a clean, decoupled architecture.

## 🚀 Features
- **Code Analysis**: Get explanations for complex code snippets.
- **Bug Fixing**: Automatically identify and fix bugs in your code.
- **Performance Optimization**: Receive suggestions to make your code more efficient.
- **Provider Agnostic**: Supports Ollama (local) and any OpenAI-compatible API endpoints.
- **Query Logging**: Full persistence of every input and output for audit and quality analysis.

## 🏗️ Architecture

### Backend (.NET 10.0 Web API)
The backend is built with a service-oriented architecture:
- **AIController**: Handles HTTP requests, input validation, and response formatting.
- **AIService**: Orchestrates the communication with AI providers (Ollama/OpenAI).
- **QueryLogService**: Manages the persistence of AI interactions in a SQL database.
- **AppDbContext**: Entity Framework Core context for SQLite persistence.

### Frontend (React)
- A modern user interface providing a seamless experience for interacting with the AI.

### Data Flow
`User Request` $\rightarrow$ `AIController` $\rightarrow$ `AIService` $\rightarrow$ `AI Provider` $\rightarrow$ `AIService` $\rightarrow$ `QueryLogService (Save to DB)` $\rightarrow$ `User Response`

## 🛠️ Technical Stack
- **Language**: C# (.NET 10.0)
- **Database**: SQLite (via EF Core)
- **AI Integration**: Ollama API, OpenAI-compatible APIs
- **Frontend**: React.js

## 💎 Value Proposition & Engineering Decisions

### 1. Observability & Quality Assurance
By implementing the `QueryLogs` system, the application moves from a stateless tool to a data-driven system. This allows for:
- **Error Analysis**: Tracking where the AI fails to provide helpful responses.
- **Model Benchmarking**: Comparing different LLMs (e.g., Llama 3 vs. GPT-4) based on real-world user data.
- **Feedback Loop**: Creating a dataset for future fine-tuning (RLHF).

### 2. Security-First Design
Sensitive information, such as API keys used for custom endpoints, is handled in memory and **never persisted** to the database, ensuring that the log remains a safe audit trail.

### 3. Portability
The use of SQLite ensures that the entire project is "plug-and-play," requiring no external database installation for reviewers or interviewers.

## 🗺️ Technical Roadmap
- [x] Core AI Integration (Ollama & Custom Endpoints)
- [x] SQL Persistence for Query Logging
- [ ] **Caching Layer**: Implement Redis or in-memory caching to avoid redundant AI calls for identical requests.
- [ ] **History Dashboard**: A frontend view to browse and analyze past queries.
- [ ] **Semantic Search**: Integrating a vector database (e.g., pgvector) to find similar past issues and their resolutions.
- [ ] **User Authentication**: Adding multi-user support with personalized history.

## 🏁 Getting Started

### Prerequisites
- Node.js and npm (for the frontend)
- .NET SDK (for the backend)
- Ollama (for AI processing)

### Quick Start
1. **Start the backend** (from the `backend` folder):
   ```bash
   dotnet run
   ```
2. **Start the frontend** (from the `frontend` folder):
   ```bash
   npm install
   npm run dev
   ```
3. Open your browser to `http://localhost:3000`
