# Como Publicar uma Nova Versão no NuGet

Este guia explica como publicar uma nova versão do pacote FlyGon.CQRS no NuGet usando GitHub Actions.

## Pré-requisitos

### 1. Configurar API Key do NuGet

1. Acesse [NuGet.org](https://www.nuget.org/) e faça login
2. Vá em **Account Settings** → **API Keys**
3. Clique em **Create** para criar uma nova API Key
4. Configure:
   - **Key Name**: `GitHub Actions - FlyGon.CQRS`
   - **Select Scopes**: `Push` e `Push new packages and package versions`
   - **Select Packages**: `FlyGon.CQRS` (ou deixe em branco para todos)
   - **Glob Pattern**: `FlyGon.CQRS`
5. Copie a API Key gerada (você só verá uma vez!)

### 2. Adicionar Secret no GitHub

1. Vá para o repositório no GitHub: https://github.com/gregojoao/FlyGon.CQRS
2. Clique em **Settings** → **Secrets and variables** → **Actions**
3. Clique em **New repository secret**
4. Configure:
   - **Name**: `NUGET_API_KEY`
   - **Secret**: Cole a API Key do NuGet
5. Clique em **Add secret**

## Métodos de Publicação

### Método 1: Via GitHub Release (Recomendado)

Este é o método mais comum e recomendado.

#### Passo a Passo:

1. **Atualizar a versão no .csproj**
   ```xml
   <Version>2.0.0</Version>
   <AssemblyVersion>2.0.0.0</AssemblyVersion>
   <FileVersion>2.0.0.0</FileVersion>
   ```

2. **Fazer commit e push**
   ```bash
   git add src/FlyGon.CQRS/FlyGon.CQRS.csproj
   git commit -m "Bump version to 2.0.0"
   git push origin master
   ```

3. **Criar uma tag**
   ```bash
   git tag v2.0.0
   git push origin v2.0.0
   ```

4. **Criar Release no GitHub**
   - Acesse: https://github.com/gregojoao/FlyGon.CQRS/releases/new
   - **Choose a tag**: Selecione `v2.0.0` (ou a tag que você criou)
   - **Release title**: `v2.0.0 - Upgrade to .NET 10`
   - **Description**: Descreva as mudanças (veja exemplo abaixo)
   - Marque **Set as the latest release**
   - Clique em **Publish release**

5. **Aguardar a publicação**
   - O GitHub Actions será acionado automaticamente
   - Acompanhe em: https://github.com/gregojoao/FlyGon.CQRS/actions
   - Em alguns minutos, o pacote estará disponível no NuGet

#### Exemplo de Descrição de Release:

```markdown
## 🚀 What's New in v2.0.0

### Major Changes
- ✅ Upgraded to .NET 10 (LTS until November 2028)
- ✅ Updated all dependencies to latest versions
- ✅ Added comprehensive unit test coverage (33 tests)

### Breaking Changes
⚠️ **Target Framework**: Changed from `netstandard2.1` to `net10.0`
- Projects must target .NET 10 or later

### Dependencies Updated
- FluentAssertions: 6.12.2 → 8.9.0
- Microsoft.NET.Test.Sdk: 17.11.1 → 18.5.1
- xunit.runner.visualstudio: 2.8.2 → 3.1.5

### Metadata
- Company updated to "Greco Labs"
- Repository URLs updated

## 📦 Installation

```bash
dotnet add package FlyGon.CQRS --version 2.0.0
```

## 🔗 Links
- [NuGet Package](https://www.nuget.org/packages/FlyGon.CQRS/2.0.0)
- [Documentation](https://github.com/gregojoao/FlyGon.CQRS#readme)
```

### Método 2: Via Workflow Manual

Se você não quiser criar uma release, pode acionar o workflow manualmente.

#### Passo a Passo:

1. Acesse: https://github.com/gregojoao/FlyGon.CQRS/actions/workflows/publish-nuget.yml
2. Clique em **Run workflow**
3. Selecione o branch (geralmente `master`)
4. Digite a versão (ex: `2.0.0`)
5. Clique em **Run workflow**

**Nota**: Certifique-se de que a versão no `.csproj` corresponde à versão que você está publicando.

## Verificação

### 1. Verificar no GitHub Actions
- Acesse: https://github.com/gregojoao/FlyGon.CQRS/actions
- Verifique se o workflow "Publish to NuGet" foi executado com sucesso
- Todos os steps devem estar verdes ✅

### 2. Verificar no NuGet.org
- Acesse: https://www.nuget.org/packages/FlyGon.CQRS
- A nova versão deve aparecer em alguns minutos
- Pode levar até 15 minutos para indexação completa

### 3. Testar a instalação
```bash
dotnet new console -n TestApp
cd TestApp
dotnet add package FlyGon.CQRS --version 2.0.0
```

## Versionamento Semântico

Siga o [Semantic Versioning](https://semver.org/):

- **MAJOR** (X.0.0): Mudanças incompatíveis na API
  - Exemplo: `1.0.0` → `2.0.0` (upgrade de framework)
  
- **MINOR** (0.X.0): Novas funcionalidades compatíveis
  - Exemplo: `2.0.0` → `2.1.0` (novos métodos)
  
- **PATCH** (0.0.X): Correções de bugs compatíveis
  - Exemplo: `2.0.0` → `2.0.1` (bug fixes)

## Troubleshooting

### Erro: "Package already exists"
- A versão já foi publicada no NuGet
- Incremente a versão no `.csproj` e tente novamente

### Erro: "Invalid API Key"
- Verifique se o secret `NUGET_API_KEY` está configurado corretamente
- A API Key pode ter expirado - gere uma nova no NuGet.org

### Erro: "Tests failed"
- O workflow não publica se os testes falharem
- Corrija os testes e faça push novamente

### Workflow não foi acionado
- Verifique se você criou uma **Release** (não apenas uma tag)
- Ou use o método manual via "Run workflow"

## Comandos Úteis

### Listar tags locais
```bash
git tag
```

### Deletar tag local
```bash
git tag -d v2.0.0
```

### Deletar tag remota
```bash
git push origin --delete v2.0.0
```

### Ver última versão publicada
```bash
dotnet nuget list source https://api.nuget.org/v3/index.json | grep FlyGon.CQRS
```

## Checklist de Release

- [ ] Atualizar versão no `.csproj`
- [ ] Atualizar `PackageReleaseNotes` no `.csproj`
- [ ] Atualizar `README.md` se necessário
- [ ] Fazer commit e push
- [ ] Criar tag (`git tag vX.Y.Z`)
- [ ] Push da tag (`git push origin vX.Y.Z`)
- [ ] Criar Release no GitHub
- [ ] Aguardar GitHub Actions
- [ ] Verificar no NuGet.org
- [ ] Testar instalação do pacote
- [ ] Anunciar a release (se aplicável)

## Suporte

Se tiver problemas, abra uma issue em:
https://github.com/gregojoao/FlyGon.CQRS/issues
