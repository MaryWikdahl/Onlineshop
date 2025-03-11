<template>
    <div class="orders-view">
      <h1>Your Orders</h1>
  
      <!-- Loading indicator -->
      <div v-if="isLoading" class="loading">
        Loading your orders...
      </div>
  
      <!-- Error message -->
      <div v-if="errorMessage" class="error-message">
        {{ errorMessage }}
      </div>
  
      <!-- Orders list -->
      <div v-if="orders.length > 0">
        <ul>
          
          <li v-for="order in orders" :key="order.id" class="order-item">
            <p>Status: {{ order.status}}</p>
            <p @click="toggleOrderDetails(order.id)" class="order-id">
              Order ID: {{ order.id }}
            </p>
  
            <!-- Expandable order details -->
            <div v-if="expandedOrder === order.id" class="order-details">
              <p>Date: {{ new Date(order.orderDate).toLocaleDateString() }}</p>
  
              <ul>
                <li v-for="item in order.orderItems" :key="item.id" class="order-item-details">
                  <div class="product-image-container">
                    <img :src="item.encodedImage" alt="Product Image" class="product-image" />
                  </div>
                  <div class="product-details">
                    <h3>{{ item.name }}</h3>
                    <p>Quantity: {{ item.quantity }}</p>
                    <p>Price per item: {{ item.price }} SEK</p>
                    <router-link :to="'/reviewitem/' + item.productId">
                    <button class="review-button">Write a Review</button>
                  </router-link>
                  </div>
                </li>
              </ul>
  
              <!-- Total price for this order -->
              <p>Total: {{ calculateOrderTotal(order.orderItems) }} SEK</p>
            </div>
  
            <hr />
          </li>
        </ul>
      </div>
  
      <!-- No orders message -->
      <div v-else-if="!isLoading && orders.length === 0">
        <p>You have no orders yet.</p>
      </div>
    </div>
  </template>
  
  <script>
  import { mapState, mapActions } from "vuex";
  
  export default {
    data() {
      return {
        expandedOrder: null, // Håller koll på vilken order som är expanderad
        products: {}, // För att lagra produktdata
       
      };
    },
    computed: {
      ...mapState(['orders', 'isLoading', 'errorMessage']),
  
    },
    methods: {
      ...mapActions(['fetchOrders']),
        async getProductsFromOrder(order) {
            order.orderItems = await Promise.all(order.orderItems.map(async (item) => {
                const response = await fetch(`https://localhost:7131/api/products/${item.productId}`)
                const data = await response.json();
                item.name = data.name;
                item.encodedImage = data.encodedImage;
                item.price = data.price
                return await item;
            }))
            console.log('updatedOrder', order)
        },
    
  
      toggleOrderDetails(orderId) {

        this.expandedOrder = this.expandedOrder === orderId ? null : orderId;
      },
  
      calculateOrderTotal(orderItems) {
        return orderItems.reduce((total, item) => total + (item.price * item.quantity), 0).toFixed(2);
      },
      sortOrders() {
      // Sortera ordrarna i fallande ordning baserat på orderDate
      this.orders.sort((a, b) => new Date(b.orderDate) - new Date(a.orderDate));
    },
    },
 
    async mounted() {
      if (this.$store.state.isAuthenticated) {
        await this.fetchOrders(); // Hämta ordrarna om användaren är inloggad
        this.orders.forEach(async (order) => {
            await this.getProductsFromOrder(order);
        });
        this.sortOrders();
      } else {
        this.errorMessage = "You need to be logged in to view your orders.";
      }
  
    },
  };
  </script>
  
  <style scoped>
  .orders-view {
    padding: 20px;
  }
  
  .loading {
    font-size: 18px;
    color: gray;
  }
  
  .error-message {
    color: red;
    font-weight: bold;
  }
  
  ul {
    list-style-type: none;
    padding: 0;
  }
  
  .order-item {
    margin-bottom: 20px;
  }
  
  .order-id {
    cursor: pointer;
    font-weight: bold;
    font-size: 1.2rem;
    color: #333;
  }
  
  .order-details {
    padding-left: 20px;
    margin-top: 10px;
  }
  
  .product-image-container {
    display: inline-block;
    width: 60px;
    height: 60px;
    margin-right: 20px;
  }
  
  .product-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
    border-radius: 5px;
  }
  
  .product-details {
    display: inline-block;
    vertical-align: top;
  }
  
  .product-details h3 {
    font-size: 1rem;
    margin: 0;
  }
  
  .product-details p {
    color: #888;
    font-size: 0.9rem;
  }
  
  hr {
    margin-top: 10px;
    border-color: #ccc;
  }
  </style>
  