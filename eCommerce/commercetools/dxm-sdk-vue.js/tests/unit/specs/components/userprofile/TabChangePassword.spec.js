import Vuelidate from 'vuelidate';
import { shallowMount, createLocalVue } from '@vue/test-utils';
import TabChangePassword from '@/components/useraccount/TabChangePassword/TabChangePassword.vue';

jest.mock('@/auth', () => ({ clientLogin: jest.fn() }));

const localVue = createLocalVue();
localVue.use(Vuelidate);

describe('TabChangePassword/index.vue', () => {
  let options;

  beforeEach(() => {
    options = {
      localVue,
      mocks: { $t: jest.fn() },
      // overriding methods is depricated
      // methods: { customerSignMeUp: jest.fn() },
    };
  });

  it('renders a vue instance', () => {
    expect(shallowMount(TabChangePassword, options).vm).toBeTruthy();
  });
});
