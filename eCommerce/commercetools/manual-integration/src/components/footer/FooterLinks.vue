<template>
  <div class="footer-links">
    <div class="col-sm-2">
      <div class="footer-col">
        <p>
          {{ $t('pleaseWait') }}
        </p>
      </div>
    </div>
  </div>
</template>

<script>

export default {

  mounted: function() {
    loadPage("/footer-links.html", ".footer-links");
  },
};
</script>

<i18n>
en:
  pleaseWait: "Please wait..."
de:
  pleaseWait: "Bitte warten..."
</i18n>
