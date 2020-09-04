import gql from 'graphql-tag';
import { required } from 'vuelidate/lib/validators';
import cartMixin from '../../../mixins/cartMixin';
import BaseRadio from '../../common/form/BaseRadio/BaseRadio.vue';
import BaseMoney from '../../common/BaseMoney/BaseMoney.vue';
import BaseForm from '../../common/form/BaseForm/BaseForm.vue';
import BaseLabel from '../../common/form/BaseLabel/BaseLabel.vue';
import ServerError from '../../common/form/ServerError/ServerError.vue';
import MONEY_FRAGMENT from '../../Money.gql';

export default {
  components: {
    BaseLabel,
    ServerError,
    BaseForm,
    BaseMoney,
    BaseRadio,
  },
  mixins: [cartMixin],
  data: () => ({
    me: null,
    selectedShippingMethod: null,
  }),
  methods: {
    price(shippingMethod) {
      const shippingRate = this.matchingShippingRate(shippingMethod);
      return this.isFree(shippingRate) ? null : shippingRate.price;
    },
    matchingShippingRate(shippingMethod) {
      return this.matchingZoneRate(shippingMethod).shippingRates
        .find((shippingRate) => shippingRate.isMatching);
    },
    matchingZoneRate(shippingMethod) {
      return shippingMethod.zoneRates
        .find((zoneRate) => zoneRate.shippingRates
          .some((shippingRate) => shippingRate.isMatching));
    },
    isFree(shippingRate) {
      const totalPrice = this.me.activeCart.totalPrice.centAmount;
      return totalPrice > shippingRate.freeAbove?.centAmount;
    },
  },
  watch: {
    me(value) {
      this.selectedShippingMethod = value?.activeCart?.shippingInfo?.shippingMethod?.id;
    },
    shippingMethodsByLocation(value) {
      if (!this.selectedShippingMethod) {
        this.selectedShippingMethod = value.find((shippingMethod) => shippingMethod.isDefault)?.id || value[0]?.id;
      }
    },
    selectedShippingMethod() {
      this.updateMyCart([
        {
          setShippingMethod: {
            shippingMethod: {
              typeId: 'shipping-method',
              id: this.selectedShippingMethod,
            },
          },
        },
      ]);
    },
  },
  apollo: {
    me: {
      query: gql`
        query me {
          me {
            activeCart {
              id
              version
              shippingInfo {
                shippingMethod {
                  id
                }
              }
              shippingAddress {
                country
                state
              }
              totalPrice {
                centAmount
                currencyCode
              }
            }
          }
        }`,
    },
    shippingMethodsByLocation: {
      query: gql`
        query checkoutShippingMethods($currency: Currency!, $country: Country!, $state: String) {
          shippingMethodsByLocation(currency: $currency, country: $country, state: $state) {
            id
            name
            description
            isDefault
            zoneRates {
              shippingRates {
                isMatching
                freeAbove {
                  centAmount
                }
                price {
                  ...MoneyFields
                }
              }
            }
          }
        }
        ${MONEY_FRAGMENT}`,
      variables() {
        return {
          currency: this.me.activeCart.totalPrice.currencyCode,
          ...this.me.activeCart.shippingAddress,
        };
      },
      skip() {
        return !this.cartExists;
      },
    },
  },
  validations: {
    form: {
      shippingMethod: { required },
    },
  },
};
