# **DevXpert Academy - Aplicação de Plataforma de Educação Online API RESTful**

## **1. Apresentação**

Bem-vindo ao repositório do projeto **DevXpert Academy**. Este projeto é uma entrega do MBA DevXpert Full Stack .NET e é referente ao módulo **Introdução ao Desenvolvimento ASP.NET Core**.
O objetivo principal desenvolver uma aplicação de blog que permite aos usuários criar, editar, visualizar e excluir posts e comentários, tanto através de uma interface web utilizando MVC quanto através de uma API RESTful.
Descreva livremente mais detalhes do seu projeto aqui.

### **Autor**
- **Pedro Otávio Gutierres**

## **2. Proposta do Projeto**

O projeto consiste em:

- **API RESTful:** Exposição dos recursos do blog para integração com outras aplicações ou desenvolvimento de front-ends alternativos.
- **Autenticação e Autorização:** Implementação de controle de acesso, diferenciando administradores e usuários comuns.
- **Acesso a Dados:** Implementação de acesso ao banco de dados através de ORM.
- **Modelagem de Dominios Ricos:** Utilizando DDD, TDD, CQRS e outros padrões arquiteturais.

## **3. Tecnologias Utilizadas**

- **Linguagem de Programação:** C#
- **Frameworks:**
  - ASP.NET Core Web API
  - Entity Framework Core
- **Banco de Dados:** SQL Server (ou SQLite)
- **Autenticação e Autorização:**
  - ASP.NET Core Identity
  - JWT (JSON Web Token) para autenticação na API
- **Documentação da API:** Swagger

## **4. Estrutura do Projeto**

A estrutura do projeto é organizada da seguinte forma:

- src/
  - [em construção] ...
- README.md - Arquivo de Documentação do Projeto
- FEEDBACK.md - Arquivo para Consolidação dos Feedbacks
- .gitignore - Arquivo de Ignoração do Git

## **5. Funcionalidades Implementadas**

- **Autenticação e Autorização:** Diferenciação entre usuários comuns e administradores.
- **API RESTful:** Exposição de endpoints para operações CRUD via API.
- **Documentação da API:** Documentação automática dos endpoints da API utilizando Swagger.

### Decisões de negócios:
- **Administrador:** O administrador terá apenas um cadastro no IdentityServer como Usuário e perfil Administrador;
- **Alunos:** Um aluno terá um cadastro na base de negócio como Aluno e um cadastro no IdentityServer como Usuário, com perfil Aluno;
- **Matriculas:** As matriculas são vinculadas ao agregador Aluno
  - Cada matricula terá um curso vinculado
  - O aluno poderá ter apenas uma matrícula por curso
- **Pagamentos:** Os pagamentos serão vínculados à matrícula, e não ao aluno, pois o aluno poderá ter várias matrículas, mas cada matrícula terá apenas um pagamento vinculado;

## **6. Como Executar o Projeto**

### **Pré-requisitos**

- .NET SDK 9.0 ou superior
- SQL Server (não necessário para ambiente de desenvolvimento)
- Visual Studio 2022 ou superior (ou qualquer IDE de sua preferência)
- Git

### **Passos para Execução**

1. **Clone o Repositório:**
   - `git clone https://github.com/pedrogutierres/DevXpert.Academy.git`
   - `cd DevXpert.Academy`

2. **Configuração do Banco de Dados:**
   - No arquivo `appsettings.json`, configure a string de conexão do SQL Server ou SQLite em ambiente de desenvolvimento.
   - Rode o projeto para que a configuração do Seed crie o banco e popule com os dados básicos

3. **Executar a API:**
   - `cd src/[em construção]...
   - `dotnet run`
   - Acesse a documentação da API em: http://localhost:5001/swagger

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

http://localhost:5001/swagger

> **Documentação PlantUML disponível em [/docs/PlantUML](https://github.com/pedrogutierres/DevXpert.Academy/tree/master/docs/PlantUML)**

## **9. Avaliação**

- Este projeto é parte de um curso acadêmico e não aceita contribuições externas. 
- Para feedbacks ou dúvidas utilize o recurso de Issues
- O arquivo `FEEDBACK.md` é um resumo das avaliações do instrutor e deverá ser modificado apenas por ele.

## **10. Documentação PlantUML**

**TAMBÉM ESTÁ DISPONÍVEL DOCUMENTAÇÃO DA ESTRUTURA E ALGUNS FLUXOS DO PROJETO, VOCÊ PODE ENCONTRAR OS ARQUIVOS PLANTUML E SUAS IMAGENS NA PASTA [/docs/PlantUML](https://github.com/pedrogutierres/DevXpert.Academy/tree/master/docs/PlantUML)**