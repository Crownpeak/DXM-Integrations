<script>
import { CmsComponent } from 'crownpeak-dxm-vuejs-sdk';

export default {
  extends: CmsComponent,
  name: 'FooterBit',
};
</script>

<template>
  <div class="service-area pt-70 pb-70">
    <div class="container">
      <div class="row">
        <div class="col-lg-3 col-md-6 col-sm-6">
          <div class="service-wrap mb-30 wow fadeInUp" data-wow-delay="0.1s">
            <div class="service-icon">
              <i class="las la-shipping-fast"></i>
            </div>
            <div class="service-content">
              <h6>Free Worldwide Shipping</h6>
              <p>
                <!-- cp-scaffold
                  On all orders over $100
                else -->
                On all orders over {{
                    $store.state.country === 'DE' ? '€' : '$'
                  }}100
                <!-- /cp-scaffold -->
              </p>
            </div>
          </div>
        </div>
        <div class="col-lg-3 col-md-6 col-sm-6">
          <div class="service-wrap mb-30 wow fadeInUp" data-wow-delay="0.2s">
            <div class="service-icon">
              <i class="las la-coins"></i>
            </div>
            <div class="service-content">
              <h6>30 Days Money Returns</h6>
              <p>30 days money back guarantee</p>
            </div>
          </div>
        </div>
        <div class="col-lg-3 col-md-6 col-sm-6">
          <div class="service-wrap mb-30 wow fadeInUp" data-wow-delay="0.3s">
            <div class="service-icon">
              <i class="las la-headphones"></i>
            </div>
            <div class="service-content">
              <h6>Support 24/7</h6>
              <p>Offered in the country of usage</p>
            </div>
          </div>
        </div>
        <div class="col-lg-3 col-md-6 col-sm-6">
          <div class="service-wrap mb-30 wow fadeInUp" data-wow-delay="0.4s">
            <div class="service-icon">
              <i class="las la-shield-alt"></i>
            </div>
            <div class="service-content">
              <h6>100% Secure Checkout</h6>
              <p>Paypal / Visa / Master Card</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
