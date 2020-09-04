<script>
import { CmsDropZoneComponent } from 'crownpeak-dxm-vuejs-sdk';
import ProductCategory from '@/components/home/ProductCategory/ProductCategory.vue';

export default {
  extends: CmsDropZoneComponent,
  name: 'DropZone',
  props: ['name'],
  render(h) {
    const componentRegistry = {
      ProductCategory,
    };
    return h('div', {}, this.getComponents(h, componentRegistry));
  },
};
</script>
