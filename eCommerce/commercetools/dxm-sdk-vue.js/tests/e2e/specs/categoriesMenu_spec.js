describe('Categories menu', () => {
  before(() => {
    cy.visit('/');
  });

  it('links to any level category', () => {
    cy.get('[data-test=category-1st-level]', { timeout: Cypress.config('graphqlTimeout') })
      .eq(1)
      .then(($menu) => {
        cy.wrap($menu)
          .find('[data-test=category-1st-level-link]')
          .contains('WOMEN')
          .should('have.attr', 'href', '/products/women');

        cy.wrap($menu).trigger('mouseenter');

        cy.wrap($menu)
          .find('[data-test=category-2nd-level-link]')
          .should('have.attr', 'href', '/products/women-shoes')
          .contains('Shoes');

        cy.wrap($menu)
          .find('[data-test=category-3rd-level-link]')
          .should('have.attr', 'href', '/products/women-shoes-sneakers')
          .contains('Sneakers');
      });
  });
});
