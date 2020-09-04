<i18n src="./TheHeader.txt"></i18n>
<script>
import { CmsComponent } from 'crownpeak-dxm-vuejs-sdk';
import CategoriesMenu from '../CategoriesMenu/CategoriesMenu.vue';
import LoginButton from '../LoginButton/LoginButton.vue';
import LocationSelector from '../LocationSelector/LocationSelector.vue';
import cartMixin from '../../../mixins/cartMixin';

export default {
  extends: CmsComponent,
  name: 'TheHeader',
  components: {
    CategoriesMenu,
    LoginButton,
    LocationSelector,
  },
  data() {
    return {
      searchText: this.$route.query.q || '',
      mobileMenuOpen: false,
      searchOpen: false,
    };
  },
  mixins: [cartMixin],
  computed: {
    totalCartItems() {
      return this.$store.state.cartItems;
    },
  },
  methods: {
    toggleSearch() {
      this.searchOpen = !this.searchOpen;
    },
    search() {
      this.toggleSearch();
      const {
        query,
      } = this.$route;
      this.$router.push({
        name: 'products',
        params: {
          categorySlug: 'all',
          page: 1,
        },
        query: {
          ...query,
          q: this.searchText,
        },
      });
    },
    toggleMobileMenu() {
      this.mobileMenuOpen = !this.mobileMenuOpen;
    },
    onToggleMinicart() {
      this.$store.dispatch('toggleMiniCart');
    },
    openMiniCart() {
      this.$store.dispatch('openMiniCart', 0);
    },
  },
  watch: {
    $route(to) {
      this.searchText = to.query.q || '';
    },
  },
};

</script>

<template>
  <header class="header-area">
    <div class="main-header-wrap bg-gray">
      <div class="custom-container">
        <div class="header-top pt-10 pb-10">
          <div class="row align-items-center">
            <div class="col-sm-6">
              <div class="header-info header-info-inc">
                <a
                    href="#"
                    data-test="stores-link"
                >
                  <!-- cp-scaffold
                  Stores
                  else -->
                  {{ $t('stores') }}
                  <!-- /cp-scaffold -->
                </a>
                <a href="#">HELP</a>
              </div>
            </div>
            <div class="col-sm-6 d-flex justify-content-end">
              <div class="curr-lang-wrap curr-lang-inc">
                <ul>
                  <!-- cp-scaffold else -->
                  <LocationSelector
                      v-bind:values="$sunrise.countries"
                      title="location"
                      data-test="country-selector-dropdown"
                  />
                  <LocationSelector
                      v-bind:values="$sunrise.languages"
                      title="language"
                      data-test="language-selector-dropdown"
                  />
                  <!-- /cp-scaffold -->
                </ul>
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="site-header-outer">
        <div class="intelligent-header bg-white">
          <div class="header-middle">
            <div class="custom-container">
              <div class="row align-items-center">
                <div class="col-xl-2 col-lg-3">
                  <div class="logo">
                    <router-link to="/">
                      <img
                          src="https://s3.surety.financial.cprd.io/Skunks-Works/commercetools-Vue-SDK/_Assets/images/logo.svg"
                          alt="SUNRISE"
                          class="img-responsive sunrise-logo"
                      />
                    </router-link>
                  </div>
                </div>
                <div class="col-xl-8 col-lg-6 position-static">
                  <div
                      class="main-menu menu-lh-3 main-menu-blod main-menu-center"
                  >
                    <!-- cp-scaffold
                      <nav><ul><li data-test="category-1st-level" class="position-static"><a href="/products/new" class="" data-test="category-1st-level-link"> NEW </a><ul class="mega-menu menu-2-col mega-menu-width3"><li><ul class="mega-menu-width4"><li><a href="/products/new-women" class="menu-title" data-test="category-2nd-level-link"><span>Women</span></a><ul><li><a href="/products/new-women-clothing" class="" data-test="category-3rd-level-link"><span>Clothing</span></a></li><li><a href="/products/new-women-shoes" class="" data-test="category-3rd-level-link"><span>Shoes</span></a></li><li><a href="/products/new-women-bags" class="" data-test="category-3rd-level-link"><span>Bags</span></a></li></ul></li><li><a href="/products/new-men" class="menu-title" data-test="category-2nd-level-link"><span>Men</span></a><ul><li><a href="/products/new-men-clothing" class="" data-test="category-3rd-level-link"><span>Clothing</span></a></li><li><a href="/products/new-men-shoes" class="" data-test="category-3rd-level-link"><span>Shoes</span></a></li></ul></li></ul></li></ul></li><li data-test="category-1st-level" class="position-static"><a href="/products/women" class="" data-test="category-1st-level-link"> WOMEN </a><ul class="mega-menu mega-menu-width3"><li><ul class="mega-menu-width4"><li><a href="/products/women-clothing" class="menu-title" data-test="category-2nd-level-link"><span>Clothing</span></a><ul><li><a href="/products/women-clothing-jackets" class="" data-test="category-3rd-level-link"><span>Jackets</span></a></li><li><a href="/products/women-clothing-blazer" class="" data-test="category-3rd-level-link"><span>Blazer</span></a></li><li><a href="/products/women-clothing-tops" class="" data-test="category-3rd-level-link"><span>Tops</span></a></li><li><a href="/products/women-clothing-shirts" class="" data-test="category-3rd-level-link"><span>Shirts</span></a></li><li><a href="/products/women-clothing-t-shirts" class="" data-test="category-3rd-level-link"><span>T-shirts</span></a></li><li><a href="/products/women-clothing-jeans" class="" data-test="category-3rd-level-link"><span>Jeans</span></a></li><li><a href="/products/women-clothing-trouser" class="" data-test="category-3rd-level-link"><span>Trouser</span></a></li><li><a href="/products/women-clothing-skirts" class="" data-test="category-3rd-level-link"><span>Skirts</span></a></li><li><a href="/products/women-clothing-dresses" class="" data-test="category-3rd-level-link"><span>Dresses</span></a></li></ul></li><li><a href="/products/women-shoes" class="menu-title" data-test="category-2nd-level-link"><span>Shoes</span></a><ul><li><a href="/products/women-shoes-sneakers" class="" data-test="category-3rd-level-link"><span>Sneakers</span></a></li><li><a href="/products/women-shoes-boots" class="" data-test="category-3rd-level-link"><span>Boots</span></a></li><li><a href="/products/women-shoes-ankle-boots" class="" data-test="category-3rd-level-link"><span>Ankle boots</span></a></li><li><a href="/products/women-shoes-pumps" class="" data-test="category-3rd-level-link"><span>Pumps</span></a></li><li><a href="/products/women-shoes-ballerinas" class="" data-test="category-3rd-level-link"><span>Ballerinas</span></a></li><li><a href="/products/women-shoes-loafers" class="" data-test="category-3rd-level-link"><span>Loafers</span></a></li><li><a href="/products/women-shoes-sandals" class="" data-test="category-3rd-level-link"><span>Sandals</span></a></li></ul></li><li><a href="/products/women-bags" class="menu-title" data-test="category-2nd-level-link"><span>Bags</span></a><ul><li><a href="/products/women-bags-clutches" class="" data-test="category-3rd-level-link"><span>Clutches</span></a></li><li><a href="/products/women-bags-shoulder-bags" class="" data-test="category-3rd-level-link"><span>Shoulder bags</span></a></li><li><a href="/products/women-bags-shopper" class="" data-test="category-3rd-level-link"><span>Shopper</span></a></li><li><a href="/products/women-bags-handbag" class="" data-test="category-3rd-level-link"><span>Handbag</span></a></li><li><a href="/products/women-bags-wallets" class="" data-test="category-3rd-level-link"><span>Wallets</span></a></li></ul></li></ul></li></ul></li><li data-test="category-1st-level" class="position-static"><a href="/products/men" class="" data-test="category-1st-level-link"> MEN </a><ul class="mega-menu menu-2-col mega-menu-width3"><li><ul class="mega-menu-width4"><li><a href="/products/men-clothing" class="menu-title" data-test="category-2nd-level-link"><span>Clothing</span></a><ul><li><a href="/products/men-clothing-jackets" class="" data-test="category-3rd-level-link"><span>Jackets</span></a></li><li><a href="/products/men-clothing-tops" class="" data-test="category-3rd-level-link"><span>Tops</span></a></li><li><a href="/products/men-clothing-shirts" class="" data-test="category-3rd-level-link"><span>Shirts</span></a></li><li><a href="/products/men-clothing-trousers" class="" data-test="category-3rd-level-link"><span>Trousers</span></a></li><li><a href="/products/men-clothing-jeans" class="" data-test="category-3rd-level-link"><span>Jeans</span></a></li><li><a href="/products/men-clothing-blazer" class="" data-test="category-3rd-level-link"><span>Blazer</span></a></li><li><a href="/products/men-clothing-suits" class="" data-test="category-3rd-level-link"><span>Suits</span></a></li><li><a href="/products/men-clothing-t-shirts" class="" data-test="category-3rd-level-link"><span>T-shirts</span></a></li></ul></li><li><a href="/products/men-shoes" class="menu-title" data-test="category-2nd-level-link"><span>Shoes</span></a><ul><li><a href="/products/men-shoes-sneakers" class="" data-test="category-3rd-level-link"><span>Sneakers</span></a></li><li><a href="/products/men-shoes-boots" class="" data-test="category-3rd-level-link"><span>Boots</span></a></li><li><a href="/products/men-shoes-lace-up-shoes" class="" data-test="category-3rd-level-link"><span>Lace-up shoes</span></a></li><li><a href="/products/men-shoes-loafers" class="" data-test="category-3rd-level-link"><span>Loafers</span></a></li><li><a href="/products/men-shoes-sandals" class="" data-test="category-3rd-level-link"><span>Sandals</span></a></li></ul></li></ul></li></ul></li><li data-test="category-1st-level" class="position-static"><a href="/products/accessories" class="" data-test="category-1st-level-link"> ACCESSORIES </a><ul class="mega-menu menu-2-col mega-menu-width3"><li><ul class="mega-menu-width4"><li><a href="/products/accessories-women" class="menu-title" data-test="category-2nd-level-link"><span>Women</span></a><ul><li><a href="/products/accessories-women-clothing" class="" data-test="category-3rd-level-link"><span>Clothing</span></a></li><li><a href="/products/accessories-women-looks" class="" data-test="category-3rd-level-link"><span>Looks</span></a></li><li><a href="/products/accessories-women-parfums" class="" data-test="category-3rd-level-link"><span>Parfums</span></a></li><li><a href="/products/accessories-women-sunglasses" class="" data-test="category-3rd-level-link"><span>Sunglasses</span></a></li></ul></li><li><a href="/products/accessories-men" class="menu-title" data-test="category-2nd-level-link"><span>Men</span></a><ul><li><a href="/products/accessories-men-clothing" class="" data-test="category-3rd-level-link"><span>Clothing</span></a></li><li><a href="/products/accessories-men-parfums" class="" data-test="category-3rd-level-link"><span>Parfums</span></a></li></ul></li></ul></li></ul></li><li data-test="category-1st-level" class="position-static"><a href="/products/sale" class="" data-test="category-1st-level-link"> SALE </a><ul class="mega-menu menu-2-col mega-menu-width3"><li><ul class="mega-menu-width4"><li><a href="/products/sale-women" class="menu-title" data-test="category-2nd-level-link"><span>Women</span></a><ul><li><a href="/products/sale-women-clothing" class="" data-test="category-3rd-level-link"><span>Clothing</span></a></li><li><a href="/products/sale-women-shoes" class="" data-test="category-3rd-level-link"><span>Shoes</span></a></li></ul></li><li><a href="/products/sale-men" class="menu-title" data-test="category-2nd-level-link"><span>Men</span></a><ul><li><a href="/products/sale-men-clothing" class="" data-test="category-3rd-level-link"><span>Clothing</span></a></li><li><a href="/products/sale-men-shoes" class="" data-test="category-3rd-level-link"><span>Shoes</span></a></li></ul></li></ul></li></ul></li></ul></nav>
                    else -->
                    <CategoriesMenu/>
                    <!-- /cp-scaffold -->
                  </div>
                </div>
                <!-- cp-scaffold else -->
                <div class="col-xl-2 col-lg-3">
                  <div class="header-component-wrap">
                    <div class="header-search-2 component-same">
                      <a
                          href
                          @click.prevent="toggleSearch"
                          class="search-active"
                      >
                        <i class="dl-icon-search10"></i>
                      </a>
                    </div>
                    <LoginButton/>
                    <div class="cart-wrap component-same ml-10">
                      <a
                          href
                          @click.prevent="openMiniCart"
                          data-test="mini-cart-open-button"
                          class="cart-active"
                      >
                        <i class="dl-icon-cart1"></i>
                        <!-- cp-scaffold else -->
                        <span class="count-style">{{ totalCartItems }} </span>
                        <!-- /cp-scaffold -->
                      </a>
                    </div>
                  </div>
                </div>
                <!-- /cp-scaffold -->
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="header-small-mobile">
      <div class="container">
        <div class="row align-items-center">
          <div class="col-6">
            <div class="mobile-logo logo-width">
              <a href="index.html">
                <img alt="" src="https://s3.surety.financial.cprd.io/Skunks-Works/commercetools-Vue-SDK/_Assets/images/logo.svg"/>
              </a>
            </div>
          </div>
          <div class="col-6">
            <div class="mobile-header-right-wrap">
              <div class="same-style cart-wrap">
                <a href="#" class="cart-active">
                  <i class="dl-icon-cart1 "></i>
                  <span class="count-style">02</span>
                </a>
              </div>
              <div class="mobile-off-canvas">
                <a class="mobile-aside-button" href="#"
                ><i class="dl-icon-menu2"></i
                ></a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

  </header>
</template>
