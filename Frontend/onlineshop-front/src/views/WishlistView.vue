<template>
    <div class="wishlist-container">
      <h1>Min Wishlist</h1>
    
      <!-- Visa felmeddelande om något går fel -->
      <div v-if="errorMessage" class="error-message">{{ errorMessage }}</div>
    
      <!-- Lista över produkter i wishlisten -->
      <div v-if="wishlistItems.length > 0" class="wishlist-grid">
        
        <div v-for="product in wishlistItems" :key="product.id" class="wishlist-item">
            <button @click="removeFromWishlist(product.id)" class="remove-btn">

                <i class="fa fa-trash"></i>
            </button>
          <router-link :to="'/product/' + product.id" class="product-link">
           
      
            <img :src="product.encodedImage" alt="Produktbild" class="product-image">
            <div class="product-details">
              <h3 class="product-name">{{ product.name }}</h3>
              <p class="product-price">{{ product.price }} SEK</p>
            </div>
          </router-link>
          <button @click="addToCartHandler(product)" class="add-btn">Add to cart</button>
           </div>
      </div>
    
      <!-- Visa meddelande om wishlisten är tom -->
      <div v-else class="empty-message">
        <p>Din wishlist är tom.</p>
      </div>
    </div>
  </template>
  
  <script>
  import { mapState } from 'vuex';
  
  export default {
    data() {
      return {
        wishlistItems: [],
        errorMessage: '',
      };
    },
  
    computed: {
      ...mapState(['token']),
    },
  
    methods: {
        addToCartHandler(product) {
      this.$store.dispatch('addToCart', product);
    },
      async fetchWishlist() {
        if (!this.token) {
          this.errorMessage = 'Du måste vara inloggad för att visa din wishlist.';
          return;
        }
  
        try {
          const response = await fetch('https://localhost:7131/api/wishlist', {
            method: 'GET',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${this.token}`,
            },
          });
  
          if (!response.ok) {
            throw new Error('Failed to fetch wishlist');
          }
  
          const products = await response.json();
          this.wishlistItems = products;
        } catch (error) {
          this.errorMessage = 'Något gick fel när vi hämtade din wishlist.';
          console.error(error);
        }
      },
  
      async removeFromWishlist(productId) {
        if (!this.token) {
          this.errorMessage = 'Du måste vara inloggad för att ta bort en produkt från wishlist.';
          return;
        }
  
        try {
          const response = await fetch(`https://localhost:7131/api/Wishlist/${productId}`, {
            method: 'DELETE',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${this.token}`,
            },
          });
  
          if (!response.ok) {
            throw new Error('Failed to remove from wishlist');
          }
  
          this.wishlistItems = this.wishlistItems.filter(item => item.id !== productId);
        } catch (error) {
          this.errorMessage = `Något gick fel när vi försökte ta bort produkten från din wishlist: ${error.message}`;
          console.error(error);
        }
      },
    },
  
    created() {
      this.fetchWishlist();
    },
  };
  </script>
  
  <style scoped>
  .wishlist-container {
    max-width: 1200px;
    margin: 0 auto;
    padding: 20px;
  }
  
  h1 {
    font-size: 2rem;
    margin-bottom: 20px;
    color: #333;
  }
  
  .error-message {
    color: red;
    margin-bottom: 1rem;
  }
  
  .wishlist-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(220px, 1fr));
    gap: 20px;
    margin-top: 20px;
  }
  
  .wishlist-item {
    background-color: #fff;
    border-radius: 10px;
    box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    overflow: hidden;
    text-align: center;
    transition: transform 0.3s ease;
  }
  
  .wishlist-item:hover {
    transform: translateY(-5px);
  }
  
  .product-link {
    text-decoration: none;
    color: inherit;
  }
  
  .product-image {
    width: 100%;
    height: 200px;
    object-fit: cover;
    border-bottom: 1px solid #ddd;
  }
  
  .product-details {
    padding: 15px;
  }
  
  .product-name {
    font-size: 1.2rem;
    margin: 0 0 10px 0;
    color: #333;
  }
  
  .product-price {
    font-size: 1.1rem;
    color: #007bff;
  }
  
  .remove-btn {
   
    background-color: transparent;
    border: none;
    outline: none;
    cursor: pointer;
    font-size: 16px;
  }
  
  .remove-btn:hover {

  }
  
  .add-btn{
    background-color: #ff4080c2;
    color: white;
    border: none;
    padding: 10px;
    width: 100%;
    cursor: pointer;
    font-size: 1rem;
    border-radius: 0 0 10px 10px;
    transition: background-color 0.3s;
}
.add-btn:hover {
    background-color:  #ff4081;
  }
  
  .empty-message {
    text-align: center;
    color: #777;
    font-size: 1.2rem;
    margin-top: 50px;
  }
  </style>
  