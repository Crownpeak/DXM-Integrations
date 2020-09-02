<template>
  <span class="crownpeak-categories-links"></span>
</template>

<script>
export default {

  mounted: function() {
    loadPage("/header-categories-links.html", ".crownpeak-categories-links");
  }
}
</script>