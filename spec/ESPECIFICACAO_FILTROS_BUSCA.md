# Especificação técnica: filtros e busca no estoque

## 1. Visão geral
A funcionalidade deve permitir que o usuário consulte e organize a listagem de produtos com filtros mais eficientes, reduzindo o tempo de busca e tornando a gestão do estoque mais prática.

## 2. Requisitos funcionais
### 2.1 Busca textual
- O usuário pode buscar por nome do produto.
- O sistema deve procurar também por categoria e fornecedor, conforme a lógica do módulo.

### 2.2 Filtros adicionais
- Filtro por categoria
- Filtro por fornecedor
- Filtro por preço mínimo
- Filtro por preço máximo
- Filtro por quantidade mínima em estoque
- Filtro para mostrar apenas produtos com estoque baixo

### 2.3 Ordenação
- Ordenar por nome
- Ordenar por preço
- Ordenar por data de cadastro

### 2.4 Persistência de filtros
- Os valores selecionados devem continuar exibidos após a execução da busca.
- A interface deve indicar ao usuário quais filtros estão ativos.

### 2.5 Reset
- Deve existir um botão para limpar todos os filtros.

## 3. Requisitos não funcionais
- Código em C# seguindo o padrão do projeto
- Uso de LINQ com `AsQueryable()`
- Compatibilidade com a UI atual em Razor
- Manutenção da autenticação e sessão
- Sem quebra de outras áreas do sistema

## 4. Critérios de aceite
- Em `EstoqueController`, os parâmetros de busca e filtros são aceitos via query string.
- A query só aplica filtros quando os valores forem informados.
- A listagem renderiza corretamente os itens com os filtros aplicados.
- A tela de estoque continua funcionando sem regressões.
- A compilação do projeto finaliza com sucesso.

## 5. Arquivos esperados
- `Controllers/EstoqueController.cs`
- `Views/Estoque/index.cshtml`
- Possível `Models/FiltroProdutoViewModel.cs`

## 6. Entrega mínima
Entregar a implementação funcional com documentação de planejamento e validação final.
