# **DevXpert Academy - Plataforma de Educação Online em Microsserviços**

## **1. Apresentação**

Bem-vindo ao repositório do projeto **DevXpert Academy**.  
Este projeto é uma entrega do MBA DevXpert Full Stack .NET e está sendo evoluído para uma arquitetura baseada em **Microsserviços** com um **Front-end em Angular**.  

O objetivo principal é desenvolver uma plataforma de educação online que permita a gestão de alunos, cursos, matrículas e pagamentos, oferecendo:  
- **APIs RESTful** segmentadas por contexto de negócio;  
- **Autenticação centralizada** via IdentityServer;  
- **Front-end em Angular** como aplicação cliente;  
- Comunicação assíncrona entre microsserviços via eventos.  

### **Autor**
- **Pedro Otávio Gutierres**

## **2. Proposta do Projeto**

O projeto consiste em:

- **Microsserviços RESTful:** APIs independentes responsáveis por diferentes domínios (Alunos, Cursos, Matrículas, Pagamentos, Autenticação).  
- **API BFF (Backend For Frontend):** Camada intermediária para orquestrar chamadas entre o front-end Angular e os microsserviços.  
- **Autenticação e Autorização:** IdentityServer com suporte a JWT e perfis de usuário (Administrador e Aluno).  
- **Modelagem de Domínios Ricos:** Uso de DDD, CQRS, TDD e outros padrões arquiteturais.  
- **Mensageria:** Comunicação entre serviços via eventos, garantindo escalabilidade e consistência eventual.  

## **3. Tecnologias Utilizadas**

- **Back-end (Microsserviços):**
  - C# / ASP.NET Core Web API
  - Entity Framework Core
  - IdentityServer (Duende)
  - Swagger (documentação)
- **Front-end:**
  - Angular
  - TypeScript
- **Banco de Dados:** SQL Server (ou SQLite em dev)
- **Mensageria:** RabbitMQ
- **Autenticação:**
  - JWT (JSON Web Token)
  - ASP.NET Core Identity

## **4. Estrutura do Projeto**

A estrutura do projeto (em evolução) é organizada da seguinte forma:

- src/
  - backend/
	- Alunos/
	- Auth/
	- BFF/
	- Conteudo/
	- Financeiro/
  - Frontend/ (Angular)
- README.md - Arquivo de Documentação do Projeto
- FEEDBACK.md - Arquivo para Consolidação dos Feedbacks
- .gitignore - Arquivo de Ignoração do Git

## **5. Funcionalidades Implementadas**

- **Autenticação e Autorização:** Controle de acesso via IdentityServer, diferenciando perfis de Administrador e Aluno.  
- **APIs RESTful:** Endpoints CRUD para cada contexto de negócio.  
- **Eventos de Domínio:** Comunicação assíncrona entre microsserviços.  
- **Swagger:** Documentação automática das APIs.  

### Decisões de negócio:
- **Administrador:** Cadastro único no IdentityServer com perfil **Administrador**.  
- **Alunos:** Cadastro em dois contextos:
  - Domínio **Aluno** (dados acadêmicos);  
  - IdentityServer (credenciais de acesso), perfil **Aluno**.  
- **Matrículas:**  
  - Vinculadas ao agregado **Aluno**;  
  - Cada matrícula pertence a um único curso;  
  - Um aluno pode ter apenas uma matrícula por curso.  
- **Pagamentos:**  
  - Vinculados à matrícula (e não ao aluno);  
  - Cada matrícula possui apenas um pagamento.  


## **6. Como Executar o Projeto**

### **Pré-requisitos**

- .NET SDK 9.0 ou superior  
- Node.js + Angular CLI  
- SQL Server (não necessário em dev se usar SQLite)  
- RabbitMQ (para mensageria)  
- Visual Studio 2022 ou superior (ou VS Code / Rider)  
- Git

### **Passos para Execução**

1. **Clone o Repositório:**
   - `git clone https://github.com/pedrogutierres/DevXpert.Academy.MS.git`
   - `cd DevXpert.Academy.MS`

2. **Execute o docker-compose.infra.yml ou execute o sh 'build-infra.sh' para levantar o RabbitMQ:**
   - `docker-compose -f docker-compose.infra.yml up -d`
   - ou
   - `sh build-infra.sh`

3. **Configuração do Banco de Dados:**
   - No arquivo `appsettings.json`, configure a string de conexão do SQL Server, se for SQLite em ambiente de desenvolvimento não precisa configurar nada.
   - Rode o projeto para que a configuração do Seed crie o banco e popule com os dados básicos

4. **Executar a API:**
   - Abra o Visual Studio com a solução "DevXpert.Academy.MS.sln"
   - Selecione a opção de execução "MultipleStartup" e execute
   - As documentações de cada API estão em:
	- BFF: http://localhost:5000/swagger
	- Auth: http://localhost:5001/swagger
	- Alunos: http://localhost:5002/swagger
	- Conteudo: http://localhost:5003/swagger
	- Financeiro: http://localhost:5004/swagger

### **Uso do Blog**

1. **Usuário Administrador Padrão:** Poderá ser utilizado como o administrador da API, onde poderá visualizar, editar e excluir cursos.
   - email: `admin@academy.com`
   - senha: `Academy@123456`
   
2. **Usuários Alunos:** Poderá ser criado mormalmente através da API, todo novo usuário será um Aluno automaticamente, mas também existem alguns default, como:
   - email: `eduardo.pires@desenvolvedor.io`
   - senha: `Eduardo@123456`

## **7. Instruções de Configuração**

- **JWT para API:** As chaves de configuração do JWT estão no `appsettings.json`.
- **Migrações do Banco de Dados:** As migrações são gerenciadas pelo Entity Framework Core. Não é necessário aplicar devido a configuração do Seed de dados.

## **8. Documentação da API**

A documentação da API está disponível através do Swagger. Após iniciar a API, acesse a documentação em:

http://localhost:5000/swagger

## **9. Avaliação**

- Este projeto é parte de um curso acadêmico e não aceita contribuições externas. 
- Para feedbacks ou dúvidas utilize o recurso de Issues
- O arquivo `FEEDBACK.md` é um resumo das avaliações do instrutor e deverá ser modificado apenas por ele.
