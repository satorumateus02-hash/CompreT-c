# Planejamento da feature: filtros e busca no módulo de estoque

## 1. Objetivo
Implementar uma busca mais robusta e filtros de consulta no módulo de estoque da aplicação ASP.NET Core MVC, permitindo que o usuário encontre produtos com maior rapidez, reduza a necessidade de navegação manual e mantenha a experiência de uso consistente com o restante do sistema.

## 2. Contexto
A aplicação possui um módulo de estoque com listagem de produtos e cadastro de itens. Atualmente, a busca é limitada e não oferece filtros avançados. A feature tem como objetivo melhorar a usabilidade da tela de listagem, sem quebrar o fluxo existente de cadastro, edição e visualização de produtos.

## 3. Escopo da funcionalidade
### 3.1 Busca textual
- Buscar por nome do produto.
- Buscar também por categoria e fornecedor, conforme a lógica do domínio.
- Tratar entradas vazias e espaços em branco como ausência de filtro.

### 3.2 Filtros adicionais
- Filtro por categoria.
- Filtro por fornecedor.
- Filtro por preço mínimo.
- Filtro por preço máximo.
- Filtro por quantidade mínima em estoque.
- Filtro para mostrar apenas produtos com estoque baixo.

### 3.3 Ordenação
- Ordenar por nome.
- Ordenar por preço.
- Ordenar por data de cadastro.

### 3.4 Persistência da interface
- Os valores informados devem continuar visíveis após a pesquisa.
- O sistema deve indicar quais filtros estão ativos.
- A URL deve refletir os parâmetros da busca quando for necessário.

### 3.5 Reset
- Botão para limpar todos os filtros e retornar à listagem completa.

## 4. Requisitos funcionais
1. O usuário deve conseguir buscar um produto por texto.
2. O sistema deve permitir combinar filtros sem perder o estado da tela.
3. A busca deve funcionar com query string, permitindo filtros e paginação sem quebrar a UX.
4. O backend deve aplicar filtros somente quando os valores forem informados.
5. A view deve manter os campos preenchidos após a execução da pesquisa.
6. O botão de limpar filtros deve remover todos os critérios aplicados.
7. A ordenação deve ser consistente e previsível.

## 5. Requisitos não funcionais
- Código em C# seguindo o padrão do projeto.
- Uso de `AsQueryable()` para montar a query de forma segura e escalável.
- Compatibilidade com Razor Views e Bootstrap.
- Preservação da autenticação e sessão da aplicação.
- Manutenção da lógica atual de estoque e cadastro.
- Sem adição de dependências ou tecnologias novas sem necessidade.

## 6. Arquivos previstos
- `Controllers/EstoqueController.cs`
- `Views/Estoque/index.cshtml`
- `Models/Produto.cs`
- `Models/FiltroProdutoViewModel.cs` (opcional, se a implementação exigir separação da lógica de filtros)

## 7. Estratégia de implementação

### 7.1 Backend
1. Ajustar o método `Index` do `EstoqueController` para receber parâmetros via query string.
2. Iniciar a consulta com `AsQueryable()`.
3. Aplicar filtros condicionalmente usando `Where`.
4. Validar entradas vazias, nulas e conversões numéricas.
5. Ordenar os resultados por um campo configurado.
6. Enviar os dados para a view mantendo os valores dos filtros.

### 7.2 Frontend
1. Criar uma área de filtros na parte superior da listagem.
2. Usar campos de texto, select e inputs numéricos para os filtros.
3. Manter o método HTTP GET para preservar os parâmetros na URL.
4. Exibir os valores e marcar visualmente filtros ativos.
5. Incluir botão de limpar filtros para retornar ao estado inicial.

### 7.3 Estrutura lógica recomendada
- `textoBusca`: pesquisa por nome/categoria/fornecedor
- `categoria`: filtro textual ou seleção
- `fornecedor`: filtro textual ou seleção
- `precoMinimo`: decimal
- `precoMaximo`: decimal
- `quantidadeMinima`: int
- `estoqueBaixo`: bool
- `ordenacao`: enum/string representando o tipo de ordenação

## 8. Fluxo de execução
1. A requisição chega ao controller com os parâmetros da busca.
2. O sistema valida e normaliza os dados recebidos.
3. A query é montada com `AsQueryable()`.
4. Filtros são aplicados em sequência conforme os parâmetros informados.
5. A lista é ordenada.
6. O resultado é retornado para a view.
7. A view renderiza a tabela com os filtros ativos e os valores preenchidos.

## 9. Critérios de aceitação
- A página de estoque aceita filtros e busca por query string.
- A consulta aplica somente os campos preenchidos.
- A listagem renderiza corretamente os produtos resultantes.
- A tela continua funcionando sem regressões em outras partes do sistema.
- O botão de limpar filtros remove todas as condições.
- O projeto compila sem erros após a implementação.

## 10. Plano de tarefas

### Fase 1 — análise e modelagem
- Revisar `EstoqueController` e `Views/Estoque/index.cshtml`.
- Verificar a estrutura da entidade `Produto`.
- Identificar campos disponíveis para filtro e busca.
- Decidir se será necessário um `ViewModel` específico para filtros.

### Fase 2 — implementação no backend
- Ajustar o método de listagem para receber parâmetros.
- Criar a lógica de consulta com `Where` condicional.
- Aplicar ordenação e filtros de preço/quantidade.
- Garantir que entradas vazias não gerem erro.

### Fase 3 — implementação da interface
- Adicionar os campos de busca e filtros na view.
- Ajustar o layout para se manter visualmente consistente.
- Manter o cadastro de produtos e outros elementos visuais intactos.

### Fase 4 — validação final
- Revisar o código em conjunto.
- Executar `dotnet build`.
- Validar que a tela de estoque funciona corretamente com e sem filtros.

## 11. Riscos e cuidados
- Não remover a funcionalidade atual de cadastro e listagem.
- Evitar filtragem que gere erro ao receber valores nulos ou vazios.
- Garantir que filtros e busca funcionem em conjunto, sem conflitos.
- Não alterar o padrão de autenticação e sessão da aplicação.

## 12. Resultado esperado
A feature deve entregar uma listagem de produtos mais eficiente, com busca por texto e filtros específicos, sem interrupção no fluxo atual do sistema. A interface deve ser simples, funcional e manter os valores dos filtros visíveis para o usuário.

## 13. Observação final
Este planejamento foi preparado como referência para a implementação real da feature de filtros e busca no estoque, seguindo o padrão do projeto e as exigências funcionais e técnicas documentadas na especificação do módulo.
