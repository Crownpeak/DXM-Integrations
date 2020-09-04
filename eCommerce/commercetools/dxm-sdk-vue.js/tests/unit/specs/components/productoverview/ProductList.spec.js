import Vuex from 'vuex';
import { shallowMount, createLocalVue } from '@vue/test-utils';
import ProductList from '@/components/productoverview/ProductList/ProductList.vue';
import ProductThumbnail from '@/components/common/ProductThumbnail/ProductThumbnail.vue';
// tests/unit/specs/components/productoverview/
jest.mock('../../../../../src/api', () => ({
  products: {
    get: () => Promise.resolve({ results: [], total: 0 }),
    paramsFromComponent: () => false,
  },
  onlyLastRequestedPromise: () => (y) => y,
}));
const localVue = createLocalVue();

localVue.use(Vuex);

describe('ProductList/index.vue', () => {
  let options;

  beforeEach(() => {
    options = {
      localVue,
      mocks: {
        $t: jest.fn(),
        $route: {
          query: {},
          params: {
            page: 1,
          },
        },
      },
      store: new Vuex.Store({
        state: {
          locale: 'en',
          currency: 'EUR',
          country: 'DE',
        },
      }),
      computed: {
        isLoading: jest.fn(),
      },
      directives: {
        'scroll-to': jest.fn(),
        vpshow: jest.fn(),
      },
    };
  });

  it('renders a vue instance', () => {
    expect(shallowMount(ProductList, options).vm).toBeTruthy();
  });

  it('obtains category information', () => {
    const wrapper = shallowMount(ProductList, options);
    wrapper.setData({
      categories: {
        results: [],
      },
    });
    expect(wrapper.vm.category).toBeUndefined();

    wrapper.setData({
      categories: {
        results: [{ id: 'category-id-1' }],
      },
    });
    expect(wrapper.vm.category).toEqual({ id: 'category-id-1' });
  });

  it('calls ProductThumbnail for each obtained product', async () => {
    const wrapper = shallowMount(ProductList, options);
    wrapper.setData({
      categories: {
        results: [{ id: 'category-id-1' }],
      },
      products: {
        results: [],
      },
    });
    await wrapper.vm.$nextTick();
    expect(wrapper.vm.ProductThumbnail).toBeUndefined();

    wrapper.setData({
      categories: {
        results: [{ id: 'category-id-1' }],
      },
      products: {
        results: [
          { id: 'product-id-1' },
          { id: 'product-id-2' },
          { id: 'product-id-3' },
        ],
      },
    });
    await wrapper.vm.$nextTick();
    const thumbnails = wrapper.findAllComponents(ProductThumbnail);

    expect(thumbnails.length).toBe(3);
    expect(thumbnails.at(0).props().product.id).toEqual('product-id-1');
    expect(thumbnails.at(1).props().product.id).toEqual('product-id-2');
    expect(thumbnails.at(2).props().product.id).toEqual('product-id-3');
  });
});
