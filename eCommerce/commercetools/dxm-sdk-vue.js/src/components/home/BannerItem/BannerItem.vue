<script>
import { CmsComponent, CmsField, CmsFieldTypes } from 'crownpeak-dxm-vuejs-sdk';

export default {
  extends: CmsComponent,
  name: 'BannerItem',
  props: ['data'],
  data() {
    return {
      title1: new CmsField('Title_1',
        CmsFieldTypes.TEXT,
        this.data ? this.data.Title_1 : null),
      title2: new CmsField('Title_2',
        CmsFieldTypes.TEXT,
        this.data ? this.data.Title_2 : null),
      image: new CmsField('Image',
        CmsFieldTypes.IMAGE,
        this.data ? this.data.Image : null),
      button_text: new CmsField('Button_Text',
        CmsFieldTypes.TEXT,
        this.data ? this.data.Button_Text : null),
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
  <!-- cp-scaffold
    <div
      class="single-slider bg-img slider-height-2 align-items-center custom-d-flex"
      style="background-image:url('{{ image:Src }}');">
    else -->
  <div
      class="single-slider bg-img slider-height-2 align-items-center custom-d-flex"
      :style="{backgroundImage: 'url(' + image + ')'}">
    <!-- /cp-scaffold -->
    <div class="container">
      <div class="row height-100-percent align-items-center">
        <div class="col-lg-12 col-md-12 col-sm-12 col-12">
          <div class="slider-content-8 slider-animated-1 text-center">
            <h3 class="animated">{{ title1 }}</h3>
            <h1 class="animated">{{ title2 }}</h1>
            <div class="slider-btn-1">
              <!-- cp-scaffold
                <cp-publish>{Link}{Category_Slug}</cp-publish>
                <a>{Button_Text}</a>
              else -->
              <router-link
                  :to="{ name: link, params: { categorySlug: category_slug } }"
                  class="animated"
              >{{ button_text }}</router-link>
              <!-- /cp-scaffold -->
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
