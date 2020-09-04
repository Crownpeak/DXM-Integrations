<script>
import { CmsComponent, CmsField, CmsDataCache } from 'crownpeak-dxm-vuejs-sdk';
import carousel from 'vue-owl-carousel';
import BannerItem from '@/components/home/BannerItem/BannerItem.vue';

export default {
  extends: CmsComponent,
  name: 'Banner',
  components: {
    carousel,
    BannerItem,
  },
  props: ['data'],
  data() {
    return {
      bannerItems: new CmsField('BannerItem', '', CmsDataCache.get(CmsDataCache.cmsAssetId).Banner),
    };
  },
};
</script>

<template>
  <div class="slider-area">
    <carousel
      animateIn="fadeIn"
      animateOut="fadeOut"
      :autoplayHoverPause="false"
      :nav="false"
      :autoplay="true"
      :items="1"
      class="slider-active-1 nav-style-2"
    >
      <!-- <List name="BannerItems" type="BannerItem"> -->
      <BannerItem v-for="(bi, i) in bannerItems.value" :key="i" :data="bi.BannerItem" />
      <!-- </List> -->
    </carousel>
  </div>
</template>
