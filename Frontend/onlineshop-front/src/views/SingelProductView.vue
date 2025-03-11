<template>
  <div>
    <h1>{{ product.name }}</h1>

    <!-- Visa felmeddelande om något går fel -->
    <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>

    <!-- Produktinformation -->
    <div class="row">
      <div class="col-md-3">
        <div class="product-container">
          <router-link :to="'/product/' + product.id">
            <h3 style="color: var(--mainblue);">{{ "\"" + product.name + "\"" }}</h3>
          </router-link>

          <!-- Bild och hjärtknapp -->
          <div class="image-container">
            <img class="grid-item d-flex" :src="product.encodedImage" alt="Produktbild">
            <button class="heart-button" @click="addToWishlistHandler">
              <i class="fa fa-heart-o"></i>
            </button>
          </div>

          <h4 style="color: var(--mainblue);">{{ product.description }}</h4>
          <p style="color: rgba(0, 0, 0, 0.344);">{{ product.price }} SEK</p>

          <!-- Dropdown för storlek -->
          <label for="size-select">Välj storlek:</label>
          <select id="size-select" v-model="selectedSize"  >
            <option disabled value="">Välj en storlek</option>
            <option 
             v-for="info in product.productInfo" 
            :key="info.size" 
            :value="info.size"
            :class="{'out-of-stock-size': info.isOutOfStock}"
            >
             {{ info.size }}
            </option>
          </select>

          <!-- Lagersaldo -->
          <div v-if="selectedSize">
            <span>
              <i class="fa fa-home" :class="stockQuantityInfo().className"></i> {{ stockQuantityInfo().message }}
            </span>
          </div>

          <!-- Knapp för att lägga till i varukorgen -->
          <button :disabled="!selectedSize || stockQuantity <= 0" class="cartBtn" @click="addToCartHandler">
            Lägg till i varukorgen
          </button>

          <!-- Recensioner -->
          <button class="ReviewBtn" @click="openModal">
            <i v-for="n in 5" :key="n" :class="n <= totalRat ? 'fa fa-star' : 'fa fa-star-o'"></i>
            <span>{{ ' ' + this.localReviews.length }} Betyg</span>
          </button>

          <div v-if="reviewMode">
            <reviewComp :reviews="localReviews" :idProp="idProp" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>



<script>
import reviewComp from  '../components/ReviewComp.vue'
export default {
  name: 'SingleProductView',
  components:{
    reviewComp,
  },
  data() {
    return {
      product: {},  // Produktdata
      errorMessage: '',  // Felmeddelande
      reviewMode: false,
      idProp:this.$route.params.id,
      totalRat: 0,
      localReviews: [],
      selectedSize: '',
      stockQuantity: 0, 
    };
  },
  computed: {

 
},

  methods: {
    isOutOfStock() {
      this.product.productInfo.forEach(info => {
      info.isOutOfStock = info.stockQuantity === 0; // Lägg till en isOutOfStock flagga för varje storlek
    }); 
  },
    
stockQuantityInfo() {
  const stockQuantity = this.stockQuantity;

  switch (true) {
    case stockQuantity <= 0:
      return {
        message: "Produkten är slut i lager.",
        className: "out-of-stock", 
      };

    case stockQuantity > 0 && stockQuantity <= 5:
      return {
        message: "Mindre än 5 i lagersaldo!",
        className: "low-stock", 
      };

    case stockQuantity > 5:
      return {
        message: "Produkten finns i lager!",
        className: "in-stock",
      };

    default:
      return {
        message: "Okänt lagersaldo.",
        className: "unknown-stock", // Klass för styling
      };
  }
},

    averageRating() {
  if (this.localReviews.length === 0) return -1; // Om inga recensioner finns, returnera -1 (för att inte visa medelbetyget)

  const totalRating = this.localReviews.reduce((sum, review) => sum + review.rating, 0);
  this.totalRat = totalRating / this.localReviews.length;

    },
   
    openModal(){
this.reviewMode = !this.reviewMode

    },
    // Hämta produktdata från API baserat på produktens ID
    async getProduct() {
  try {
    const response = await fetch(`https://localhost:7131/api/products/${this.$route.params.id}`);
    if (!response.ok) throw new Error('Failed to fetch product');
    const data = await response.json();



    this.product = data; // Sätt produktdata i state
    
    this.sortSize(this.product);

    this.isOutOfStock();
 
  } catch (error) {
    console.error(error);
    this.errorMessage = 'Kunde inte hämta produkten.'; // Visa felmeddelande
  }
},
 sortSize(data){
  console.log('Sorted Product Info:', this.product.productInfo); // Kontrollera att det är sorterat


    if (data.productInfo && Array.isArray(data.productInfo)) {
    
      data.productInfo.sort((a, b) => a.size.localeCompare(b.size));
    }

 },

    async fetchReviews() {
        try {
          const response = await fetch(`https://localhost:7131/api/Reviews/product/${this.idProp}`);
          if (!response.ok) {
            throw new Error("Failed to fetch reviews.");
          }
          const data = await response.json();
          this.localReviews = data;
         this.averageRating();

        
        } catch (error) {
          this.errorMessage = "Kunde inte hämta recensionerna.";
        } finally {
          this.isLoading = false;
         
        }
      },

    // Hantera klick på hjärtknappen för att lägga till i wishlist
    addToWishlistHandler() {
      alert('Produkten är nu tillagd till din wishlist')
      this.$store.dispatch('addToWishlist', this.product);
    },

    // Hantera klick på varukorgs-knappen
    addToCartHandler() {
  if (!this.selectedSize) {
    alert("Välj en storlek innan du lägger till produkten i varukorgen.");
    return;
  }

  // Hitta den valda varianten (storleken) för att hämta korrekt stockQuantity
  const selectedInfo = this.product.productInfo.find(info => info.size === this.selectedSize);
  const stockQuantity = selectedInfo ? selectedInfo.stockQuantity : 0; // Hämta stockQuantity för vald storlek

  // Skicka produkt och stockQuantity till Vuex för att lägga till i varukorgen
  this.$store.dispatch('addToCart', { 
    ...this.product, 
    selectedSize: this.selectedSize,
    stockQuantity: stockQuantity // Lägg till stockQuantity
  });
},
updateStockQuantity() {
    const selectedInfo = this.product.productInfo.find(info => info.size === this.selectedSize);
    if (selectedInfo) {
      this.stockQuantity = selectedInfo.stockQuantity;  // Uppdatera stockQuantity för vald storlek
    } else {
      this.stockQuantity = 0;  // Om ingen storlek är vald, sätt stockQuantity till 0
    }
  },
},
  
  created() {
    // Hämta produktinformation när komponenten skapas
    this.getProduct();
    this.fetchReviews();
    this.stockQuantityInfo()
 
  },
  watch: {
  selectedSize() {
    // När selectedSize ändras, kör updateStockQuantity
    this.updateStockQuantity();
  },
},


};
</script>



<style scoped>
.out-of-stock-size {
  color: #b0b0b0;  /* Ljusgrå färg för texten */
  text-decoration: line-through; /* Valfritt: Gör texten genomstruken om den är slut i lager */
}
.in-stock{
  color: rgb(16, 212, 16);
}
.low-stock{
  color: rgb(209, 209, 19);
}
.out-of-stock{
  color: rgb(197, 14, 14);
}
.cartBtn:disabled {
  background-color: #ccc;
  cursor: not-allowed;
  opacity: 0.6;
}
.cartBtn:disabled:hover {
  background-color: #ccc;
}
/* Felmeddelande stil */
.error-message {
  color: red;
  margin-bottom: 1rem;
}

/* Flexbox layout för produktinformation */
.row {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 1rem;
}

/* Kolumninställningar för produktdetaljer */
.col-md-3 {
  flex: 0 0 calc(25% - 1rem);
  max-width: calc(25% - 1rem);
  box-sizing: border-box;
}

/* Produktcontainer */
.product-container {
  position: relative;
}

/* Bildens container */
.image-container {
  position: relative;
}

/* Produktbildens stil */
.grid-item {
  border: 1px solid rgba(0, 0, 0, 0.048);
  max-width: 100%;
  border-radius: 5px;
}

/* Knappen som hjärta */
.heart-button {
  position: absolute;
  top: 0px;
  right: 30px;
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
  background: none;
  color: #ff4081;
}

/* Ikon i knappen */
.heart-button i {
  font-size: 20px;
}

/* Button stil */
.cartBtn{
  background-color: var(--mainblue);
  color: white;
  border: none;
  padding: 10px;
  cursor: pointer;
  border-radius: 5px;
  margin-top: 1rem;
  width: 100%;
}

.cartBtn:hover {
  background-color: darkblue;
}

.ReviewBtn{
  color:rgb(94, 93, 93);
  font-size: 16px;
  background-color: transparent;
  border: none;
  outline: none;
  cursor: pointer;
}
.ReviewBtn:hover{
  color: #ff4081;
}
</style>
