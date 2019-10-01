<template>
  <span class="crownpeak-links"></span>
</template>

<script>
export default {

  mounted: function() {
    loadPage("/header-links.html", ".crownpeak-links");
  }
}
</script>