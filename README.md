# 🚀 Framework de Testes E2E com C# Selenium

[![Build Status](https://dev.azure.com/yourorg/E2ETestFramework/_apis/build/status/E2E-Testing-Pipeline?branchName=main)](https://dev.azure.com/yourorg/E2ETestFramework/_build/latest?definitionId=1&branchName=main)
[![Test Results](https://img.shields.io/azure-devops/tests/yourorg/E2ETestFramework/1?style=flat-square)](https://dev.azure.com/yourorg/E2ETestFramework/_build/latest?definitionId=1&branchName=main)
[![Quality Gate](https://img.shields.io/badge/Quality%20Gate-Passed-brightgreen?style=flat-square)](https://dev.azure.com/yourorg/E2ETestFramework/_build/latest?definitionId=1&branchName=main)
[![License](https://img.shields.io/badge/License-MIT-blue.svg?style=flat-square)](LICENSE)

Framework avançado de testes end-to-end utilizando C# Selenium, Page Object Model, SpecFlow e integração completa com Azure DevOps CI/CD.

## 🌟 Características Principais

- **🏗️ Page Object Model**: Implementação completa para melhor manutenção e reutilização
- **🥒 SpecFlow + Gherkin**: Cenários BDD em linguagem natural
- **🔄 STLC**: Estrutura baseada no Software Testing Life Cycle
- **📊 Relatórios Avançados**: ExtentReports com métricas detalhadas e screenshots
- **🚀 CI/CD Completo**: Pipeline Azure DevOps com múltiplos ambientes
- **🌐 Multi-browser**: Suporte para Chrome, Firefox e Edge com WebDriverManager
- **📈 Métricas**: Coleta automática de métricas de performance e execução
- **🔒 Testes de Segurança**: Validações de segurança e acessibilidade
- **♿ Acessibilidade**: Testes automatizados de acessibilidade
- **🎯 Quality Gates**: Gates de qualidade automáticos no pipeline

## 📁 Estrutura do Projeto

\`\`\`
E2ETestFramework/
├── 📂 Core/                           # Classes base e utilitários core
│   ├── 🔧 DriverFactory.cs            # Gerenciamento avançado de WebDriver
│   ├── 🌐 WebDriverManager.cs         # Wrapper para WebDriver
│   └── 📄 BasePage.cs                 # Classe base para Page Objects
├── 📂 Pages/                          # Page Objects
│   ├── 🔐 LoginPage.cs                # Page Object para página de login
│   └── 📊 DashboardPage.cs            # Page Object para dashboard
├── 📂 Utils/                          # Utilitários
│   └── 📋 TestDataManager.cs          # Gerenciamento de dados de teste
├── 📂 Reporting/                      # Relatórios
│   └── 📈 ExtentReportManager.cs      # Gerenciamento de relatórios
└── 📂 Metrics/                        # Métricas
    └── 📊 TestMetricsCollector.cs     # Coleta de métricas de teste

E2ETestFramework.Tests/
├── 📂 Features/                       # Arquivos .feature do SpecFlow
│   └── 🔐 Login.feature               # Cenários de login
├── 📂 StepDefinitions/                # Implementações dos steps
│   └── 🔐 LoginSteps.cs               # Steps para cenários de login
├── 📂 Hooks/                          # Hooks do SpecFlow
│   └── ⚙️ TestHooks.cs                # Hooks para configuração de testes
├── 📂 TestData/                       # Dados de teste
│   └── 👥 users.json                  # Dados de usuários para testes
└── 📂 docs/                           # Documentação
    └── 🚀 azure-devops-setup.md       # Guia de configuração Azure DevOps

Pipeline & Scripts/
├── 📂 scripts/                        # Scripts de automação
│   └── 🔧 setup-azure-devops.ps1      # Script de configuração automatizada
├── 📂 templates/                      # Templates do pipeline
│   ├── 🧪 e2e-test-template.yml       # Template para execução de testes
│   └── 📢 notification-template.yml   # Template para notificações
├── 📂 config/                         # Configurações
│   └── ⚙️ azure-devops-config.json    # Configuração do Azure DevOps
└── 🚀 azure-pipelines.yml             # Pipeline principal CI/CD
\`\`\`

## 🛠️ Pré-requisitos

- **.NET 8.0 SDK** ou superior
- **Visual Studio 2022** ou VS Code
- **Navegadores**: Chrome, Firefox, Edge (instalados automaticamente)
- **Azure DevOps** (para CI/CD)
- **Git** para controle de versão

## ⚡ Instalação Rápida

### 1. Clone o Repositório
\`\`\`bash
git clone https://github.com/yourorg/E2ETestFramework.git
cd E2ETestFramework
\`\`\`

### 2. Configuração Automatizada
\`\`\`bash
# Restaurar dependências
dotnet restore

# Executar configuração (Windows)
.\scripts\setup-azure-devops.ps1 -OrganizationUrl "https://dev.azure.com/yourorg" -ProjectName "YourProject" -PersonalAccessToken "your-pat"
\`\`\`

### 3. Configuração Manual
\`\`\`bash
# Abrir solução
start E2ETestFramework.sln

# Restaurar pacotes NuGet
dotnet restore

# Configurar appsettings.json
# Editar E2ETestFramework.Tests/appsettings.json
\`\`\`

## 🧪 Execução dos Testes

### 🖥️ Via Visual Studio
1. Abra o **Test Explorer** (`Test` > `Test Explorer`)
2. Selecione os testes desejados
3. Clique em **Run** ou **Debug**

### 💻 Via Linha de Comando

#### Executar Todos os Testes
\`\`\`bash
dotnet test E2ETestFramework.Tests/E2ETestFramework.Tests.csproj
\`\`\`

#### Executar por Categoria
\`\`\`bash
# Testes de smoke
dotnet test --filter Category=smoke

# Testes de login
dotnet test --filter Category=login

# Testes de segurança
dotnet test --filter Category=security

# Testes de acessibilidade
dotnet test --filter Category=accessibility
\`\`\`

#### Configurações de Ambiente
\`\`\`bash
# Modo headless
dotnet test --environment TestSettings__Headless=true

# Navegador específico
dotnet test --environment TestSettings__Browser=Firefox

# Ambiente específico
dotnet test --environment TestSettings__Environment=Staging
\`\`\`

### 🌐 Execução Multi-Browser
\`\`\`bash
# Chrome (padrão)
dotnet test --environment TestSettings__Browser=Chrome

# Firefox
dotnet test --environment TestSettings__Browser=Firefox

# Edge
dotnet test --environment TestSettings__Browser=Edge
\`\`\`

## 🚀 Azure DevOps CI/CD

### 📊 Pipeline Status
- **🟢 Main Branch**: [![Build Status](https://dev.azure.com/yourorg/E2ETestFramework/_apis/build/status/E2E-Testing-Pipeline?branchName=main)](https://dev.azure.com/yourorg/E2ETestFramework/_build/latest?definitionId=1&branchName=main)
- **🟡 Develop Branch**: [![Build Status](https://dev.azure.com/yourorg/E2ETestFramework/_apis/build/status/E2E-Testing-Pipeline?branchName=develop)](https://dev.azure.com/yourorg/E2ETestFramework/_build/latest?definitionId=1&branchName=develop)

### 🌍 Ambientes Configurados

| Ambiente | Branch | URL | Status | Aprovação |
|----------|--------|-----|--------|-----------|
| **Development** | `feature/*` | [Dev Environment](https://dev-app.company.com) | 🟢 Active | Automática |
| **Staging** | `develop` | [Staging Environment](https://staging-app.company.com) | 🟡 Active | Tech Lead |
| **Production** | `main` | [Production Environment](https://app.company.com) | 🔴 Active | Tech Lead + PO |

### 🔧 Configuração do Pipeline

#### Setup Automatizado
\`\`\`powershell
# Execute o script de configuração
.\scripts\setup-azure-devops.ps1 `
  -OrganizationUrl "https://dev.azure.com/yourorg" `
  -ProjectName "E2ETestFramework" `
  -PersonalAccessToken "your-pat" `
  -TeamsWebhookUrl "your-teams-webhook" `
  -SlackWebhookUrl "your-slack-webhook"
\`\`\`

#### Setup Manual
1. Siga o guia detalhado: [`docs/azure-devops-setup.md`](docs/azure-devops-setup.md)
2. Configure ambientes e variáveis
3. Defina aprovações e quality gates

### 📈 Quality Gates
- ✅ **Taxa de Sucesso**: ≥ 90%
- ✅ **Cobertura de Código**: ≥ 80%
- ✅ **Issues de Segurança**: 0 críticos
- ✅ **Performance**: < 30s por teste

## 📊 Relatórios e Métricas

### 📋 Relatórios Gerados
- **📄 HTML Reports**: `/Reports/TestReport_[timestamp].html`
- **📸 Screenshots**: `/Screenshots/` (em caso de falha)
- **📊 Métricas JSON**: `/Reports/TestMetrics_[timestamp].json`
- **📝 Logs Detalhados**: `/Logs/test-log-[date].txt`

### 📈 Dashboard Azure DevOps
Acesse o dashboard: [E2E Testing Dashboard](https://dev.azure.com/yourorg/E2ETestFramework/_dashboards/dashboard/12345678-1234-1234-1234-123456789012)

### 📊 Métricas Coletadas
- ⏱️ Tempo de execução por teste
- 📈 Taxa de sucesso/falha
- 🌐 Performance por navegador
- 🔍 Cobertura de funcionalidades
- 🚨 Alertas de regressão

## 🏷️ Tags e Categorias de Teste

| Tag | Descrição | Tempo Estimado |
|-----|-----------|----------------|
| `@smoke` | Testes críticos de smoke | ~5 min |
| `@login` | Testes de autenticação | ~10 min |
| `@negative` | Testes de cenários negativos | ~15 min |
| `@security` | Testes de segurança | ~20 min |
| `@accessibility` | Testes de acessibilidade | ~10 min |
| `@regression` | Suite completa de regressão | ~45 min |

## 🔔 Notificações

### 📢 Microsoft Teams
- ✅ Build bem-sucedido
- ❌ Falhas de build
- 🚀 Deploy realizado
- ⚠️ Quality gates falharam

### 📧 Email
- 📊 Relatório semanal de métricas
- 🚨 Alertas críticos
- 📈 Resumo mensal de qualidade

### 📱 Slack (Opcional)
- 🔄 Status de pipeline em tempo real
- 📊 Métricas de teste
- 🎯 Alertas de quality gate

## 🔧 Configuração Avançada

### ⚙️ Arquivo de Configuração
\`\`\`json
{
  "TestSettings": {
    "Browser": "Chrome",
    "Headless": false,
    "TimeoutSeconds": 30,
    "BaseUrl": "https://the-internet.herokuapp.com",
    "Environment": "Test",
    "ScreenshotOnFailure": true,
    "VideoRecording": false,
    "MaxParallelTests": 3
  }
}
\`\`\`

### 🌐 Variáveis de Ambiente
\`\`\`bash
# Configuração via variáveis de ambiente
export TestSettings__Browser=Firefox
export TestSettings__Headless=true
export TestSettings__BaseUrl=https://staging.company.com
\`\`\`

### 🔒 Configurações de Segurança
\`\`\`json
{
  "SecuritySettings": {
    "EnableSecurityTests": true,
    "CheckForSQLInjection": true,
    "ValidateSSLCertificates": true,
    "TestCSRFProtection": true
  }
}
\`\`\`

## 🚨 Troubleshooting

### ❓ Problemas Comuns

#### 🌐 WebDriver Issues
\`\`\`bash
# Limpar cache de drivers
rm -rf ~/.cache/selenium/

# Reinstalar WebDriverManager
dotnet add package WebDriverManager --version 2.17.4
\`\`\`

#### 🧪 Testes Falhando
\`\`\`bash
# Executar em modo debug
dotnet test --logger "console;verbosity=detailed"

# Verificar screenshots
ls Screenshots/

# Verificar logs
tail -f Logs/test-log-$(date +%Y%m%d).txt
\`\`\`

#### 🚀 Pipeline Issues
- ✅ Verificar variáveis do pipeline
- ✅ Confirmar service connections
- ✅ Validar branch policies
- ✅ Checar quality gates

### 📞 Suporte
1. 📖 Consulte a [documentação completa](docs/)
2. 🔍 Verifique os [logs detalhados](Logs/)
3. 📊 Analise o [dashboard de métricas](https://dev.azure.com/yourorg/E2ETestFramework/_dashboards)
4. 🎫 Abra um ticket no [Azure DevOps](https://dev.azure.com/yourorg/E2ETestFramework/_workitems)

## 🤝 Contribuição

### 🔄 Workflow de Desenvolvimento
1. **Fork** o repositório
2. **Crie** uma branch feature (`git checkout -b feature/AmazingFeature`)
3. **Commit** suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. **Push** para a branch (`git push origin feature/AmazingFeature`)
5. **Abra** um Pull Request

### ✅ Checklist de PR
- [ ] Testes passando localmente
- [ ] Código seguindo padrões
- [ ] Documentação atualizada
- [ ] Screenshots de falhas analisadas
- [ ] Quality gates atendidos

### 🎯 Guidelines
- 📝 Use mensagens de commit descritivas
- 🧪 Adicione testes para novas funcionalidades
- 📖 Atualize documentação quando necessário
- 🔍 Siga os padrões de código estabelecidos

## 📈 Roadmap

### 🎯 Próximas Funcionalidades
- [ ] 🤖 Integração com AI para geração de testes
- [ ] 📱 Suporte para testes mobile (Appium)
- [ ] 🐳 Containerização com Docker
- [ ] ☁️ Execução em nuvem (Azure Container Instances)
- [ ] 📊 Dashboard personalizado com Power BI
- [ ] 🔍 Integração com SonarQube
- [ ] 🛡️ Testes de penetração automatizados

### 🏆 Melhorias Planejadas
- [ ] ⚡ Otimização de performance
- [ ] 🎨 Interface de configuração web
- [ ] 📧 Relatórios personalizáveis
- [ ] 🔄 Auto-healing de testes
- [ ] 📱 App mobile para monitoramento

## 📄 Licença

Este projeto está licenciado sob a **MIT License** - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🙏 Agradecimentos

- **Selenium WebDriver** - Framework de automação web
- **SpecFlow** - Framework BDD para .NET
- **ExtentReports** - Relatórios avançados
- **Azure DevOps** - Plataforma CI/CD
- **Microsoft** - Ferramentas de desenvolvimento

## 📊 Estatísticas do Projeto

![GitHub repo size](https://img.shields.io/github/repo-size/yourorg/E2ETestFramework?style=flat-square)
![GitHub contributors](https://img.shields.io/github/contributors/yourorg/E2ETestFramework?style=flat-square)
![GitHub last commit](https://img.shields.io/github/last-commit/yourorg/E2ETestFramework?style=flat-square)
![GitHub issues](https://img.shields.io/github/issues/yourorg/E2ETestFramework?style=flat-square)
![GitHub pull requests](https://img.shields.io/github/issues-pr/yourorg/E2ETestFramework?style=flat-square)

---

<div align="center">

**🚀 Desenvolvido com ❤️ para automação de testes de qualidade**

[📖 Documentação](docs/) • [🐛 Reportar Bug](https://github.com/yourorg/E2ETestFramework/issues) • [💡 Solicitar Feature](https://github.com/yourorg/E2ETestFramework/issues) • [💬 Discussões](https://github.com/yourorg/E2ETestFramework/discussions)

</div>
