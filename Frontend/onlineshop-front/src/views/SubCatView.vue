<template>
    <div class="align-items-center">

  
      <div v-if="errorMessage" class="error-message">
        {{ errorMessage }}
      </div>
  
      <div class="row">
        <div class="col-md-3" v-for="item in searchResults" :key="item.id">
          <router-link :to="'/product/' + item.id"> 
            <h3 style="color: var(--mainblue);">{{ "\"" + item.name + "\"" }}</h3>
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
    name: 'SubCatView',
    components: {

    },
    data() {
      return {
        products: [],  // Alla produkter från API
        searchQuery: '',
        searchResults: [],  // Filtrerade produkter baserat på subCategoryId och sökfråga
        errorMessage: '',
      };
    },
    methods: {
      addToWishlistHandler(item) {
      alert('Produkten är nu tillagd till din wishlist')
      this.$store.dispatch('addToWishlist', item);
    },
      // Den här metoden hämtar alla produkter
      async getAllProducts() {
        try {
          const response = await fetch('https://localhost:7131/api/products');
          if (!response.ok) {
            throw new Error('Failed to fetch products');
          }
          const data = await response.json();
          console.log('Fetched all products:', data); // Kontrollera om vi får rätt data
          this.products = data;  // Alla produkter sparas här
          this.filterProductsBySubCategory(); // Filtrera produkter baserat på subCategoryId
        } catch (error) {
          console.error('Error fetching products:', error);
          this.errorMessage = 'Kunde inte hämta produkter. Försök igen senare.';
        }
      },
  
      // Filtrera produkter baserat på subCategoryId
      filterProductsBySubCategory() {
        const subCategoryId = this.$route.params.subCategoryId;  // Hämta subCategoryId från URL
        if (subCategoryId) {
          // Filtrera produkter där subCategoryId matchar
          this.searchResults = this.products.filter(product => product.subCategoryId === parseInt(subCategoryId));
          if (this.searchResults.length === 0) {
            this.errorMessage = 'Inga produkter hittades för denna subkategori.';
          }
        } else {
          this.errorMessage = 'Subkategori-ID saknas!';
        }
      },
  
  
    },
  
    created() {
      this.getAllProducts(); // Hämta alla produkter när komponenten laddas
    },
  
  
  
    beforeRouteUpdate(to, from, next) {
    
      console.log('Navigerar till ny subkategori:', to.params.subCategoryId);
      this.getAllProducts(); // Hämta produkterna igen
      next(); // Fortsätt med navigeringen
    },
  };
  </script>
  
  <style>
  /* Layout för produkterna */
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
    display: flex;
    flex-wrap: wrap;
    justify-content: center;
    gap: 1rem;
  }
  
  .col-md-3 {
    flex: 0 0 calc(50% - 1rem);  /* Vi justerar för att få 2 bilder per rad istället för 4 */
    max-width: calc(50% - 1rem);  /* Ökar till 50% för att ge plats åt bredare bilder */
    box-sizing: border-box;
    display: flex;
    flex-direction: column;
    align-items: center;
  }
  
  .grid-item {
    width: 100%;  /* Gör så att bilderna fyller hela kolumnens bredd */
    height: auto;  /* Gör så att bilderna behåller sina proportioner */
    max-height: 500px;  /* Höjdjustering så bilderna inte blir för stora */
    object-fit: contain;  /* Gör så att hela bilden får plats utan att beskäras */
    object-position: center;  /* Gör så att om det finns luft runt bilden så är bilden centrerad */
    border-radius: 5px;  /* Rundade hörn på bilderna */
  }
  
  /* För att göra bilderna bredare */
  img.grid-item {
    width: 100%;  /* Gör bilderna 100% av kolumnens bredd */
    height: auto;
    max-width: 100%;  /* Gör så att bilderna får sin fulla bredd */
    max-height: 500px;  /* Maxhöjd för att förhindra att bilderna blir för stora */
    object-fit: contain;  /* För att bibehålla bildens proportioner utan att beskäras */
  }
  .heart-button {
  position: absolute;
  top: 200px;
  right: 200px;
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
  