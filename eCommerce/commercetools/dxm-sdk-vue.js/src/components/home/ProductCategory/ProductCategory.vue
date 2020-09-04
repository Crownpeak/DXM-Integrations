<script>
import { CmsComponent, CmsField, CmsFieldTypes } from 'crownpeak-dxm-vuejs-sdk';

export default {
  extends: CmsComponent,
  name: 'ProductCategory',
  props: ['data'],
  data() {
    return {
      title: new CmsField('Title',
        CmsFieldTypes.TEXT,
        this.$props && this.$props.data ? this.$props.data.Title : null),
      image: new CmsField('Image',
        CmsFieldTypes.IMAGE,
        this.$props && this.$props.data ? this.$props.data.Image : null),
      link: new CmsField('Link',
        CmsFieldTypes.TEXT,
        this.$props && this.$props.data ? this.$props.data.Link : null),
      category_slug: new CmsField('Category_Slug',
        CmsFieldTypes.TEXT,
        this.$props && this.$props.data ? this.$props.data.Category_Slug : null),
    };
  },
};
</script>

<template>
  <div
      class="banner-wrap mb-30 wow fadeIn"
      data-wow-duration="1s"
      data-wow-delay="0.2s"
  >
    <div class="banner-img banner-hover banner-zoom">
      <!-- cp-scaffold
        <cp-publish>{link}{category_slug}</cp-publish>
        <a>
      else -->
      <router-link
          :to="{ name: link, params: { categorySlug: category_slug } }"
      >
      <!-- /cp-scaffold -->
        <img :src="image" alt="banner"/>
        <div class="banner-content">
          <h3>{{title}}</h3>
        </div>
      <!-- cp-scaffold
        </a>
      else -->
      </router-link>
      <!-- /cp-scaffold -->
    </div>
  </div>
</template>
