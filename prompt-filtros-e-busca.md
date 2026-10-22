# Prompt passo a passo para implementar filtros e busca no projeto HelpDeskMvc

Você é um desenvolvedor .NET especializado em ASP.NET Core MVC, C#, Entity Framework Core e Razor Views.

Contexto do projeto:
- Nome do projeto: HelpDeskMvc
- Stack: ASP.NET Core MVC, Identity, EF Core, SQLite
- Estrutura principal: Controllers, Models, Data, Views, wwwroot
- Entidade relevante: Produto
- O módulo de estoque já possui uma busca simples por nome/categoria, mas ainda não tem filtros avançados e busca consistente.

Objetivo:
Implementar a feature de filtros e busca no módulo de estoque, melhorando a experiência de consulta e manutenção de produtos.

Instruções obrigatórias:
1. Analise o projeto antes de alterar qualquer arquivo.
2. Use o padrão existente da aplicação em controllers, models e Razor Views.
3. Preserve a autenticação e a lógica de sessão já implementada.
4. Não remova funcionalidades existentes sem necessidade.
5. Escreva o código em C# e Razor de forma clara, organizada e compatível com o projeto atual.
6. Valide a implementação com build/compilação.

Escopo da feature:
- Busca por nome do produto
- Busca por categoria
- Busca por fornecedor
- Filtro por faixa de preço mínimo e máximo
- Filtro por quantidade mínima em estoque
- Filtro para mostrar apenas produtos com estoque baixo
- Ordenação por nome, preço e data de cadastro
- Preservação dos filtros após a pesquisa
- Interface amigável no Razor View

Requisitos de implementação:
1. Ajustar o controller de estoque para aceitar filtros via query string.
2. Montar uma query LINQ com `AsQueryable()` e aplicar `Where` somente quando os filtros forem preenchidos.
3. Tratar valores vazios, nulos e conversões de forma segura.
4. Usar `ViewData` ou `ViewBag` para manter o valor dos filtros na tela.
5. Atualizar a view de listagem para exibir os campos de filtro e o botão de limpar filtros.
6. Manter o botão de cadastro de novo produto sem quebrar o layout.
7. Garantir que os campos de pesquisa e filtros funcionem em conjunto.
8. Mantenha o padrão visual do projeto usando Bootstrap.
9. Caso seja necessário, criar um `ViewModel` dedicado para filtros.

Estrutura esperada:
- Controller: `EstoqueController`
- View: `Views/Estoque/index.cshtml`
- Model: `Produto` e opcionalmente um ViewModel de filtro
- Banco: sem necessidade de migration nova, a menos que a estrutura do banco exija

Fluxo de trabalho que você deve seguir:
1. Verifique o controller de estoque e a view de listagem.
2. Entenda como o projeto resolve autenticação e sessão.
3. Identifique os campos relevantes da entidade Produto.
4. Proponha a lógica de filtros mais eficiente e compatível com o projeto.
5. Implemente a busca e os filtros no backend.
6. Atualize a interface com campos de pesquisa e filtros visíveis.
7. Adicione um botão para limpar filtros.
8. Faça uma revisão final para garantir que o sistema continue funcionando corretamente.
9. Execute a validação final com `dotnet build` ou a verificação equivalente.

Resultado esperado:
- Código funcional
- Busca avançada implementada
- Filtros persistentes na UI
- Sem erros de compilação
- Explicação curta do que foi alterado

Formato de resposta final:
- Resuma a implementação em tópicos
- Liste os arquivos alterados
- Descreva os filtros e a busca adicionados
- Mostre o comando de validação executado
- Informe se a compilação foi bem-sucedida

Importante:
- Não invente tecnologias novas
- Não apague a funcionalidade atual de cadastro de produtos
- Mantenha a implementação aderente ao padrão do projeto
- Seja objetivo, mas detalhado o suficiente para um desenvolvimento real
