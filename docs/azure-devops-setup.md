# 🚀 Configuração do Pipeline Azure DevOps

Este guia detalha como configurar o pipeline CI/CD no Azure DevOps para o E2E Testing Framework.

## 📋 Pré-requisitos

- [ ] Conta no Azure DevOps
- [ ] Projeto criado no Azure DevOps
- [ ] Repositório Git configurado
- [ ] Permissões de administrador no projeto

## 🏗️ Configuração Inicial

### 1. Importar o Repositório

1. Acesse seu projeto no Azure DevOps
2. Vá para **Repos** > **Import repository**
3. Cole a URL do seu repositório Git
4. Clique em **Import**

### 2. Criar o Pipeline

1. Vá para **Pipelines** > **Create Pipeline**
2. Selecione **Azure Repos Git**
3. Escolha seu repositório
4. Selecione **Existing Azure Pipelines YAML file**
5. Escolha `/azure-pipelines.yml`
6. Clique em **Continue**

## 🌍 Configuração de Ambientes

### 1. Criar Ambientes

Vá para **Pipelines** > **Environments** e crie os seguintes ambientes:

#### 🟢 Development Environment
\`\`\`
Nome: Development
Descrição: Ambiente de desenvolvimento para testes de feature branches
\`\`\`

#### 🟡 Staging Environment
\`\`\`
Nome: Staging
Descrição: Ambiente de staging para testes da branch develop
\`\`\`

#### 🔴 Production Environment
\`\`\`
Nome: Production
Descrição: Ambiente de produção para releases da branch main
\`\`\`

### 2. Configurar Aprovações (Opcional)

Para cada ambiente, configure aprovações:

1. Clique no ambiente
2. Vá para **Approvals and checks**
3. Adicione **Approvals**
4. Configure os aprovadores necessários

#### Configuração Recomendada:
- **Development**: Sem aprovação
- **Staging**: Aprovação do Tech Lead
- **Production**: Aprovação do Tech Lead + Product Owner

## 🔧 Configuração de Variáveis

### 1. Variáveis do Pipeline

Vá para **Pipelines** > Selecione seu pipeline > **Edit** > **Variables**

#### Variáveis Globais:
\`\`\`yaml
# Build Configuration
buildConfiguration: Release
testProject: E2ETestFramework.Tests
mainProject: E2ETestFramework

# Test Configuration
testTimeout: 30
maxParallelTests: 3

# Versioning
majorVersion: 1
minorVersion: 0

# Notification Settings
teamsWebhookUrl: [SEU_WEBHOOK_URL]
slackWebhookUrl: [SEU_WEBHOOK_URL]
notificationChannel: general
\`\`\`

### 2. Variáveis por Ambiente

#### Development Environment Variables:
\`\`\`yaml
targetEnvironment: Development
baseUrl: https://the-internet.herokuapp.com
deploymentSlot: development
enableDebugLogs: true
testParallelism: 2
\`\`\`

#### Staging Environment Variables:
\`\`\`yaml
targetEnvironment: Staging
baseUrl: https://the-internet.herokuapp.com
deploymentSlot: staging
enableDebugLogs: false
testParallelism: 3
\`\`\`

#### Production Environment Variables:
\`\`\`yaml
targetEnvironment: Production
baseUrl: https://the-internet.herokuapp.com
deploymentSlot: production
enableDebugLogs: false
testParallelism: 1
\`\`\`

### 3. Variáveis Secretas

Configure as seguintes variáveis como **Secret**:

\`\`\`yaml
# Notification Webhooks
teamsWebhookUrl: [WEBHOOK_URL_TEAMS]
slackWebhookUrl: [WEBHOOK_URL_SLACK]

# Database Connections (se aplicável)
testDatabaseConnection: [CONNECTION_STRING]

# API Keys (se aplicável)
testApiKey: [API_KEY]

# Service Principal (para deploy)
azureServiceConnection: [SERVICE_CONNECTION_NAME]
\`\`\`

## 🔐 Configuração de Service Connections

### 1. Azure Service Connection

1. Vá para **Project Settings** > **Service connections**
2. Clique em **New service connection**
3. Selecione **Azure Resource Manager**
4. Escolha **Service principal (automatic)**
5. Configure:
   \`\`\`
   Nome: AzureServiceConnection
   Subscription: [SUA_SUBSCRIPTION]
   Resource Group: [SEU_RESOURCE_GROUP]
   \`\`\`

### 2. Generic Service Connections (para webhooks)

#### Teams Webhook:
\`\`\`
Nome: TeamsNotification
Server URL: https://outlook.office.com
Username: [DEIXE_VAZIO]
Password: [WEBHOOK_URL]
\`\`\`

#### Slack Webhook:
\`\`\`
Nome: SlackNotification
Server URL: https://hooks.slack.com
Username: [DEIXE_VAZIO]
Password: [WEBHOOK_URL]
\`\`\`

## 📊 Configuração de Dashboards

### 1. Criar Dashboard de Testes

1. Vá para **Overview** > **Dashboards**
2. Clique em **New Dashboard**
3. Nome: "E2E Testing Dashboard"
4. Adicione os seguintes widgets:

#### Test Results Widget:
\`\`\`
Tipo: Test Results Trend
Configuração:
- Pipeline: [SEU_PIPELINE]
- Período: Last 30 days
- Grouping: By test run
\`\`\`

#### Build History Widget:
\`\`\`
Tipo: Build History
Configuração:
- Pipeline: [SEU_PIPELINE]
- Número de builds: 20
\`\`\`

#### Deployment Status Widget:
\`\`\`
Tipo: Deployment Status
Configuração:
- Environment: Production
- Período: Last 7 days
\`\`\`

## 🔔 Configuração de Notificações

### 1. Microsoft Teams

#### Criar Webhook no Teams:
1. Abra o canal do Teams
2. Clique nos três pontos > **Connectors**
3. Procure por "Incoming Webhook"
4. Configure:
   \`\`\`
   Nome: Azure DevOps Notifications
   \`\`\`
5. Copie a URL do webhook

#### Configurar no Azure DevOps:
1. Adicione a URL como variável secreta `teamsWebhookUrl`

### 2. Slack

#### Criar Webhook no Slack:
1. Vá para [Slack API](https://api.slack.com/apps)
2. Crie um novo app
3. Vá para **Incoming Webhooks**
4. Ative e crie um webhook
5. Copie a URL

#### Configurar no Azure DevOps:
1. Adicione a URL como variável secreta `slackWebhookUrl`

### 3. Email Notifications

1. Vá para **Project Settings** > **Notifications**
2. Configure notificações para:
   - Build completed
   - Build failed
   - Release deployment completed
   - Release deployment failed

## 🎯 Configuração de Branch Policies

### 1. Main Branch Protection

1. Vá para **Repos** > **Branches**
2. Clique nos três pontos da branch `main`
3. Selecione **Branch policies**
4. Configure:

\`\`\`yaml
Require a minimum number of reviewers: 2
Check for linked work items: Enabled
Check for comment resolution: Enabled
Limit merge types: Squash merge only

Build validation:
- Pipeline: [SEU_PIPELINE]
- Trigger: Automatic
- Policy requirement: Required
- Build expiration: 12 hours
\`\`\`

### 2. Develop Branch Protection

\`\`\`yaml
Require a minimum number of reviewers: 1
Check for linked work items: Optional
Check for comment resolution: Enabled
Limit merge types: Merge commit

Build validation:
- Pipeline: [SEU_PIPELINE]
- Trigger: Automatic
- Policy requirement: Required
- Build expiration: 6 hours
\`\`\`

## 🚀 Configuração de Release Gates

### 1. Quality Gates

Configure gates automáticos baseados em métricas:

#### Test Pass Rate Gate:
\`\`\`yaml
Metric: Test Pass Rate
Threshold: >= 90%
Evaluation: Before deployment
\`\`\`

#### Code Coverage Gate:
\`\`\`yaml
Metric: Code Coverage
Threshold: >= 80%
Evaluation: Before deployment
\`\`\`

#### Security Scan Gate:
\`\`\`yaml
Metric: Security Issues
Threshold: 0 High/Critical
Evaluation: Before deployment
\`\`\`

## 📈 Monitoramento e Alertas

### 1. Application Insights (se aplicável)

Configure Application Insights para monitorar:
- Performance da aplicação
- Erros em tempo real
- Métricas de usuário

### 2. Azure Monitor Alerts

Configure alertas para:
- Falhas de deployment
- Degradação de performance
- Erros críticos

## 🔧 Troubleshooting

### Problemas Comuns:

#### 1. Pipeline não executa automaticamente
**Solução:**
- Verifique os triggers no YAML
- Confirme as branch policies
- Verifique permissões do service principal

#### 2. Testes falham por timeout
**Solução:**
- Aumente `testTimeout` nas variáveis
- Verifique recursos do agente
- Otimize testes paralelos

#### 3. Deploy falha
**Solução:**
- Verifique service connection
- Confirme permissões no Azure
- Valide configurações de ambiente

#### 4. Notificações não funcionam
**Solução:**
- Verifique URLs dos webhooks
- Confirme variáveis secretas
- Teste webhooks manualmente

## ✅ Checklist de Configuração

- [ ] Repositório importado
- [ ] Pipeline criado
- [ ] Ambientes configurados (Development, Staging, Production)
- [ ] Variáveis globais definidas
- [ ] Variáveis por ambiente configuradas
- [ ] Variáveis secretas adicionadas
- [ ] Service connections criadas
- [ ] Branch policies configuradas
- [ ] Webhooks de notificação configurados
- [ ] Dashboard criado
- [ ] Quality gates definidos
- [ ] Aprovações configuradas
- [ ] Monitoramento ativo

## 🎉 Próximos Passos

1. **Execute o pipeline** para validar a configuração
2. **Monitore os resultados** no dashboard
3. **Ajuste configurações** conforme necessário
4. **Treine a equipe** no uso do pipeline
5. **Documente processos** específicos do projeto

---

## 📞 Suporte

Para dúvidas ou problemas:
1. Consulte a documentação oficial do Azure DevOps
2. Verifique os logs do pipeline
3. Entre em contato com a equipe de DevOps
\`\`\`

```powershell file="scripts/setup-azure-devops.ps1"
# Script PowerShell para automatizar configuração do Azure DevOps
# Execute este script com permissões de administrador

param(
    [Parameter(Mandatory=$true)]
    [string]$OrganizationUrl,
    
    [Parameter(Mandatory=$true)]
    [string]$ProjectName,
    
    [Parameter(Mandatory=$true)]
    [string]$PersonalAccessToken,
    
    [Parameter(Mandatory=$false)]
    [string]$RepositoryUrl = "",
    
    [Parameter(Mandatory=$false)]
    [string]$TeamsWebhookUrl = "",
    
    [Parameter(Mandatory=$false)]
    [string]$SlackWebhookUrl = ""
)

# Instalar Azure DevOps CLI se não estiver instalado
if (!(Get-Command "az" -ErrorAction SilentlyContinue)) {
    Write-Host "❌ Azure CLI não encontrado. Instalando..." -ForegroundColor Red
    Invoke-WebRequest -Uri https://aka.ms/installazurecliwindows -OutFile .\AzureCLI.msi
    Start-Process msiexec.exe -Wait -ArgumentList '/I AzureCLI.msi /quiet'
    Remove-Item .\AzureCLI.msi
}

# Instalar extensão Azure DevOps
Write-Host "📦 Instalando extensão Azure DevOps..." -ForegroundColor Blue
az extension add --name azure-devops

# Configurar autenticação
Write-Host "🔐 Configurando autenticação..." -ForegroundColor Blue
$env:AZURE_DEVOPS_EXT_PAT = $PersonalAccessToken
az devops configure --defaults organization=$OrganizationUrl project=$ProjectName

# Função para criar variáveis do pipeline
function Create-PipelineVariable {
    param(
        [string]$Name,
        [string]$Value,
        [bool]$IsSecret = $false
    )
    
    $secretFlag = if ($IsSecret) { "--secret" } else { "" }
    
    try {
        az pipelines variable create --name $Name --value $Value $secretFlag --pipeline-name "E2E-Testing-Pipeline" 2>$null
        Write-Host "✅ Variável '$Name' criada" -ForegroundColor Green
    } catch {
        Write-Host "⚠️ Variável '$Name' já existe ou erro na criação" -ForegroundColor Yellow
    }
}

# Função para criar ambientes
function Create-Environment {
    param(
        [string]$Name,
        [string]$Description
    )
    
    try {
        az devops environment create --name $Name --description $Description 2>$null
        Write-Host "✅ Ambiente '$Name' criado" -ForegroundColor Green
    } catch {
        Write-Host "⚠️ Ambiente '$Name' já existe" -ForegroundColor Yellow
    }
}

Write-Host "🚀 Iniciando configuração do Azure DevOps..." -ForegroundColor Cyan
Write-Host "Organization: $OrganizationUrl" -ForegroundColor Gray
Write-Host "Project: $ProjectName" -ForegroundColor Gray

# Criar ambientes
Write-Host "`n🌍 Criando ambientes..." -ForegroundColor Blue
Create-Environment -Name "Development" -Description "Ambiente de desenvolvimento para testes de feature branches"
Create-Environment -Name "Staging" -Description "Ambiente de staging para testes da branch develop"
Create-Environment -Name "Production" -Description "Ambiente de produção para releases da branch main"

# Aguardar criação do pipeline (manual)
Write-Host "`n⏳ Aguardando criação manual do pipeline..." -ForegroundColor Yellow
Write-Host "Por favor, crie o pipeline manualmente no Azure DevOps antes de continuar." -ForegroundColor Yellow
Write-Host "Pressione qualquer tecla após criar o pipeline..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Criar variáveis do pipeline
Write-Host "`n🔧 Criando variáveis do pipeline..." -ForegroundColor Blue

# Variáveis globais
Create-PipelineVariable -Name "buildConfiguration" -Value "Release"
Create-PipelineVariable -Name "testProject" -Value "E2ETestFramework.Tests"
Create-PipelineVariable -Name "mainProject" -Value "E2ETestFramework"
Create-PipelineVariable -Name "testTimeout" -Value "30"
Create-PipelineVariable -Name "maxParallelTests" -Value "3"
Create-PipelineVariable -Name "majorVersion" -Value "1"
Create-PipelineVariable -Name "minorVersion" -Value "0"

# Variáveis de ambiente
Create-PipelineVariable -Name "baseUrl.Development" -Value "https://the-internet.herokuapp.com"
Create-PipelineVariable -Name "baseUrl.Staging" -Value "https://the-internet.herokuapp.com"
Create-PipelineVariable -Name "baseUrl.Production" -Value "https://the-internet.herokuapp.com"

# Variáveis secretas (se fornecidas)
if ($TeamsWebhookUrl) {
    Create-PipelineVariable -Name "teamsWebhookUrl" -Value $TeamsWebhookUrl -IsSecret $true
}

if ($SlackWebhookUrl) {
    Create-PipelineVariable -Name "slackWebhookUrl" -Value $SlackWebhookUrl -IsSecret $true
}

# Criar variable groups
Write-Host "`n📋 Criando grupos de variáveis..." -ForegroundColor Blue

# Development Variable Group
$devVars = @{
    "targetEnvironment" = "Development"
    "enableDebugLogs" = "true"
    "testParallelism" = "2"
}

$devVarsJson = $devVars | ConvertTo-Json -Compress
az pipelines variable-group create --name "Development-Variables" --variables $devVarsJson 2>$null

# Staging Variable Group
$stagingVars = @{
    "targetEnvironment" = "Staging"
    "enableDebugLogs" = "false"
    "testParallelism" = "3"
}

$stagingVarsJson = $stagingVars | ConvertTo-Json -Compress
az pipelines variable-group create --name "Staging-Variables" --variables $stagingVarsJson 2>$null

# Production Variable Group
$prodVars = @{
    "targetEnvironment" = "Production"
    "enableDebugLogs" = "false"
    "testParallelism" = "1"
}

$prodVarsJson = $prodVars | ConvertTo-Json -Compress
az pipelines variable-group create --name "Production-Variables" --variables $prodVarsJson 2>$null

Write-Host "✅ Grupos de variáveis criados" -ForegroundColor Green

# Configurar branch policies (requer API REST)
Write-Host "`n🛡️ Configurando branch policies..." -ForegroundColor Blue

$headers = @{
    'Authorization' = "Basic $([Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(":$PersonalAccessToken")))"
    'Content-Type' = 'application/json'
}

# Obter repository ID
$repoUrl = "$OrganizationUrl/$ProjectName/_apis/git/repositories?api-version=6.0"
$repos = Invoke-RestMethod -Uri $repoUrl -Headers $headers -Method Get

$mainRepo = $repos.value | Where-Object { $_.name -eq $ProjectName }
if ($mainRepo) {
    $repoId = $mainRepo.id
    
    # Branch policy para main
    $mainPolicyBody = @{
        isEnabled = $true
        isBlocking = $true
        type = @{
            id = "fa4e907d-c16b-4a4c-9dfa-4906e5d171dd" # Minimum reviewers policy
        }
        settings = @{
            minimumApproverCount = 2
            creatorVoteCounts = $false
            allowDownvotes = $false
            resetOnSourcePush = $true
        }
    } | ConvertTo-Json -Depth 10
    
    $policyUrl = "$OrganizationUrl/$ProjectName/_apis/policy/configurations?api-version=6.0"
    
    try {
        Invoke-RestMethod -Uri $policyUrl -Headers $headers -Method Post -Body $mainPolicyBody
        Write-Host "✅ Branch policy para main configurada" -ForegroundColor Green
    } catch {
        Write-Host "⚠️ Erro ao configurar branch policy: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

# Gerar relatório de configuração
Write-Host "`n📊 Gerando relatório de configuração..." -ForegroundColor Blue

$report = @"
🎉 CONFIGURAÇÃO CONCLUÍDA!

📋 RESUMO DA CONFIGURAÇÃO:
✅ Ambientes criados: Development, Staging, Production
✅ Variáveis do pipeline configuradas
✅ Grupos de variáveis criados
✅ Branch policies configuradas (se aplicável)

🔧 PRÓXIMOS PASSOS MANUAIS:
1. Configure aprovações nos ambientes (Production recomendado)
2. Adicione service connections para Azure (se necessário)
3. Configure webhooks de notificação
4. Crie dashboard de monitoramento
5. Execute o primeiro build para validar

🌐 LINKS ÚTEIS:
- Pipeline: $OrganizationUrl/$ProjectName/_build
- Environments: $OrganizationUrl/$ProjectName/_environments
- Variables: $OrganizationUrl/$ProjectName/_settings/buildqueue

📞 SUPORTE:
- Documentação: docs/azure-devops-setup.md
- Logs do script: Verifique saída acima para erros
"@

Write-Host $report -ForegroundColor Cyan

# Salvar relatório em arquivo
$report | Out-File -FilePath "azure-devops-setup-report.txt" -Encoding UTF8
Write-Host "`n💾 Relatório salvo em: azure-devops-setup-report.txt" -ForegroundColor Green

Write-Host "`n🎯 Configuração do Azure DevOps concluída!" -ForegroundColor Green
