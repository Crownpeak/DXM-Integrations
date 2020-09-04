<script>
import Vue from 'vue';
import { CmsDynamicPage } from 'crownpeak-dxm-vuejs-sdk';
import TheHeader from '@/components/header/TheHeader/TheHeader.vue';
import TheFooter from '@/components/footer/TheFooter/TheFooter.vue';
import Banner from '@/components/home/Banner/Banner.vue';
import ProductCategory from '@/components/home/ProductCategory/ProductCategory.vue';
import FooterBit from '@/components/home/FooterBit/FooterBit.vue';
import DropZone from '../../crownpeak/DropZone.vue';

Vue.prototype.$cmsAssetId = 276801;

export default {
  extends: CmsDynamicPage,
  name: 'Homepage',
  components: {
    DropZone,
    FooterBit,
    ProductCategory,
    TheFooter,
    TheHeader,
    Banner,
  },
};
</script>

<template>
<span>
    <TheHeader/>
    <span>
      <Banner/>
      <div class="banner-area pt-30">
        <div class="custom-container">
          <div class="row">
            <div class="col-lg-4 col-md-4 col-xs-12">
              <ProductCategory/>
            </div>
            <div class="col-lg-4 col-md-4 col-xs-12">
              <ProductCategory/>
              <DropZone name="productCategoryDZ"/>
            </div>
            <div class="col-lg-4 col-md-4 col-xs-12">
              <ProductCategory/>
            </div>
          </div>
        </div>
      </div>
      <FooterBit/>
    </span>
    <TheFooter/>
  </span>
</template>
