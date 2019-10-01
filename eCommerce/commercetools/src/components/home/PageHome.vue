<template>
  <div class="home-page">
    <p>
      {{ $t('pleaseWait') }}
    </p>
  </div>
</template>

<script>

import BannerPicture from './BannerPicture.vue';

export default {

  mounted: function() {
    loadPage("/home.html", ".home-page");
  },
};
</script>

<i18n>
en:
  pleaseWait: "Please wait..."
de:
  pleaseWait: "Bitte warten..."
</i18n>
