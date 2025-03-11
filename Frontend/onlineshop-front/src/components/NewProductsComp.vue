<template>
  <div class="NewProdukt">
    <!-- Felmeddelande om vi inte kan hämta produkterna -->
    <div v-if="errorMessage" class="error-message">
      {{ errorMessage }}
    </div>

    <!-- Om det finns färre än 5 produkter -->
    <div v-if="searchResults.length < 5" class="row">
      <div class="col-md-3" v-for="item in searchResults" :key="item.id" style="position: relative;">
        <router-link :to="'/product/' + item.id">
          <img class="grid-item d-flex" :src="item.encodedImage" alt="Produktbild">
          <h3 style="color: var(--mainblue);">{{ "\"" + item.name + "\"" }}</h3>
          <p style="color: rgba(0, 0, 0, 0.344);">{{ item.price + " SEK" }}</p>
        </router-link>
        <button class="heart-button" @click="addToWishlistHandler(item)">
            <i class="fa fa-heart-o"></i>
          </button>
      </div>
    </div>

    <!-- Om det finns fler än 5 produkter, visa de 4 senaste -->
    <div v-else class="row">
      <div class="col-md-3" v-for="item in fourProducts" :key="item.id" style="position: relative;">
        <router-link :to="'/product/' + item.id">
          <img class="grid-item d-flex" :src="item.encodedImage" alt="Produktbild">
          
          <h3 style="color: var(--mainblue);">{{ "\"" + item.name + "\"" }}</h3>
          <p style="color: rgba(0, 0, 0, 0.344);">{{ item.price + " SEK" }}</p>
        </router-link>
        <button class="heart-button" @click="addToWishlistHandler(item)">
            <i class="fa fa-heart-o"></i>
          </button>
      </div>
    </div>
  </div>
</template>

<script>


export default {
  name: 'ProductsView',
  data() {
    return {
      products: [],
      searchResults: [],
      errorMessage: '',
      fourProducts: [],
    };
  },
  methods: {
    
    async getProducts() {
      try {
        const response = await fetch('https://localhost:7131/api/products');
        if (!response.ok) {
          throw new Error('Failed to fetch products');
        }
        const data = await response.json();
        console.log('Fetched products:', data);
        this.products = data;
        this.searchResults = data;
        this.filterFourProducts();
      } catch (error) {
        console.error('Error fetching products:', error);
        this.errorMessage = 'Kunde inte hämta produkter. Försök igen senare.';
      }
    },
    filterFourProducts() {
  const filteredProducts = this.searchResults.filter(product => product.productInfo.some(pi => pi.stockQuantity > 0));

  this.fourProducts = filteredProducts.slice(-4);

  // Kontrollera i konsolen vilka produkter som valdes
  console.log('Detta loggas:', this.fourProducts);
},
    addToWishlistHandler(item) {
      alert('Produkten är nu tillagd till din wishlist')
      this.$store.dispatch('addToWishlist', item);
    },
  },
  created() {
    this.getProducts();
  },
  watch: {
    searchResults: {
      handler() {
        if (this.searchResults.length <= 5) {
          this.filterFourProducts();
        }
      },
      immediate: true,
    },
  },
};
</script>

<style scoped>
.NewProdukt {

  background-color: #ffffff;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.error-message {
  color: red;
  margin-bottom: 1rem;
}

.row {
  background-color: #fefaf9;
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 1.3rem;
  padding-top: 1rem;
  padding-bottom: 1rem;
  padding-left: 0;
}
h3{
  padding-top: 1rem;
}
.col-md-3 {
  position: relative;
  padding: 1rem;
  background-color: rgb(255, 255, 255);
  flex: 0 0 calc(25% - 1rem);
  max-width: calc(25% - 1rem);
  box-sizing: border-box;
  align-items: center;
  padding-left: 0.3rem;
 
}

.grid-item {
  align-items: center;
  background-color: rgba(223, 223, 223, 0.048);
  padding-top: 0.3rem;
  padding-bottom: 0.3rem;
  border: 1px solid rgba(223, 223, 223, 0.048);
  max-width: 100%;
  min-width: 100%;
  height: 70%;
  min-height: 70%;
  border-radius: 5px;
  object-fit: cover;
}
img{
  height: 100%;
  width: 100%;
  min-height: 100%;
  min-width: 100%;
}

.heart-button {
  position: absolute;
  top: 20px;
  right: 20px;
  background: none;
  border: none;
  color: rgba(0, 0, 0, 0.5);
  cursor: pointer;
  font-size: 1.5rem;
  transition: color 0.3s ease;
}
.heart-button:hover {
  color: #ff4081; /* Ändra färg när man hovrar över knappen */
}

.heart-button i {
  font-size: 25px;
}


</style>
