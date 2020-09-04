import { shallowMount } from '@vue/test-utils';
import BaseInput from '@/components/common/form/BaseInput/BaseInput.vue';

describe('BaseInput/index.vue', () => {
  let options;

  beforeEach(() => {
    options = {
      propsData: {
        vuelidate: {
          $touch: jest.fn(),
        },
      },
    };
  });

  it('renders a vue instance', () => {
    expect(shallowMount(BaseInput, options).vm).toBeTruthy();
  });

  it('updates model with prop value', () => {
    const wrapper = shallowMount(BaseInput, options);
    expect(wrapper.vm.model).toBeNull();

    wrapper.setProps({ value: 'some val' });
    expect(wrapper.vm.model).toBe('some val');
  });

  it('emits input changes', () => {
    const wrapper = shallowMount(BaseInput, options);
    wrapper.find('input').setValue('some val');
    expect(wrapper.emitted('input')[0]).toEqual(['some val']);
  });
});
