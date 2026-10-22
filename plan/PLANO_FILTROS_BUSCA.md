# Plano de implementação: Filtros e busca no módulo de estoque

## 1. Objetivo
Melhorar a listagem de produtos do sistema, permitindo busca e filtros mais completos para facilitar a consulta e a operação de estoque.

## 2. Contexto do projeto
O projeto é uma aplicação ASP.NET Core MVC com autenticação e Entity Framework Core. O módulo de estoque já contém uma busca simples por nome/categoria, mas ainda está limitado.

## 3. Requisitos da feature
- Busca por nome do produto
- Busca por categoria
- Busca por fornecedor
- Filtro por preço mínimo
- Filtro por preço máximo
- Filtro por quantidade mínima
- Filtro para estoque baixo
- Ordenação por nome, preço e data de cadastro
- Manter os filtros visíveis após a pesquisa
- Botão para limpar filtros

## 4. Arquivos envolvidos
- `Controllers/EstoqueController.cs`
- `Views/Estoque/index.cshtml`
- `Models/Produto.cs`
- Possivelmente um `ViewModel` novo para filtros

## 5. Estratégia de desenvolvimento
### Backend
- Ajustar o método `Index` para receber parâmetros de pesquisa e filtros.
- Iniciar a query com `AsQueryable()`.
- Aplicar filtros condicionalmente com `Where`.
- Ordenar a listagem de forma consistente.
- Preservar os valores dos filtros na view com `ViewData`.

### Frontend
- Adicionar campos de busca e filtros na parte superior da tabela.
- Usar o método GET para manter a URL com os parâmetros.
- Incluir botão de limpar para resetar os filtros.
- Manter o layout visual compatível com Bootstrap.

## 6. Fluxo de execução
1. Ler a requisição GET com os parâmetros informados.
2. Validar os dados recebidos.
3. Construir a query de produtos.
4. Aplicar os filtros em sequência.
5. Ordenar os resultados.
6. Enviar a lista para a view.
7. Renderizar os filtros com os valores selecionados.

## 7. Critérios de aceitação
- A página de estoque deve permitir filtro por texto e por campos específicos.
- O usuário consegue combinar filtros sem perder o estado da tela.
- Produtos com estoque baixo podem ser filtrados facilmente.
- A ação de limpar filtros retorna a listagem completa.
- A funcionalidade deve compilar sem erros.

## 8. Validação
- Revisar o controller e a view.
- Compilar o projeto com `dotnet build`.
- Confirmar que a busca e os filtros não quebram o fluxo atual de gestão de produtos.

## 9. Entrega
Entregar a feature pronta, com código funcional, documentação breve do que foi implementado e validação final do projeto.
