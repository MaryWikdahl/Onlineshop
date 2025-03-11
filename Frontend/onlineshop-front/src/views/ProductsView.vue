<template>
  <div class="align-items-center">
    <h1 v-if="AllProducts">Alla Produkter</h1>
    <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>
    <div v-if="searchResults.length" class="row">
      <div 
        class="col-md-3" 
        v-for="item in searchResults" 
        :key="item.id" 
        @click="itemClicked(false)"
        style="position: relative;"
      >
        <router-link :to="'/product/' + item.id">
          <h3 style="color: var(--mainblue);">{{ '"' + item.name + '"' }}</h3>
          <img class="grid-item d-flex" :src="item.encodedImage" alt="Produktbild">
         
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
  name: "ProdView",
  data() {
    return {
      AllProducts: false,
      products: [],
      searchResults: [],
      errorMessage: "",
      isItemClicked: true, 
    };
  },
  computed: {
    searchQuery() {
      return this.$store.getters.searchQuery; // Hämta sökquery från Vuex
    },
  },
  methods: {
    async getProducts() {
      try {
        const response = await fetch("https://localhost:7131/api/products");
        const data = await response.json();
        this.products = data;
        this.searchResults = data; // Sätt initiala resultat
        this.filterProducts();
      } catch (error) {
        this.errorMessage = "Kunde inte hämta produkter. Försök igen senare.";
      }
    },
    filterProducts() {
      if (this.searchQuery) {
        this.searchResults = this.products.filter((product) =>
          product.name.toLowerCase().startsWith(this.searchQuery.toLowerCase())
        );
        if (this.searchResults.length === 0) {
          this.errorMessage = `Inga produkter matchar sökordet "${this.searchQuery}".`;
        } else {
          this.errorMessage = ""; // Rensa felmeddelandet om resultat hittas
        }
      } else {
        this.AllProducts = true;
        this.searchResults = this.products;
      }
    },
    itemClicked(status) {
      this.$store.dispatch('updateItemClicked', status); // Uppdatera isItemClicked i Vuex
      // console.log('Item clicked:', this.$store.getters.isItemClicked); // För debugging, kontrollera värdet i store

    },
    addToWishlistHandler(item) {
      alert('Produkten är nu tillagd till din wishlist')
      this.$store.dispatch('addToWishlist', item);
    },
  },
  created() {
    this.getProducts(); // Hämta produkter vid skapande
  },
  watch: {
    searchQuery() {
      this.filterProducts(); // Filtrera produkter när searchQuery ändras
    },
  },
};
</script>

<style scoped>.product-container {
  position: relative; /* Gör så att hjärtknappen kan positioneras i förhållande till denna container */
  width: 100%; /* Sätt bredden till 100% för att säkerställa att varje container tar upp hela sin kolumn */
  height: 100%; /* Gör så att höjden blir korrekt för varje produkt */
}

.align-items-center {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.error-message {
  color: red;
  margin-bottom: 1rem;
}

.row {
  min-width: 100%;
  min-height: 100%;
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 1rem;
}

.col-md-3 {
  flex: 0 0 calc(25% - 1rem);
  max-width: calc(25% - 1rem);
  box-sizing: border-box;
}

.grid-item {
  width: 100%; /* Gör så att produktbilderna tar upp hela bredden */
  height: 70%; /* Bildens höjd anpassas till dess bredd */
  border-radius: 5px;
  display: block; /* Gör bilden till ett block-element för bättre kontroll */
}

.heart-button {
  z-index: 10;
  position: absolute; /* Placera hjärtknappen i förhållande till produktcontainern */
  top: 80px; /* Placera hjärtknappen lite från toppen */
  right: 10px; /* Placera hjärtknappen lite från höger */
  background: none;
  color: rgba(0, 0, 0, 0.178);
  border-radius: 50%;
  width: 40px;
  height: 40px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: background-color 0.3s, color 0.3s;
}

.heart-button:hover {
  color: #ff4081; /* Ändra färg när man hovrar över knappen */
}

/* Ikon i hjärtknappen */
.heart-button i {
  font-size: 25px;
}



</style>
