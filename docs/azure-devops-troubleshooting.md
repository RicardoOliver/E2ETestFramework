# 🔧 Troubleshooting Azure DevOps Pipeline

Este guia ajuda a resolver problemas comuns com o pipeline do Azure DevOps para o E2E Testing Framework.

## 📋 Problemas Comuns

### 🚫 Pipeline não é detectado automaticamente

**Problema**: O Azure DevOps não detecta o arquivo `azure-pipelines.yml` automaticamente.

**Soluções**:

1. **Verificar localização do arquivo**:
   - O arquivo `azure-pipelines.yml` deve estar na **raiz** do repositório
   - Confirme que o nome está correto (sem maiúsculas ou espaços)

2. **Verificar permissões**:
   - Verifique se você tem permissões para criar pipelines
   - Confirme que o Azure DevOps tem acesso ao repositório

3. **Criar pipeline manualmente**:
   \`\`\`
   1. Vá para Pipelines > New Pipeline
   2. Selecione seu repositório
   3. Escolha "Existing Azure Pipelines YAML file"
   4. Selecione o arquivo azure-pipelines.yml
   \`\`\`

4. **Verificar sintaxe YAML**:
   - Use um validador YAML para verificar erros de sintaxe
   - Confirme que não há caracteres especiais ou BOM no arquivo

### ❌ Falha na execução do pipeline

**Problema**: O pipeline é detectado, mas falha ao executar.

**Soluções**:

1. **Verificar logs de erro**:
   - Examine os logs detalhados da execução
   - Procure por erros específicos em cada etapa

2. **Verificar templates**:
   - Confirme que os arquivos de template existem nos caminhos corretos
   - Verifique se os templates têm a sintaxe correta

3. **Verificar variáveis**:
   - Confirme que todas as variáveis necessárias estão definidas
   - Verifique se as variáveis secretas estão configuradas

4. **Verificar agentes**:
   - Confirme que o pool de agentes está disponível
   - Verifique se o agente tem os requisitos necessários

### 🔄 Problemas com templates

**Problema**: Erros relacionados aos templates do pipeline.

**Soluções**:

1. **Estrutura de diretórios**:
   - Confirme que a pasta `templates` está na raiz do repositório
   - Verifique se os caminhos nos templates estão corretos

2. **Sintaxe de referência**:
   - Use o caminho relativo correto: `templates/nome-do-template.yml`
   - Verifique a indentação nos arquivos YAML

3. **Parâmetros**:
   - Confirme que todos os parâmetros necessários estão sendo passados
   - Verifique se os tipos de parâmetros estão corretos

### 🌐 Problemas com ambientes

**Problema**: Erros relacionados aos ambientes do pipeline.

**Soluções**:

1. **Criar ambientes**:
   \`\`\`
   1. Vá para Pipelines > Environments
   2. Crie os ambientes: Development, Staging, Production
   \`\`\`

2. **Configurar aprovações**:
   - Configure aprovações para ambientes que necessitam
   - Adicione aprovadores com permissões adequadas

3. **Variáveis de ambiente**:
   - Confirme que as variáveis específicas de ambiente estão definidas
   - Verifique se as condições de ambiente estão corretas

## 📝 Verificação Rápida

Use esta lista para verificar rapidamente problemas comuns:

- [ ] `azure-pipelines.yml` está na raiz do repositório
- [ ] Pasta `templates` existe e contém os arquivos necessários
- [ ] Ambientes estão criados no Azure DevOps
- [ ] Variáveis necessárias estão configuradas
- [ ] Permissões de pipeline estão corretas
- [ ] Agentes têm os requisitos necessários
- [ ] Não há erros de sintaxe no YAML

## 🔍 Ferramentas de Diagnóstico

1. **Validador YAML**:
   - Use [YAML Lint](http://www.yamllint.com/) para validar a sintaxe

2. **Azure DevOps CLI**:
   \`\`\`bash
   # Listar pipelines
   az pipelines list --organization https://dev.azure.com/yourorg --project YourProject
   
   # Ver detalhes do pipeline
   az pipelines show --id [pipelineId] --organization https://dev.azure.com/yourorg --project YourProject
   \`\`\`

3. **Diagnóstico de agentes**:
   \`\`\`bash
   # Verificar status dos agentes
   az pipelines agent list --pool-id [poolId] --organization https://dev.azure.com/yourorg
   \`\`\`

## 📞 Suporte

Se os problemas persistirem:

1. Consulte a [documentação oficial do Azure Pipelines](https://docs.microsoft.com/en-us/azure/devops/pipelines/)
2. Verifique o [status do Azure DevOps](https://status.dev.azure.com/)
3. Abra um ticket de suporte no [Azure DevOps Support](https://azure.microsoft.com/en-us/support/devops/)
